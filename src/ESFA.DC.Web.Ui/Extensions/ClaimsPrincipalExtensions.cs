using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DC.Web.Authorization.Idams;
using DC.Web.Ui.Services.Models;

namespace DC.Web.Ui.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static long Ukprn(this ClaimsPrincipal claimsPrincipal)
        {
            long.TryParse(GetClaimValue(claimsPrincipal, IdamsClaimTypes.Ukprn), out var result);
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

        private static string GetClaimValue(ClaimsPrincipal claimsPrincipal, string claimType)
        {
            return claimsPrincipal?.Claims?.FirstOrDefault(claim => claim.Type == claimType)?.Value;
        }
    }
}
