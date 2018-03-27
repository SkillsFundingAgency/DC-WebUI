using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DC.Web.Ui.ClaimTypes;
using DC.Web.Ui.Services.Models;

namespace DC.Web.Ui.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string Ukprn(this ClaimsPrincipal claimsPrincipal)
        {
            return GetClaimValue(claimsPrincipal, IdamsClaimTypes.Ukprn);
        }

        public static string DisplayName(this ClaimsPrincipal claimsPrincipal)
        {
            return GetClaimValue(claimsPrincipal, IdamsClaimTypes.DisplayName);
        }

        public static string Name(this ClaimsPrincipal claimsPrincipal)
        {
            return GetClaimValue(claimsPrincipal,IdamsClaimTypes.Name);
        }

        private static string GetClaimValue(ClaimsPrincipal claimsPrincipal, string claimType)
        {
            return claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == claimType)?.Value;
        }
    }
}
