using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace emri_models.Dto
{
    public class PracticeDetailDTO
    {
        /// <summary>
        /// Also known as GroupingID
        /// </summary>
        [JsonProperty("practice_id")]
        public string PracticeID { get; set; }

        [JsonProperty("npi")]
        public string PracticeNPI { get; set; }

        [JsonProperty("name")]
        public string PracticeName { get; set; }
    }
}
