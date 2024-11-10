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
    public Search Search { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Extract? Extract { get; set; } = null;
}
