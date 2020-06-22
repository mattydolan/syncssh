using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Filters;
using NLog;
using DddsUtils.Logging;
using Ninject;

namespace emriweb.Attributes
{
    /// <summary>
    /// Purpose of this filter is to intercept action responses.
    /// This filter will capture request as well as response data.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AuditLogFilterAttribute : ActionFilterAttribute
    {
        private ILogger _dbLogger;

        [Inject]
        public ILogFactory AuditLogFactory
        {
            set { _dbLogger = value.GetLogger("AuditLog"); }
        }

        /// <summary>
        /// this method is invoked once the action is executed.
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {

            string jsonContent = ReadContent(actionExecutedContext);
            string verb = actionExecutedContext.Request.Method.Method;

            string reqhdr = $"{actionExecutedContext.Request.Headers.Referrer}, {actionExecutedContext.Request.Headers.UserAgent}";
            string reshdr = $"{actionExecutedContext.Response.Headers.TransferEncoding}";
            string errormsg = null;
            if (actionExecutedContext.Exception != null)
                errormsg = $"{actionExecutedContext.Exception?.Message} {Environment.NewLine} {actionExecutedContext.Exception?.StackTrace}";

            Enum.TryParse(actionExecutedContext.Response.StatusCode.ToString(), out HttpStatusCode tm);
            var statuscode = tm;
            var apiendpoint = actionExecutedContext.Request.RequestUri.PathAndQuery;

            _dbLogger.Trace("{verb} {reqhdr} {jsonContent} {reshdr} {errormsg} {statuscode} {apiendpoint}",
                verb, reqhdr, jsonContent, reshdr, errormsg,statuscode, apiendpoint);

            base.OnActionExecuted(actionExecutedContext);
        }

        private string ReadContent(HttpActionExecutedContext actionExecutedContext)
        {
            string req = string.Empty;
            if (actionExecutedContext.Request.Properties.TryGetValue("rawpostdata", out object k))
            {
                req = k.ToString();
            }

            string res = string.Empty;
            if (actionExecutedContext.Response.Content != null)
                res = actionExecutedContext.Response.Content.ReadAsStringAsync().Result;

            return $" Request content {Environment.NewLine} {req} {Environment.NewLine} Response Content {Environment.NewLine} {res}";
        }
    }
}