using System;
using System.Collections.Generic;

using Moq;
using emri_models.Dto;
using DddsUtils.Logging.NetStandard;
using DddsUtils.Data.Context;
using Xunit;
using NLog;

using emri_repository;
using Microsoft.Extensions.Configuration;

using System.Data;


namespace emri_repository_tests
{
    public class EMRTrackingRepositoryTest
    {
        private Mock<ILogger> logger;
        private Mock<ILogFactory> logFactory;
        private Mock<IConfiguration> _configuration;
        private Mock<IDapperContext> _dapperContext;

        public EMRTrackingRepositoryTest()
        {
            logFactory = new Mock<ILogFactory>();
            logger = new Mock<ILogger>();
            _configuration = new Mock<IConfiguration>();
            _dapperContext = new Mock<IDapperContext>();
            logFactory.Setup(s => s.GetLogger(It.IsAny<string>())).Returns(logger.Object);
        }

        [Fact]
        public void EMRTrackingRepositoryConstructorTest()
        {
            var trackObj = new EMRTrackingRepository(_configuration.Object,
                logFactory.Object, _dapperContext.Object);

            Assert.IsType<EMRTrackingRepository>(trackObj);
        }

        [Fact]
        public void EMRTrackingRepositoryCommandTimeoutTest()
        {
            _configuration.Setup(x => x["ConnectionStrings:CommandTimeout"])
                .Returns("31");

            var trackObj = new EMRTrackingRepository(_configuration.Object,
                logFactory.Object, _dapperContext.Object);

            Assert.IsType<EMRTrackingRepository>(trackObj);
        }

        [Fact]
        public void EMRTrackingInsertRecordTest()
        {
            var localCfg = TestHelper.GetIConfigurationRoot();

            if(int.TryParse(localCfg["ConnectionStrings:CommandTimeout"], out int ct))
            {
                _configuration.Setup(x => x["ConnectionStrings:CommandTimeout"])
                            .Returns(ct.ToString());
            }

            var _connectionString = localCfg["ConnectionStrings:emri"];
            _configuration.Setup(y => y["ConnectionStrings:emri"])
                .Returns(_connectionString);

            var connection = new Mock<IDbConnection>();
            var res = new DbResponse { Id = 1 };
            var dbResponse = new List<DbResponse>();
            dbResponse.Add(res);

            _dapperContext.Setup(x => x.Connection).Returns(connection.Object);
            _dapperContext.Setup(y => y.Query<DbResponse>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CommandType>(), It.IsAny<int?>(),It.IsAny<string>()))
                .Returns(dbResponse);
                
            var trackObj = new EMRTrackingRepository(_configuration.Object,
                logFactory.Object,
                _dapperContext.Object);
            
            var result = trackObj.InsertEMRTrackingRecord(new EMRTrackingDTO());
            Assert.True(result == 1);
        }

        [Fact]
        public void EMRTrackingInsertRecordErrorTest()
        {
            var localCfg = TestHelper.GetIConfigurationRoot();

            if (int.TryParse(localCfg["ConnectionStrings:CommandTimeout"], out int ct))
            {
                _configuration.Setup(x => x["ConnectionStrings:CommandTimeout"])
                            .Returns(ct.ToString());
            }

            var _connectionString = localCfg["ConnectionStrings:emri"];
            _configuration.Setup(y => y["ConnectionStrings:emri"])
                .Returns(_connectionString);

            var connection = new Mock<IDbConnection>();
            var res = new DbResponse();
            res.Errors.Add("This is an error");

            var dbResponse = new List<DbResponse>();
            dbResponse.Add(res);

            _dapperContext.Setup(x => x.Connection).Returns(connection.Object);
            _dapperContext.Setup(y => y.Query<DbResponse>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CommandType>(), It.IsAny<int?>(), It.IsAny<string>()))
                .Returns(dbResponse);

            var trackObj = new EMRTrackingRepository(_configuration.Object,
                logFactory.Object,
                _dapperContext.Object);

            var result = trackObj.InsertEMRTrackingRecord(new EMRTrackingDTO());
            Assert.True(result == 0);
        }
    }
}
