using System;
using System.Collections.Generic;
using System.Net.Http;
using Autofac;
using DC.Web.Authorization.Base;
using DC.Web.Authorization.Data.Repository;
using DC.Web.Authorization.FileSubmissionPolicy;
using DC.Web.Authorization.Query;
using DC.Web.Ui.Services;
using DC.Web.Ui.Services.AppLogs;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Services.Services;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.DateTime.Provider;
using ESFA.DC.DateTime.Provider.Interface;
using ESFA.DC.KeyGenerator;
using ESFA.DC.KeyGenerator.Interface;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
using ILogger = ESFA.DC.Logging.Interfaces.ILogger;

namespace DC.Web.Ui.Ioc
{
    public class ServiceRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AppLogsReader>().As<IAppLogsReader>().InstancePerLifetimeScope();
            builder.RegisterType<SubmissionService>().As<ISubmissionService>().InstancePerLifetimeScope();
            builder.RegisterType<AuthorizationPolicyService>().As<IAuthorizationPolicyService>().InstancePerLifetimeScope();
            builder.RegisterType<PermissionsQueryService>().As<IPermissionsQueryService>().InstancePerLifetimeScope();
            builder.RegisterType<AuthorizeRepository>().As<IAuthorizeRepository>().InstancePerLifetimeScope();
            builder.RegisterType<JobQueueService>().As<IJobQueueService>().InstancePerLifetimeScope();
            builder.RegisterType<BespokeHttpClient>().As<IBespokeHttpClient>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationErrorsService>().As<IValidationErrorsService>().InstancePerLifetimeScope();
            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>().InstancePerLifetimeScope();
            builder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>().InstancePerLifetimeScope();
            builder.RegisterType<CollectionManagementService>().As<ICollectionManagementService>().InstancePerLifetimeScope();
            builder.RegisterType<KeyGenerator>().As<IKeyGenerator>().InstancePerLifetimeScope();
            builder.RegisterType<ReportService>().As<IReportService>().InstancePerLifetimeScope();

            builder.Register(context =>
            {
                var config = new ApplicationLoggerSettings()
                {
                    ApplicationLoggerOutputSettingsCollection = new List<IApplicationLoggerOutputSettings>()
                    {
                        new MsSqlServerApplicationLoggerOutputSettings()
                        {
                            ConnectionString = context.Resolve<ConnectionStrings>().AppLogs
                        }
                    }
                };
                return new SeriLogger(config, new ExecutionContext() { JobId = "1" });
            }).As<ILogger>().InstancePerLifetimeScope();

            builder.Register(context =>
                {
                    var logger = context.Resolve<ILogger>();
                    var registry = new PolicyRegistry();
                    registry.Add(
                        "HttpRetryPolicy",
                        Policy.Handle<HttpRequestException>()
                            .WaitAndRetryAsync(
                                3, // number of retries
                                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // exponential backoff
                                (exception, timeSpan, retryCount, executionContext) =>
                                {
                                    logger.LogError("Error occured trying to send message to api", exception);
                                }));
                    return registry;
                }).As<IReadOnlyPolicyRegistry<string>>()
                .SingleInstance();
        }
    }
}