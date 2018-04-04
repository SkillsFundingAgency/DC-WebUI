namespace DC.Web.Authorization.Data.Models
{
    public class RoleFeature
    {
        public int RoleId { get; set; }
        public int FeatureId { get; set; }
        public virtual Role Role { get; set; }
        public virtual Feature Feature { get; set; }
    }
}