using DC.Web.Authorization;
using DC.Web.Authorization.AuthorizationHandlers;
using DC.Web.Authorization.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace DC.Web.Ui.StartupConfiguration
{
    public static class AuthorisationStartup
    {
        public static void AddAndConfigureAuthorisation(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyTypes.FileSubmission, policy => policy.Requirements.Add(new FileSubmissionPolicyRequirement()));
                options.AddPolicy(PolicyTypes.AdminAccess, policy => policy.Requirements.Add(new AdminAccessPolicyRequirement()));
            });

            services.AddSingleton<IAuthorizationHandler, AuthorizationPolicyHandler>();
        }
    }
}