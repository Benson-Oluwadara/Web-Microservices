using Mango.Services.EmailAPI.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mango.Services.EmailAPI.Extension
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            var hostApplicationLife = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            hostApplicationLife.ApplicationStarted.Register(() => OnStart(app));
            hostApplicationLife.ApplicationStopping.Register(() => OnStop(app));

            return app;
        }

        private static void OnStop(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var serviceBusConsumer = scope.ServiceProvider.GetRequiredService<IAzureServiceBusConsumer>();
                serviceBusConsumer.Stop();
            }
        }

        private static void OnStart(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var serviceBusConsumer = scope.ServiceProvider.GetRequiredService<IAzureServiceBusConsumer>();
                serviceBusConsumer.Start();
            }
        }
    }
}
