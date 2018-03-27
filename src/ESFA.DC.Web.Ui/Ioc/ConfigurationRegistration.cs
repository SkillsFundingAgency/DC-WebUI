using Autofac;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.Models;
using DC.Web.Ui.Settings.Models;
using Microsoft.Extensions.Configuration;

namespace DC.Web.Ui.Ioc
{
    public static class ConfigurationRegistration
    {
        public static void SetupConfigurations(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.Register(c =>configuration.GetConfigSection<CloudStorageSettings>())
                .As<CloudStorageSettings>().SingleInstance();

            builder.Register(c =>
                    configuration.GetConfigSection<ServiceBusQueueSettings>())
                .As<ServiceBusQueueSettings>().SingleInstance();

            builder.Register(c =>
                    configuration.GetConfigSection<ConnectionStrings>())
                .As<ConnectionStrings>().SingleInstance();
        }
    }
}