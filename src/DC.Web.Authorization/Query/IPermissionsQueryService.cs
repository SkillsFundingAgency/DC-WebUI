using System.Collections.Generic;

namespace DC.Web.Authorization.Query
{
    public interface IPermissionsQueryService
    {
        bool HasPermission(string role, IEnumerable<string> features);
    }
}