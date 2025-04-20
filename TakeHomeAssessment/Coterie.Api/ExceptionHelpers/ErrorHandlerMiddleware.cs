using Coterie.Api.Models.ErrorHandler;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace Coterie.Api.ExceptionHelpers
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var response = context.Response;

            var appContext = context.Features.Get<IExceptionHandlerPathFeature>();

            BaseExceptionResponse ex = default;
            if (appContext?.Error != null)
            {
                switch (appContext.Error)
                {
                    case IndexOutOfRangeException:
                    case NullReferenceException:
                    case ArgumentException:
                        // Bad Request status
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                ex = new BaseExceptionResponse
                {
                    Message = appContext.Error.Message
                };

                await response.WriteAsJsonAsync(ex);
            }
            else
            {
                await _next(context);
            }

        }
    }
}
