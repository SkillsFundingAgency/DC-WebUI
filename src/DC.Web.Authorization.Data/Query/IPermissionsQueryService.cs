using System.Collections.Generic;

namespace DC.Web.Authorization.Data.Query
{
    public interface IPermissionsQueryService
    {
        bool HasPermission(string role, IEnumerable<string> permissions);
    }
}