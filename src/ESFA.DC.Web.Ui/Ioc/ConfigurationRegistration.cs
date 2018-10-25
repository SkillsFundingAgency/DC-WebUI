using Autofac;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Queueing.Interface.Configuration;
using Microsoft.Extensions.Configuration;

namespace DC.Web.Ui.Ioc
{
    public static class ConfigurationRegistration
    {
        public static void SetupConfigurations(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.Register(c =>
                    configuration.GetConfigSection<ConnectionStrings>())
                .As<ConnectionStrings>().SingleInstance();

            builder.Register(c =>
                    configuration.GetConfigSection<AuthenticationSettings>())
                .As<AuthenticationSettings>().SingleInstance();

            builder.Register(c =>
                    configuration.GetConfigSection<ApiSettings>())
                .As<ApiSettings>().SingleInstance();

            builder.Register(c =>
                    configuration.GetConfigSection<FeatureFlags>())
                .As<FeatureFlags>().SingleInstance();

            builder.Register(c => configuration.GetConfigSection<CloudStorageSettings>("EsfCloudStorageSettings"))
                .Keyed<IAzureStorageKeyValuePersistenceServiceConfig>(JobType.EsfSubmission).SingleInstance();
            builder.Register(c => configuration.GetConfigSection<CloudStorageSettings>("IlrCloudStorageSettings"))
                .Keyed<IAzureStorageKeyValuePersistenceServiceConfig>(JobType.IlrSubmission).SingleInstance();
            builder.Register(c => configuration.GetConfigSection<CloudStorageSettings>("EasCloudStorageSettings"))
                .Keyed<IAzureStorageKeyValuePersistenceServiceConfig>(JobType.EasSubmission).SingleInstance();

            builder.Register(c => configuration.GetConfigSection<CrossLoadingQueueConfiguration>()).As<IQueueConfiguration>().SingleInstance();
        }
    }
}