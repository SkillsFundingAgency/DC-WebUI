using DC.Web.Ui.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using DC.Web.Ui.Settings.Models;
using Microsoft.Extensions.Configuration;

namespace DC.Web.Ui.StartupConfiguration
{
    public static class AuthenticationStartup
    {
        public static void AddAndConfigureAuthentication(this IServiceCollection services, AuthenticationSettings authSettings)
        {

            services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignOutScheme = WsFederationDefaults.AuthenticationScheme;
                })
                .AddWsFederation(options =>
                {
                    options.Wtrealm = authSettings.WtRealm;
                    options.MetadataAddress = authSettings.MetadataAddress;

                    options.Events.OnSecurityTokenValidated = OnTokenValidated;
                    options.CallbackPath = "/Account/PostSignIn";
                })
                .AddCookie(options => { options.ReturnUrlParameter = "/Account/PostSignIn"; });
        }

        private static Task OnTokenValidated(SecurityTokenValidatedContext context)
        {
            return Task.CompletedTask;
        }
    }
}