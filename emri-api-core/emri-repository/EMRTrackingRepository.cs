using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Extensions.Configuration;
using DddsUtils.Logging.NetStandard;
using NLog;
using emri_models.Dto;
using System.Data.Common;
using System.Data.SqlClient;
using Dapper;
using DddsUtils.Data.Context;
using System.Linq;

namespace emri_repository
{

    public interface IEMRTrackingRepository
    {
        int InsertEMRTrackingRecord(EMRTrackingDTO trackingDTO);
    }



    public class EMRTrackingRepository : IEMRTrackingRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly int? commandTimeout = 30;
        private IDapperContext _dapperContext;
        
        public EMRTrackingRepository(
            IConfiguration configuration,
            ILogFactory logFactory,
            IDapperContext dapperContext)
        {
            _configuration = configuration;
            _logger = logFactory.GetLogger("EMRTrackingRepository");
            if(int.TryParse(_configuration["ConnectionStrings:CommandTimeout"], out int ct))
            {
                commandTimeout = ct;
            }
            _dapperContext = dapperContext;
        }

        /// <summary>
        /// returns 0 in cases of error
        /// else will return the last value generated from identity
        /// </summary>
        /// <param name="trackingDTO"></param>
        /// <returns></returns>
        public int InsertEMRTrackingRecord(EMRTrackingDTO trackingDTO)
        {
            var cn = _configuration["ConnectionStrings:emri"];
            _logger?.Info("Starting InsertEMRTrackingRecord");

            var sql = @"INSERT INTO [dbo].[EMRIntegrationTracking]
                            ([TrackingTypeID],[EMRStatusID],[JsonShape],[RequestedDateTimeUTC],
                            [SubmittedDateTimeUTC],[PublishedDateTimeUTC],[RespondedDateTimeUTC])
                        VALUES(@TrackingTypeID,@EMRStatusID,@JsonShape,@RequestedDateTimeUTC,
                            @SubmittedDateTimeUTC,@PublishedDateTimeUTC,@RespondedDateTimeUTC)

                        Select IDENT_CURRENT('EMRIntegrationTracking') as id";

            _dapperContext.Connection.ConnectionString = cn;
            if(_dapperContext.Connection.State == ConnectionState.Closed)
            {
                _dapperContext.Connection.Open();
            }
            var response = _dapperContext.Query<DbResponse>(sql, 
                    new { 
                        trackingDTO.TrackingTypeID,
                        trackingDTO.EMRStatusID,
                        trackingDTO.JsonShape,
                        RequestedDateTimeUTC = trackingDTO.RequestedDateTime,
                        SubmittedDateTimeUTC = trackingDTO.SubmittedDateTime,
                        PublishedDateTimeUTC = trackingDTO.PublishedDateTime,
                        RespondedDateTimeUTC = trackingDTO.RespondedDateTime
                        }, CommandType.Text, commandTimeout.Value);

            if (_dapperContext.LastError?.HasErrors == true)
            {
                _dapperContext.LastError.Errors.ForEach((item) => _logger?.Error(item));
                return 0; //
            }
            _dapperContext.Connection.Close();
            var result = response.FirstOrDefault()?.Id;
            return result.HasValue ? result.Value : 0;

        }
    }
}
