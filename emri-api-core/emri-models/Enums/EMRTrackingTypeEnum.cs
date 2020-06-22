using System;
using System.Collections.Generic;
using System.Text;

namespace emri_models.Enums
{
    public enum EMRTrackingTypeEnum
    {
        AlertRequest = 1,
        AlertResponse = 2,
        MedicalRecord = 3,
        MemberRoster = 4,
        MRRNotify = 5,
        ProviderRoster = 6,
        QueryRequest = 7,
        QueryResponse = 8,
        Recent = 9
    }
}
