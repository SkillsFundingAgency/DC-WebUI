﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using Autofac;
using Autofac.Features.AttributeFilters;
using DC.Web.Authorization.Data.Repository;
using DC.Web.Ui.Services;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Services.Services;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.CrossLoad;
using ESFA.DC.CrossLoad.Dto;
using ESFA.DC.CrossLoad.Message;
using ESFA.DC.DateTimeProvider;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Queueing;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using Polly;
using Polly.Registry;
using ILogger = ESFA.DC.Logging.Interfaces.ILogger;

namespace DC.Web.Ui.Ioc
{
    public class ServiceRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JobService>().As<IJobService>().InstancePerLifetimeScope();
            builder.RegisterType<AuthorizeRepository>().As<IAuthorizeRepository>().InstancePerLifetimeScope();
            builder.RegisterType<BespokeHttpClient>().As<IBespokeHttpClient>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationResultsService>().As<IValidationResultsService>().InstancePerLifetimeScope();
            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>().InstancePerLifetimeScope();
            builder.RegisterType<JsonSerializationService>().As<ISerializationService>().InstancePerLifetimeScope();
            builder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>().SingleInstance();
            builder.RegisterType<CollectionManagementService>().As<ICollectionManagementService>().InstancePerLifetimeScope();
            builder.RegisterType<StorageService>().As<IStorageService>().WithAttributeFiltering().InstancePerLifetimeScope();
            builder.RegisterType<QueuePublishService<MessageCrossLoadDctToDcftDto>>().As<IQueuePublishService<MessageCrossLoadDctToDcftDto>>().InstancePerLifetimeScope();
            builder.RegisterType<CrossLoadMessageMapper>().InstancePerLifetimeScope();
            builder.RegisterType<ProviderService>().As<IProviderService>().InstancePerLifetimeScope();

            builder.RegisterType<IlrFileNameValidationService>().Keyed<IFileNameValidationService>(EnumJobType.IlrSubmission).WithAttributeFiltering().InstancePerLifetimeScope();
            builder.RegisterType<EsfFileNameValidationService>().Keyed<IFileNameValidationService>(EnumJobType.EsfSubmission).WithAttributeFiltering().InstancePerLifetimeScope();
            builder.RegisterType<EasFileNameValidationService>().Keyed<IFileNameValidationService>(EnumJobType.EasSubmission).WithAttributeFiltering().InstancePerLifetimeScope();

            builder.Register(context =>
            {
                var config = context.ResolveKeyed<IAzureStorageKeyValuePersistenceServiceConfig>(EnumJobType.IlrSubmission);
                return new AzureStorageKeyValuePersistenceService(config);
            }).Keyed<IStreamableKeyValuePersistenceService>(EnumJobType.IlrSubmission).InstancePerLifetimeScope();

            builder.Register(context =>
            {
                var config = context.ResolveKeyed<IAzureStorageKeyValuePersistenceServiceConfig>(EnumJobType.EsfSubmission);
                return new AzureStorageKeyValuePersistenceService(config);
            }).Keyed<IStreamableKeyValuePersistenceService>(EnumJobType.EsfSubmission).InstancePerLifetimeScope();

            builder.Register(context =>
            {
                var config = context.ResolveKeyed<IAzureStorageKeyValuePersistenceServiceConfig>(EnumJobType.EasSubmission);
                return new AzureStorageKeyValuePersistenceService(config);
            }).Keyed<IStreamableKeyValuePersistenceService>(EnumJobType.EasSubmission).InstancePerLifetimeScope();

            builder.Register(context =>
            {
                var config = context.ResolveKeyed<IAzureStorageKeyValuePersistenceServiceConfig>(EnumJobType.IlrSubmission);
                return new AzureStorageKeyValuePersistenceService(config);
            }).Keyed<IKeyValuePersistenceService>(EnumJobType.IlrSubmission).InstancePerLifetimeScope();

            builder.Register(context =>
            {
                var config = context.ResolveKeyed<IAzureStorageKeyValuePersistenceServiceConfig>(EnumJobType.EsfSubmission);
                return new AzureStorageKeyValuePersistenceService(config);
            }).Keyed<IKeyValuePersistenceService>(EnumJobType.EsfSubmission).InstancePerLifetimeScope();

            builder.Register(context =>
            {
                var config = context.ResolveKeyed<IAzureStorageKeyValuePersistenceServiceConfig>(EnumJobType.EasSubmission);
                return new AzureStorageKeyValuePersistenceService(config);
            }).Keyed<IKeyValuePersistenceService>(EnumJobType.EasSubmission).InstancePerLifetimeScope();

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