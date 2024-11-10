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
public class Profile
{
    public static Profile? LoadFromFile(string path) => LoadFromFile(new FileStream(path, FileMode.Open));

    public static Profile? LoadFromFile(Stream stream)
    {
        using StreamReader sr = new(stream, leaveOpen: true);
        var text = sr.ReadToEnd();
        return JsonConvert.DeserializeObject<Profile>(text);
    }

    public static void SaveToFile(Stream stream, Profile profile)
    {
        using StreamWriter sw = new(stream, leaveOpen: true);
        using JsonTextWriter jtw = new(sw);
        jtw.Indentation = 4;
        var serializer = JsonSerializer.CreateDefault(new() { Formatting = Formatting.Indented });
        serializer.Serialize(jtw, profile);

        //var text = JsonConvert.SerializeObject(profile, Formatting.Indented);
        //return sw.WriteAsync(text);
    }

    public static void SaveToFile(string path, Profile profile) => SaveToFile(new FileStream(path, FileMode.Create), profile);

    public List<PipelineItem> Pipeline { get; set; } = [];

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<string> Exclude { get; set; } = [];

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<string> Include { get; set; } = [];

    public bool Recursive { get; set; } = true;
}
