using DddsUtils.Logging.NetStandard;
using emri_service.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NLog;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using emri_service.HttpClientHelper;
using System.IO;
using emri_models.Dto;
using emri_models.Enums;
using emri_repository;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace emri_service.Managers
{
    public class CDIAlertMananger : ICDIAlertManager
    {
        private readonly ILogger _logger;
        private HttpClient _httpClient;
        private readonly IMRRTokenManager _tokenProvider;
        private readonly IConfiguration _configuration;
        private readonly string _token;
        private IEMRTrackingRepository _emrRepo;

        public CDIAlertMananger(ILogFactory logFactory,
            IHttpClientFactory httpClientFactory,
            IMRRTokenManager tokenManager,
            IConfiguration configuration,
            IEMRTrackingRepository emrRepo)
        {
            _logger = logFactory.GetLogger("MRRHttpClientEndpoint");

            _httpClient = httpClientFactory.CreateClient();
            _tokenProvider = tokenManager;
            _configuration = configuration;
            _token = _tokenProvider.GetMRRToken();
            _emrRepo = emrRepo;
        }

        public async Task<IDictionary<string, string>> ValidateMember(string planMemberID)
        {
            _logger?.Info($"Validating member {planMemberID}");

            var error = new Dictionary<string, string>();
            var url = _configuration["MRR:mrrMemberUrl"];
            var endPoint = $"/MA/GetByMemberId?id={planMemberID}";
            var uri = string.Concat(url, endPoint);
            _httpClient.AddMRRDefaultHeaders(_token);
            var response = await _httpClient.GetAsync(uri);
            if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                error.Add("MemberID", "Plan Member ID is not valid");
                _logger?.Error($"Error while validating member {planMemberID}");
                return error;
            }
            _logger?.Info($"success validating member {planMemberID}");
            return error;
        }

        public async Task<IDictionary<string, string>> ValidateCDIAlertEntityID(int cdiAlertEnityId)
        {
            _logger?.Info($"Validating CDI Alert Entity ID {cdiAlertEnityId}");

            var error = new Dictionary<string, string>();
            var url = _configuration["MRR:mrrServiceUrl"];
            var endPoint = $"/MA/cdialerts/getByCdiAlertEnityId?cdiAlertentityId={cdiAlertEnityId}";
            var uri = string.Concat(url, endPoint);
            _httpClient.AddMRRDefaultHeaders(_token);
            var response = await _httpClient.GetAsync(uri);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                error.Add("CDIAlertEntityID", "CDI Alert Entity ID is not valid");
                _logger?.Error($"Error while validating CDI Alert Entity ID {cdiAlertEnityId}");
                return error;
            }
            _logger?.Info($"Success validating CDI Alert Entity ID {cdiAlertEnityId}");
            return error;
        }

        public int InsertEMRTrackingRecord(CDIAlertResponseDTO alertResponseDTO)
        {
            _logger?.Info($"Creating emr tracking record for cdi alert entity id : {alertResponseDTO.CDIAlertEntityID}");

            var json = JsonConvert.SerializeObject(alertResponseDTO);
            var dto = new EMRTrackingDTO
            {
                TrackingTypeID = (int)EMRTrackingTypeEnum.AlertResponse,
                EMRStatusID = (int)EMRStatusEnum.Ready,
                JsonShape = json,
                RequestedDateTime = DateTime.UtcNow,
                SubmittedDateTime = null,
                PublishedDateTime = null,
                RespondedDateTime = null
            };
            
            var id = _emrRepo.InsertEMRTrackingRecord(dto);
            if(id == 0)
            {
                _logger?.Error($"Error while emr tracking record for cdi alert entity id : {alertResponseDTO.CDIAlertEntityID}");
                return id;
            }

            _logger?.Info($"Successfully created emr tracking record for cdi alert entity id : {alertResponseDTO.CDIAlertEntityID}, trackingid {id}");
            return id;
        }
    }
}
