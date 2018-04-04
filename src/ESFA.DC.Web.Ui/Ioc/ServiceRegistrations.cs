using Autofac;
using DC.Web.Authorization.Data.Query;
using DC.Web.Authorization.FileSubmissionPolicy;
using DC.Web.Ui.AuthorizationHandlers;
using DC.Web.Ui.Controllers;
using DC.Web.Ui.Services.AppLogs;
using DC.Web.Ui.Services.ServiceBus;
using DC.Web.Ui.Services.SubmissionService;
using DC.Web.Ui.Settings.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.ServiceBus;

namespace DC.Web.Ui.Ioc
{
    public class ServiceRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(FileSubmissionPolicyService)).As(typeof(IFileSubmissionPolicyService)).InstancePerLifetimeScope();

            builder.RegisterType<ServiceBusQueue>().As<IServiceBusQueue>().InstancePerLifetimeScope();
            builder.RegisterType<AppLogsReader>().As<IAppLogsReader>().InstancePerLifetimeScope();
            builder.RegisterType<SubmissionService>().As<ISubmissionService>().InstancePerLifetimeScope();
            builder.RegisterType<FileSubmissionPolicyService>().As<IFileSubmissionPolicyService>().InstancePerLifetimeScope();
            builder.RegisterType<PermissionsQueryService>().As<IPermissionsQueryService>().InstancePerLifetimeScope();

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