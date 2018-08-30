using Autofac;
using DC.Web.Authorization.AuthorizationHandlers;
using DC.Web.Authorization.Base;
using DC.Web.Authorization.FileSubmissionPolicy;
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
                    var policy = context.Resolve<IAuthorizationPolicyService>();
                    return new AuthorizationAuthorizationPolicyHandler(policy, authenticationSettings);
                })
                .As<IAuthorizationHandler>()
                .SingleInstance();
        }
    }
}