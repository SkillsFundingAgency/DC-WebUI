using System;
using System.Collections.Generic;
using System.Text;

namespace DC.Web.Authorization
{
    public static class ClaimAccessConstants
    {
        public static IEnumerable<string> HelpDeskUserTypes = new List<string> { "LSC" };
        public static IEnumerable<string> FileSubmissionRoles = new List<string> { "DCFT", "DAA", "DCS" };
    }
}
