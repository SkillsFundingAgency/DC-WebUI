using System.ComponentModel.DataAnnotations.Schema;

namespace DC.Web.Authorization.Data.Models
{
    public class Permission
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PermissionId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}