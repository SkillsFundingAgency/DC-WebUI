using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DC.Web.Authorization;
using DC.Web.Authorization.Idams;

namespace DC.Web.Ui.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static long Ukprn(this ClaimsPrincipal claimsPrincipal)
        {
            long.TryParse(GetClaimValue(claimsPrincipal, IdamsClaimTypes.Ukprn), out var result);
            return result;
        }

        public static long Upin(this ClaimsPrincipal claimsPrincipal)
        {
            long.TryParse(GetClaimValue(claimsPrincipal, IdamsClaimTypes.Upin), out var result);
            return result;
        }

        public static string DisplayName(this ClaimsPrincipal claimsPrincipal)
        {
            return GetClaimValue(claimsPrincipal, IdamsClaimTypes.DisplayName);
        }

        public static string Name(this ClaimsPrincipal claimsPrincipal)
        {
            return GetClaimValue(claimsPrincipal, IdamsClaimTypes.Name);
        }

        public static string NameIdentifier(this ClaimsPrincipal claimsPrincipal)
        {
            return GetClaimValue(claimsPrincipal, IdamsClaimTypes.NameIdentifier);
        }

        public static string Email(this ClaimsPrincipal claimsPrincipal)
        {
            return GetClaimValue(claimsPrincipal, IdamsClaimTypes.Email);
        }

        public static bool IsAdminUser(this ClaimsPrincipal claimsPrincipal)
        {
            var claimValue = GetClaimValue(claimsPrincipal, IdamsClaimTypes.UserType);
            if (string.IsNullOrEmpty(claimValue))
            {
                return false;
            }

            return ClaimAccessConstants.HelpDeskUserTypes.Contains(claimValue.ToUpper());
        }

        private static string GetClaimValue(ClaimsPrincipal claimsPrincipal, string claimType)
        {
            return claimsPrincipal?.Claims?.FirstOrDefault(claim => claim.Type == claimType)?.Value;
        }
    }
}
