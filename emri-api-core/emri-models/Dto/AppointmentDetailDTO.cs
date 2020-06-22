using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace emri_models.Dto
{
    public class AppointmentDetailDTO
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("type")]
        public string AppointmentType { get; set; }

        [JsonProperty("datetime")]
        public string ResponseDatetime { get; set; }

    }
}
