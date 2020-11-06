using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoggerService;
using Microsoft.Extensions.DependencyInjection;

namespace GamingStore.Extensions
{
    public static class ServicesExtensions
    {
        public static void ConfigureLoggerService(this IServiceCollection services) => services.AddScoped<ILoggerManager, LoggerManager>();
    }
}
