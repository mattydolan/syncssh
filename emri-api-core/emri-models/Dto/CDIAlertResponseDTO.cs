using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace emri_models.Dto
{
    public class CDIAlertResponseDTO
    {
        [JsonProperty(PropertyName = "cdi_alert_entity_id")]
        public int CDIAlertEntityID { get; set; }
        [JsonProperty(PropertyName = "cdi_alert_workflow_id")]
        public int CDIAlertWorkflowID { get; set; }

        [JsonProperty(PropertyName = "responses")]
        public IEnumerable<CDIAlertResponseDetailDTO> CDIAlertResponseDetails { get; set; }

        [JsonProperty(PropertyName = "meta")]
        public CDIAlertResponseMetaDTO CDIAlertResponseMeta { get; set; }

        [JsonProperty(PropertyName = "interaction_id")]
        public int InteractionID { get; set; }

        [JsonProperty(PropertyName = "vendor_tracking_status")]
        public string VendorTrackingStatus { get; set; }
    }
}
