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
    public bool IsEmpty()
    {
        return Match == -1 && Group == -1;
    }

    public int Match { get; set; } = -1;

    public int Group { get; set; } = -1;
}
