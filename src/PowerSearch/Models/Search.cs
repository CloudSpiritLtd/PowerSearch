using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace PowerSearch.Models;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class Search
{
    [JsonConverter(typeof(StringEnumConverter))]
    public SearchKind Kind { get; set; } = SearchKind.Regex;

    public string With { get; set; } = string.Empty;

    public bool IgnoreCase { get; set; } = true;
}

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class Extract
{
    /// <summary>
    /// The <see cref="Extract" /> will be ignored when it is empty.
    /// </summary>
    public bool IsEmpty => Match == -1 && Group == -1;

    /// <summary>
    /// If true, each match will emit a result.
    /// If false, specific <see cref="Match"/> will be used to emit result.
    /// </summary>
    public bool UseAllMatches => Match == 0;

    /// <summary>
    /// Specific match index, 1-based.
    /// 0 for use all. -1 for don't use.
    /// </summary>
    public int Match { get; set; } = -1;

    /// <summary>
    /// Specific group index, 1-based.
    /// 0 is whole match. -1 for don't use.
    /// </summary>
    public int Group { get; set; } = -1;
}
