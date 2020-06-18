using System;
using System.Collections.Generic;
using System.Text;

namespace emri_models.Dto
{
    public class EMRTrackingDTO
    {
        public int EMRIntegrationTrackingID { get; set; }
        public int TrackingTypeID { get; set; }
        public int EMRStatusID { get; set; }
        public string JsonShape { get; set; }
        /// <summary>
        /// utc format
        /// </summary>
        public DateTime? RequestedDateTime { get; set; }
        /// <summary>
        /// utc format
        /// </summary>
        public DateTime? SubmittedDateTime { get; set; }
        /// <summary>
        /// utc format
        /// </summary>
        public DateTime? PublishedDateTime { get; set; }
        /// <summary>
        /// utc format
        /// </summary>
        public DateTime? RespondedDateTime { get; set; }
    }
}
