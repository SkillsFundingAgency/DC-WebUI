//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using DC.Web.Authorization.AuthorizationHandlers;
//using DC.Web.Authorization.Base;
//using DC.Web.Authorization.PlicyRequirements;
//using DC.Web.Ui.Settings.Models;
//using Microsoft.AspNetCore.Authorization;

//namespace DC.Web.Authorization.Tests.HelperClasses
//{
//    public class AuthorizationAuthorizationPolicyHandlerMock : FileSubmissionPolicyHandler
//    {
//        public AuthorizationAuthorizationPolicyHandlerMock(IAuthorizationPolicyService authorizationPolicyService, AuthenticationSettings authenticationSettings)
//            : base(authorizationPolicyService, authenticationSettings)
//        {
//        }

//        public Task HandleAsyncTest(AuthorizationHandlerContext context, FileSubmissionPolicyRequirement requirement)
//        {
//            return HandleAsync(context, requirement);
//        }
//    }
//}
