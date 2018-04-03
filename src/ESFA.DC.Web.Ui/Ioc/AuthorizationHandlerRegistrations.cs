using Autofac;
using DC.Web.Authorization.FileSubmissionPolicy;
using DC.Web.Ui.AuthorizationHandlers;
using DC.Web.Ui.Settings.Models;
using Microsoft.AspNetCore.Authorization;

namespace DC.Web.Ui.Ioc
{
    public class AuthorizationHandlerRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context =>
                {
                    var authenticationSettings = context.Resolve<AuthenticationSettings>();
                    var policy = context.Resolve<IFileSubmissionPolicyService>();
                    return new FileSubmissionPolicyHandler(policy, authenticationSettings);
                })
                .As<IAuthorizationHandler>()
                .SingleInstance();
        }
    }
}