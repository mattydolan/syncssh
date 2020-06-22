using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace emri_models.Dto
{
    public class CDIAlertResponseMetaDTO
    {
        [JsonProperty(PropertyName = "clientid")]
        public string ClientID { get; set; }
        [JsonProperty(PropertyName = "clientname")]
        public string ClientName { get; set; }
        [JsonProperty(PropertyName = "lob")]
        public string LineOfBusiness { get; set; }

        [JsonProperty(PropertyName = "emr_tracking_id")]
        public int EMRTractingID { get; set; }
    }
}
