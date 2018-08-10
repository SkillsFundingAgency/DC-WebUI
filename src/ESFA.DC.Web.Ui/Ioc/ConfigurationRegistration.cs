using Autofac;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DC.Web.Ui.Ioc
{
    public static class ConfigurationRegistration
    {
        public static void SetupConfigurations(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.Register(c => configuration.GetConfigSection<CloudStorageSettings>())
                .As<CloudStorageSettings>().SingleInstance();

            builder.Register(c =>
                    configuration.GetConfigSection<ConnectionStrings>())
                .As<ConnectionStrings>().SingleInstance();

            builder.Register(c =>
                    configuration.GetConfigSection<AuthenticationSettings>())
                .As<AuthenticationSettings>().SingleInstance();

            builder.Register(c =>
                    configuration.GetConfigSection<ApiSettings>())
                .As<ApiSettings>().SingleInstance();

            builder.Register(c => configuration.GetConfigSection<CloudStorageSettings>())
                .As<IAzureStorageKeyValuePersistenceServiceConfig>().SingleInstance();
        }
    }
}