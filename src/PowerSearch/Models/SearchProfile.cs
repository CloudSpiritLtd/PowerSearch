using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PowerSearch.Runner;
using ReactiveUI.Fody.Helpers;

namespace PowerSearch.Models;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class SearchProfile
{
    public static SearchProfile? LoadFromFile(string path) => LoadFromFile(new FileStream(path, FileMode.Open));

    public static SearchProfile? LoadFromFile(Stream stream)
    {
        using StreamReader sr = new(stream, leaveOpen: true);
        var text = sr.ReadToEnd();
        return JsonConvert.DeserializeObject<SearchProfile>(text);
    }

    public static void SaveToFile(Stream stream, SearchProfile profile)
    {
        using StreamWriter sw = new(stream, leaveOpen: true);
        using JsonTextWriter jtw = new(sw);
        jtw.Indentation = 4;
        var serializer = JsonSerializer.CreateDefault(new() { Formatting = Formatting.Indented });
        serializer.Serialize(jtw, profile);

        //var text = JsonConvert.SerializeObject(profile, Formatting.Indented);
        //return sw.WriteAsync(text);
    }

    public static void SaveToFile(string path, SearchProfile profile) => SaveToFile(new FileStream(path, FileMode.Create), profile);

    [Obsolete]
    public async Task<IEnumerable<SearchResult>> Run(string path)
    {
        Debug.Assert(Conditions.Count > 0);
        Debug.Assert(!string.IsNullOrEmpty(path));

        var results = new ConcurrentBag<SearchResult>();

        // Collect files
        // todo: change searchPattern
        var files = Directory.EnumerateFiles(path, "*.*", Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        List<Task> tasks = [];
        foreach (var file in files)
        {
            /*
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var r = SearchInFile(file);
                results.Add(r);
            }));
            */
            var r = SearchInFile(file);
            results.Add(r);
        }

        await Task.WhenAll(tasks);
        var query = from r in results
                    orderby r.FileName
                    select r;
        return query;
    }

    [Obsolete]
    private SearchResult SearchInFile(string file)
    {
        using StreamReader sr = new(file);
        var text = sr.ReadToEnd();

        SearchExecutor? last = null;
        SearchExecutor curr;
        for (int i = 1; i < Conditions.Count; i++)
        {
            curr = new(Conditions[i]);
            curr.Execute(text, last?.Result);
            last = curr;
        }

        return new()
        {
            FileName = file,
            Text = last!.Result.Text,
            Line = last!.Result.Line,
            Column = last!.Result.Column,
        };
    }

    [JsonProperty("conditions")]
    public List<Condition> Conditions { get; set; } = [];

    //[JsonProperty("exclude")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<string> Exclude { get; set; } = [];

    //[JsonProperty("include")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<string> Include { get; set; } = [];

    //[JsonProperty("recursive")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool Recursive { get; set; } = true;
}
