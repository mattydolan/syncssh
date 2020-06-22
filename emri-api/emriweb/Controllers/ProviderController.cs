using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using NLog;
using DddsUtils.Logging;
namespace emriweb.Controllers
{
    public class ProviderController : ApiController
    {
        private readonly ILogger _logger;
        private const string loggerName = "ProviderApi";

        public ProviderController(ILogFactory logFactory)
        {
            _logger = logFactory.GetLogger(loggerName);
        }
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            _logger.Debug("Retrieve Provider data");
            return new string[] { "TODO Return Provider Data" };
        }
    }
}