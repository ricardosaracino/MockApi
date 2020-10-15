using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog;
using Microsoft.IdentityModel.Logging;
using ILogger = Serilog.ILogger;

namespace MockApi.Controllers
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private ILogger _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = Log.Logger;
        }
    
        public async Task Invoke(HttpContext context)
        {  
            _logger.Information(await FormatRequest(context.Request));

            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await _next(context);

                _logger.Information(await FormatResponse(context.Response));
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
    
        private async Task<string> FormatRequest(HttpRequest request)
        {
            var body = request.Body;
            request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body = body;

            return $"REQUEST {request.Scheme} {request.Host} {request.Path} {request.QueryString} {bodyAsText}";
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync(); 
            response.Body.Seek(0, SeekOrigin.Begin);

            return $"RESPONSE {text}";
        }
    }
}