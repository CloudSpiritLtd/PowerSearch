using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PowerSearch.Models;
using static System.Net.Mime.MediaTypeNames;

namespace PowerSearch.Runner;

public class SearchExecutor(int pipelineId, PipelineItem ppi, string path, string content, SearchResult? lastResult)
{
    private static readonly string[] _lineEndings = ["\r\n", "\r", "\n"];
    private readonly string[] lines = content.Split(_lineEndings, StringSplitOptions.None);

    public void Execute()
    {
        Results.Clear();

        switch (ppi.Search.Kind)
        {
            case SearchKind.Text:

                // todo: CaseSensitive
                var (Line, Column) = LocatePosition(ppi.Search.With);
                Results.Add(new()
                {
                    FileName = path,
                    Column = Column,
                    Line = Line,
                    Text = ppi.Search.With,   // 考虑大小写问题
                });
                break;

            case SearchKind.Wildcard:
                throw new NotImplementedException("Wildcard search is not supported yet.");

            case SearchKind.Regex:
                var pattern = ppi.Search.With;
                if (lastResult != null)
                {
                    //pattern = string.Format(pattern, lastResult.Groups);
                    pattern = pattern.Replace("{1}", lastResult.Text);
                }
                RegexOptions options = RegexOptions.None;
                if (ppi.Search.IgnoreCase)
                {
                    options |= RegexOptions.IgnoreCase;
                }
                Regex rx = new(pattern, options);
                var matches = rx.Matches(content);

                if (matches.Count > 0)
                {
                    //  do not use extract  || all matches
                    if (ppi.Extract.IsEmpty || ppi.Extract.UseAllMatches)
                    {
                        foreach (Match match in matches)
                        {
                            ExtractResult(match);
                        }
                    }
                    // match the specific one
                    // Extract.Match is 1-based.
                    else if (matches.Count > ppi.Extract.Match - 1)
                    {
                        var match = matches[ppi.Extract.Match - 1];
                        ExtractResult(match);
                    }
                }

                break;
        }
    }

    private (int Line, int Column) LocatePosition(int matchIndex)
    {
        if (lines.Length == 0)
            return (-1, -1);

        int currentCharIndex = 0;
        int matchLine = -1, matchColumn = -1;

        // 遍历每行，找到匹配项所在的行和列
        for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            string line = lines[lineIndex];
            int lineEndIndex = currentCharIndex + line.Length;

            //todo: 需要考虑 Condition.Extract, 被提取的组，不一定在匹配项开头。

            // 检查匹配项是否在当前行范围内
            if (matchIndex >= currentCharIndex && matchIndex < lineEndIndex)
            {
                // 计算匹配项的行列位置，行列从 1 开始
                matchLine = lineIndex + 1;
                matchColumn = matchIndex - currentCharIndex + 1;
                break;
            }

            // 更新当前字符索引到下一行的开始位置
            // 根据剩余文本推断换行符长度
            int newlineLength = 1; // 默认假设为 '\n'
            if (currentCharIndex + line.Length < content.Length)
            {
                char nextChar = content[currentCharIndex + line.Length];
                if (nextChar == '\r')
                {
                    // 判断是否为 "\r\n"
                    newlineLength = (currentCharIndex + line.Length + 1 < content.Length && content[currentCharIndex + line.Length + 1] == '\n') ? 2 : 1;
                }
            }

            currentCharIndex = lineEndIndex + newlineLength;
        }

        return (matchLine, matchColumn);
    }

    private (int Line, int Column) LocatePosition(string target)
    {
        if (lines.Length == 0 || string.IsNullOrEmpty(target))
            return (-1, -1);

        for (int i = 0; i < lines.Length; i++)
        {
            int column = lines[i].IndexOf(target, StringComparison.Ordinal);
            if (column >= 0)
            {
                // 行数和列数从 1 开始
                return (i + 1, column + 1);
            }
        }

        return (-1, -1);
    }

    private void ExtractResult(Match match)
    {
        string text;
        //  use group for extract && have enough groups to extract
        if (ppi.Extract.Group > 0 && match.Groups.Count > ppi.Extract.Group)
        {
            text = match.Groups[ppi.Extract.Group].Value;
        }
        else
        {
            text = match.Value;
        }

        var (line, column) = LocatePosition(match.Index);
        Results.Add(new()
        {
            FileName = path,
            Text = text,
            Column = column,
            Line = line,
        });
    }

    // pass to next executor
    public string Content { get => content; }

    // pass to next executor
    public string Path { get => path; }

    // pass to next executor
    public int PipelineId { get => pipelineId; }

    public List<SearchResult> Results { get; } = [];
}
