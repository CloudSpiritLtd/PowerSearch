using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PowerSearch.Models;


[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class PipelineItem
{
    public bool ShouldSerializeExtract()
    {
        return UseExtract;
    }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Search Search { get; set; } = new();

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Extract Extract { get; set; } = new();

    [JsonIgnore]
    public bool UseExtract { get; set; } = false;
}
