using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace emri_models.Dto
{
    public class PhysicianDetailDTO
    {
        [JsonProperty("provider_id")]
        public string ProviderID { get; set; }

        [JsonProperty("internal_provider_id")]
        public int ProviderMasterID { get; set; }

        [JsonProperty("npi")]
        public string NPI { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("middle_name")]
        public string MiddleName { get; set; }
    }
}
