using CPMCD.Dotvanta.Mail.Interfaces;
using CPMCD.Dotvanta.Mail.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CPMCD.Dotvanta.Mail
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// appsettings.json ke "MailSettings" section se config bind karega.
        /// Usage: builder.Services.AddCpmcdMailService(builder.Configuration);
        /// </summary>
        public static IServiceCollection AddCpmcdMailService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailOptions>(configuration.GetSection(MailOptions.SectionName));
            services.AddScoped<IMailService, MailService>();
            return services;
        }

        /// <summary>Config code se dena ho (appsettings.json use nahi karna) to ye overload.</summary>
        public static IServiceCollection AddCpmcdMailService(this IServiceCollection services, Action<MailOptions> configure)
        {
            services.Configure(configure);
            services.AddScoped<IMailService, MailService>();
            return services;
        }
    }
}
