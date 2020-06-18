using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace emri_models.Dto
{
    public class CDIAlertResponseDetailDTO
    {
        [JsonProperty(PropertyName = "cdi_alert_detail_id")]
        public int CDIAlertEntityDetailID { get; set; }

        [JsonProperty(PropertyName = "physician_response")]
        public PhysicianResponseDTO PhysicianResponse { get; set; }
        [JsonProperty(PropertyName = "response_time_stamp")]
        public string ResponseTimeStamp { get; set; }

        [JsonProperty(PropertyName = "appointment_details")]
        public AppointmentDetailDTO AppointmentDetail { get; set; }

        [JsonProperty(PropertyName = "physician_details")]
        public PhysicianDetailDTO PhysicianDetail { get; set; }

        [JsonProperty(PropertyName = "practice_details")]
        public PracticeDetailDTO PracticeDetail { get; set; }

        [JsonProperty(PropertyName = "comments")]
        public string Comments { get; set; }
    }
}
