using DC.Web.Authorization;
using DC.Web.Authorization.FileSubmissionPolicy;
using DC.Web.Ui.AuthorizationHandlers;
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
            });

            services.AddSingleton<IAuthorizationHandler, FileSubmissionPolicyHandler>();
        }
    }
}