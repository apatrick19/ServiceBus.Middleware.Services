using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ServiceBus.Web.Injection
{
    public class CustomLogHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var logMetadata =  await BuildRequestMetadata(request);
            var response = await base.SendAsync(request, cancellationToken);
            logMetadata = await BuildResponseMetadata(logMetadata, response);
            await SendToLog(logMetadata);
            return response;
        }
        private async Task<LogMetadata> BuildRequestMetadata(HttpRequestMessage request)
        {
            LogMetadata log = new LogMetadata
            {
                RequestMethod = request.Method.Method,
                RequestTimestamp = DateTime.Now,
                RequestUri = request.RequestUri.ToString(),
                RequestContentType = await request.Content.ReadAsStringAsync()
        };
            return log;
        }
        private async Task<LogMetadata> BuildResponseMetadata(LogMetadata logMetadata, HttpResponseMessage response)
        {
            logMetadata.ResponseStatusCode = response.StatusCode;
            logMetadata.ResponseTimestamp = DateTime.Now;
           // logMetadata.ResponseContentType = await response.Content.ReadAsStringAsync();
            return logMetadata;
        }
        private async Task<bool> SendToLog(LogMetadata logMetadata)
        {
            // TODO: Write code here to store the logMetadata instance to a pre-configured log store...
            Trace.TraceInformation("<------------------------------------------------------------------------------------------------>");
            Trace.TraceInformation($"Method: {logMetadata.RequestMethod??""}; \n\n\n\n Time: {logMetadata.RequestTimestamp}; \n\n\n\n\n\n URL: {logMetadata.RequestUri??""}; \n\n\n\n\n Request Message{logMetadata.RequestContentType??""}; \n\n\n\n\n Response Code:{logMetadata.ResponseStatusCode}; \n\n\n\n Response time: {logMetadata.ResponseTimestamp}; \n\n\n\n\n Response Message{logMetadata.ResponseContentType}; ");
            Trace.TraceInformation("<------------------------------------------------------------------------------------------------>");
            return true;
        }
    }
}