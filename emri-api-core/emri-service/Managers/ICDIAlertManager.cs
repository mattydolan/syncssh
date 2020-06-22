using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using emri_models.Dto;

namespace emri_service.Managers
{
    public interface ICDIAlertManager
    {
        Task<IDictionary<string, string>> ValidateMember(string planMemberID);
        Task<IDictionary<string, string>> ValidateCDIAlertEntityID(int cdiAlertEntityID);
        /// <summary>
        /// if 0 some error
        /// > 0 successful
        /// </summary>
        /// <param name="alertResponseDTO"></param>
        /// <returns>return EMRIntegrationTrackingID</returns>
        int InsertEMRTrackingRecord(CDIAlertResponseDTO alertResponseDTO);
    }
}
