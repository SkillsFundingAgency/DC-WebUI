using DC.Web.Authorization.Data.Constants;
using DC.Web.Authorization.FileSubmissionPolicy;
using DC.Web.Authorization.Requirements;
using Microsoft.Extensions.DependencyInjection;

namespace DC.Web.Ui.StartupConfiguration
{
    public static class AuthorisationStartup
    {
        public static void AddAndConfigureAuthorisation(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(FeatureNames.FileSubmission, policy => policy.Requirements.Add(new FileSubmissionPolicyRequirement()));
            });
        }
    }
}