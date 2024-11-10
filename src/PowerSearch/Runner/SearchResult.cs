using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSearch.Runner;

public class SearchResult
{
    public static readonly SearchResult Empty = new();

    public string FileName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int Line { get; set; } = -1;
    public int Column { get; set; } = -1;
}
