using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OpenRIMS.PV.Main.API.Helpers;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Infrastructure;
using System;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Infrastructure.Configs.ExceptionHandler
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private MainDbContext _AppDbContext;

        public ExceptionMiddleware(RequestDelegate next, MainDbContext _appDbContext)
        {
            _next = next;
            _AppDbContext = _appDbContext;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var message = new ExceptionMessage()
            {
                StatusCode = (HttpStatusCode)context.Response.StatusCode,
                Message = exception.Message,
            };

            try
            {
                if (message.StatusCode == HttpStatusCode.InternalServerError)
                {
                    message.ReferenceCode = UniqueCodeHelper.NormalCode();

                    var frame = new StackTrace(exception, true).GetFrame(0);
                    Guid.TryParse((context.User.Identity as ClaimsIdentity).FindFirst("id")?.Value, out Guid identityId);
                    _AppDbContext.SystemLogs.Add(new SystemLog(
                        $"{frame.GetMethod()?.ReflectedType?.Namespace}.{frame.GetMethod()?.ReflectedType?.Name}.{frame.GetMethod()?.Name} [{frame.GetFileLineNumber()}]",
                        ((HttpStatusCode)context.Response?.StatusCode).ToString(), message.ReferenceCode, exception.Message)
                    {
                        ExceptionStackTrace = exception.StackTrace,
                        InnerExceptionMessage = exception.InnerException?.Message,
                        InnerExceptionStackTrace = exception.InnerException?.StackTrace,
                        RemoteIpAddress = context?.Connection?.RemoteIpAddress?.ToString(),
                    });
                    _AppDbContext.SaveEntities();
                }
            }
            catch (Exception) { }

            return context.Response.WriteAsync(JsonConvert.SerializeObject(message));
        }
    }

    public class ExceptionMessage
    {
        public HttpStatusCode StatusCode { get; set; }

        public string StatusCodeType { get { return StatusCode.ToString(); } }

        public string Message { get; set; }

        public string ReferenceCode { get; set; }
    }
}

