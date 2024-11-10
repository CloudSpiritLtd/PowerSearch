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
public class Search //: ICondition
{
    [JsonConverter(typeof(StringEnumConverter))]
    public SearchKind Kind { get; set; } = SearchKind.Regex;

    public string With { get; set; } = string.Empty;

    public bool IgnoreCase { get; set; } = true;

}

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class Extract : IExtract
{
    public int Match { get ; set ; }

    public int Group { get ; set ; }
}
