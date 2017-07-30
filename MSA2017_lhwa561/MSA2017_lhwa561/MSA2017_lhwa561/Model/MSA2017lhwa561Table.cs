using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MSA2017_lhwa561.Model
{
    public class MSA2017lhwa561Table
    {
        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "Gender")]
        public string Gender { get; set; }
        [JsonProperty(PropertyName = "Age")]
        public double Age { get; set; }
        [JsonProperty(PropertyName = "Emotion")]
        public string Emotion { get; set; }
    }
}