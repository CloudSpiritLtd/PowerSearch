using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PowerSearch.Models;

namespace PowerSearch.Runner;

public class SearchExecutor(Condition cond)
{
    private readonly Condition _cond = cond;

    public void Execute(string content, SearchResult? lastResult = null)
    {
        Success = false;
        switch (_cond.Kind)
        {
            case ConditionKind.Text:
                // var i = content.IndexOf(_filter.Expression);
                // todo: CaseSensitive
                var (Line, Column) = LocatePosition(content, _cond.Expression);
                Result.Column = Column;
                Result.Line = Line;
                Result.Text = _cond.Expression;   // 考虑大小写问题
                break;
            case ConditionKind.Wildcard:
                break;
            case ConditionKind.Regex:
                var pattern = _cond.Expression;
                if (lastResult != null)
                {
                    //pattern = string.Format(pattern, lastResult.Groups);
                    pattern = pattern.Replace("{1}", lastResult.Text);
                }
                RegexOptions options = RegexOptions.None;
                if (_cond.IgnoreCase)
                {
                    options |= RegexOptions.IgnoreCase;
                }
                Regex rx = new(pattern, options);
                var matches = rx.Matches(content);
                if (matches.Count > 0)
                {
                    Match? match = null;
                    if (_cond.Extract == null)
                    {
                        match = matches[0];
                        Result.Text = match.Value;
                    }
                    else if (matches.Count > _cond.Extract.Match - 1)
                    {
                        match = matches[_cond.Extract.Match - 1];
                        if (match.Groups.Count > _cond.Extract.Group)
                        {
                            Result.Text = match.Groups[_cond.Extract.Group].Value;
                        }
                    }

                    if (match != null)
                    {
                        var (Line1, Column1) = LocatePosition(content, match);
                        Result.Column = Column1;
                        Result.Line = Line1;
                        Success = true;
                    }
                }
                break;
        }
    }

    private static (int Line, int Column) LocatePosition(string text, Match match)
    {
        var lines = text.Split(lineEndings, StringSplitOptions.None);
        int currentCharIndex = 0;
        int matchLine = -1, matchColumn = -1;

        // 遍历每行，找到匹配项所在的行和列
        for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            string line = lines[lineIndex];
            int lineEndIndex = currentCharIndex + line.Length;

            //todo: 需要考虑 Condition.Extract, 被提取的组，不一定在匹配项开头。

            // 检查匹配项是否在当前行范围内
            if (match.Index >= currentCharIndex && match.Index < lineEndIndex)
            {
                // 计算匹配项的行列位置，行列从 1 开始
                matchLine = lineIndex + 1;
                matchColumn = match.Index - currentCharIndex + 1;
                break;
            }

            // 更新当前字符索引到下一行的开始位置
            // 根据剩余文本推断换行符长度
            int newlineLength = 1; // 默认假设为 '\n'
            if (currentCharIndex + line.Length < text.Length)
            {
                char nextChar = text[currentCharIndex + line.Length];
                if (nextChar == '\r')
                {
                    // 判断是否为 "\r\n"
                    newlineLength = (currentCharIndex + line.Length + 1 < text.Length && text[currentCharIndex + line.Length + 1] == '\n') ? 2 : 1;
                }
            }

            currentCharIndex = lineEndIndex + newlineLength;
        }

        return (matchLine, matchColumn);
    }

    private static (int Line, int Column) LocatePosition(string text, string target)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(target))
            return (-1, -1);

        var lines = text.Split(lineEndings, StringSplitOptions.None);

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

    public SearchResult Result { get; set; } = new();

    public bool Success { get; set; }

    private static readonly string[] lineEndings = ["\r\n", "\r", "\n"];
}
