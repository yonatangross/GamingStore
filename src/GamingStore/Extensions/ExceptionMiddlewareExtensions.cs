using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GamingStore.Contracts;
using LoggerService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace GamingStore.Extensions
{
    public static class ExceptionMiddlewareExtensions 
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILoggerManager logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                    
                    string errorCode = Guid.NewGuid().ToString().Split('-').Last();
                    logger.LogError($"[{errorCode}] Exception: {exceptionHandlerFeature?.Error}");
                    context.Response.Redirect($"/Errors/InternalServer?code={errorCode}");
                });
            });
        }
    }
}