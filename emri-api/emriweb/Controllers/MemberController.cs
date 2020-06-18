using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using NLog;
using DddsUtils.Logging;

namespace emriweb.Controllers
{
    public class MemberController : ApiController
    {
        private readonly ILogger _logger;
        private const string loggerName = "MemberApi";

        public MemberController(ILogFactory logFactory)
        {
            _logger = logFactory.GetLogger(loggerName);
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.Debug("Retrieve Member data");
            return new string[] { "TODO Return Member Data" };
        }

    }
}