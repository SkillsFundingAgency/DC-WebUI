namespace DC.Web.Authorization.Data.Query
{
    public interface IPermissionsQueryService
    {
        bool HasPermission(string role, string permission);
    }
}
