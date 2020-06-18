using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using NLog;
using System;
using System.IO;
using System.Net;
using DddsUtils.Logging.NetStandard;

namespace emri_service.Middleware
{
	/// <summary>
	/// Intercepts requests to capture request and response payloads for logging.
	/// </summary>
	public class AuditLogMiddleware
	{
        private readonly RequestDelegate _next;
        private ILogger _dbLogger;

        public AuditLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILogFactory logFactory)
        {
            _dbLogger = logFactory.GetLogger("AuditLog"); //todo: move elsewhere? (can't be in constructor)
            string httpVerb = context.Request.Method;

            if (httpVerb == "POST")
            {
                string jsonContent = ReadContent(context);
                WriteLog(context, jsonContent);
            }

            await _next(context);
        }

        // write log record to AuditLog table(s)
        private void WriteLog(HttpContext context, string jsonContent)
        {
            string reqHdr = $"{context.Request.Headers["Referer"]}," +
            $"{context.Request.Headers["User-Agent"]}";
            string resHdr = $"{context.Response.Headers["TransferEncoding"]}";
            string errorMsg = null;

            //todo(?)
            //if (actionExecutedContext.Exception != null)
            //    errorMsg = $"{actionExecutedContext.Exception?.Message} {Environment.NewLine} {actionExecutedContext.Exception?.StackTrace}";

            Enum.TryParse(context.Response.StatusCode.ToString(), out HttpStatusCode tm);
            var statuscode = tm;
            var apiendpoint = context.Request.GetEncodedPathAndQuery();

            _dbLogger.Trace("{verb} {reqhdr} {jsonContent} {reshdr} {errormsg} {statuscode} {apiendpoint}",
                context.Request.Method, reqHdr, jsonContent, resHdr, errorMsg, statuscode, apiendpoint);
        }

        // parse request and response bodies
        private string ReadContent(HttpContext context)
        {
            string requestBody = "";
            if (context.Request.Body.CanSeek)
            {
                context.Request.Body.Seek(0, SeekOrigin.Begin);
                using (StreamReader stream = new StreamReader(context.Request.Body))
                {
                    requestBody = stream.ReadToEnd();
                }
            }

            string responseBody = "";
            if (context.Response.Body.CanSeek)
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                using (StreamReader stream = new StreamReader(context.Response.Body))
                {
                    responseBody = stream.ReadToEnd();
                }
            }

            return $" Request content {Environment.NewLine} {requestBody} {Environment.NewLine}" +
                $"Response Content {Environment.NewLine} {responseBody}";
        }
    }

    public static class AuditLogExtension
    {
        public static IApplicationBuilder UseAuditLog(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuditLogMiddleware>();
        }
    }
}
