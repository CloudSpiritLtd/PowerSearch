using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerSearch.Models;

namespace PowerSearch.Runner;

public class SimpleRunner(Profile profile)
{
    private readonly Profile _profile = profile;

    public async IAsyncEnumerable<SearchResult> Run(string rootFolder)
    {
        // todo: 要考虑 profile.Includes & Excludes
        var files = Directory.EnumerateFiles(rootFolder, "*.*", _profile.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        foreach (var file in files)
        {
            var content = await File.ReadAllTextAsync(file, Encoding.UTF8);
            SearchResult? lastResult = null;
            foreach (var item in _profile.Pipeline)
            {
                SearchExecutor exec = new(item.Search, item.Extract);
                exec.Execute(content, lastResult);
                if (exec.Success)
                {
                    lastResult = exec.Result;
                }
                else
                {
                    lastResult = null;
                    break;
                }
            }

            if (lastResult != null)
            {
                yield return new()
                {
                    FileName = file,
                    Text = lastResult!.Text,
                    Line = lastResult!.Line,
                    Column = lastResult!.Column,
                };
            }
        }
    }
}
