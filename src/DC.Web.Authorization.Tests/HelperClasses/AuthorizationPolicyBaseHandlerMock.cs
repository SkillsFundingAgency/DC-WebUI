//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using DC.Web.Authorization.AuthorizationHandlers;
//using DC.Web.Ui.Settings.Models;
//using Microsoft.AspNetCore.Authorization;

//namespace DC.Web.Authorization.Tests.HelperClasses
//{
//    public class AuthorizationPolicyBaseHandlerMock : AuthorizationPolicyHandlerBase<IAuthorizationRequirement>
//    {
//        public AuthorizationPolicyBaseHandlerMock(AuthenticationSettings authenticationSettings)
//            : base(authenticationSettings)
//        {
//        }

//        public Task HandleRequirementAsyncTest(AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
//        {
//            return HandleRequirementAsync(context, requirement);
//        }

//        protected override Task HandleAsync(AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
