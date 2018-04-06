﻿using Autofac;
using DC.Web.Authorization.Base;
using DC.Web.Authorization.Data.Entities;
using DC.Web.Authorization.Data.Repository;
using DC.Web.Authorization.FileSubmissionPolicy;
using DC.Web.Authorization.Query;
using DC.Web.Ui.Services.AppLogs;
using DC.Web.Ui.Services.ServiceBus;
using DC.Web.Ui.Services.SubmissionService;
using DC.Web.Ui.Settings.Models;
using Microsoft.Azure.ServiceBus;

namespace DC.Web.Ui.Ioc
{
    public class ServiceRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ServiceBusQueue>().As<IServiceBusQueue>().InstancePerLifetimeScope();
            builder.RegisterType<AppLogsReader>().As<IAppLogsReader>().InstancePerLifetimeScope();
            builder.RegisterType<SubmissionService>().As<ISubmissionService>().InstancePerLifetimeScope();
            builder.RegisterType<PolicyService>().As<IPolicyService>().InstancePerLifetimeScope();
            builder.RegisterType<PermissionsQueryService>().As<IPermissionsQueryService>().InstancePerLifetimeScope();
            builder.RegisterType<AuthorizeRepository>().As<IAuthorizeRepository>().InstancePerLifetimeScope();
            builder.Register(context =>
                {
                    var queueSettings = context.Resolve<ServiceBusQueueSettings>();
                    return new QueueClient(queueSettings.ConnectionString, queueSettings.Name);
                })
                .As<IQueueClient>()
                .InstancePerLifetimeScope();
        }
    }
}