using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DC.Web.Ui.Tests
{
    public class AuthorizationServiceWrapper : IAuthorizationService
    {
        private readonly bool _result;

        public AuthorizationServiceWrapper(bool result)
        {
            _result = result;
        }

        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, IEnumerable<IAuthorizationRequirement> requirements)
        {
            throw new NotImplementedException();
        }

        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, string policyName)
        {
            return _result ? Task.FromResult(AuthorizationResult.Success()) : Task.FromResult(AuthorizationResult.Failed());
        }
    }
}