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
public class Condition //: ICondition
{
    [JsonConverter(typeof(StringEnumConverter))]
    public ConditionKind Kind { get; set; } = ConditionKind.Regex;

    public string Expression { get; set; } = string.Empty;

    public bool IgnoreCase { get; set; } = true;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Extract? Extract { get; set; } = null;
}

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class Extract : IExtract
{
    public int Match { get ; set ; }

    public int Group { get ; set ; }
}
