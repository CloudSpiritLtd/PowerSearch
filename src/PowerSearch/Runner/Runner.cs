using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using PowerSearch.Models;

namespace PowerSearch.Runner;

public class SimpleRunner(Profile profile)
{
    private readonly Profile _profile = profile;
    private readonly ConcurrentQueue<SearchExecutor> _tasks = new();

    public Task Run(string rootFolder)
    {
        // todo: 要考虑 profile.Includes & Excludes
        var files = Directory.EnumerateFiles(rootFolder, "*.*", _profile.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        foreach (var file in files)
        {
            // todo: feat: detect encoding, or let user choose
            // todo: perf: use mmap to scan file
            // todo: perf: async
            var content = File.ReadAllText(file, Encoding.UTF8);

            PipelineItem ppi = _profile.Pipeline.First();
            SearchExecutor exec = new(0, ppi, file, content, null);
            _tasks.Enqueue(exec);
        }

        return Task.Factory.StartNew(() =>
        {
            while (true)
            {
                if (_tasks.IsEmpty)
                {
                    //await Task.Delay(1000);
                    Thread.Sleep(50);

                    if (_tasks.IsEmpty)
                    {
                        Thread.Sleep(1000);
                        if (_tasks.IsEmpty)
                            break;
                    }
                }

                if (_tasks.TryDequeue(out var exec))
                {
                    exec.Execute();
                    if (exec.Results.Count > 0)
                    {
                        // reach the end of pipeline, emit results.
                        if (exec.PipelineId == _profile.Pipeline.Count - 1)
                        {
                            Results.AddRange(exec.Results);
                        }
                        // create search task with next pipeline item
                        else
                        {
                            int nextId = exec.PipelineId + 1;
                            var ppi = _profile.Pipeline[nextId];
                            foreach (var result in exec.Results)
                            {
                                SearchExecutor nextExec = new(nextId, ppi, exec.Path, exec.Content, result);
                                _tasks.Enqueue(nextExec);
                            }
                        }
                    }

                    //todo: if no result?

                    //await Task.Delay(1);
                    Thread.Sleep(1);
                }
            }
        });
    }

    public List<SearchResult> Results { get; } = [];
}
