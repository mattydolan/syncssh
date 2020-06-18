using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace emri_models.Dto
{
    public class PhysicianResponseDTO
    {
        [JsonProperty(PropertyName = "option_id")]
        public int OptionID { get; set; }
    }
}
