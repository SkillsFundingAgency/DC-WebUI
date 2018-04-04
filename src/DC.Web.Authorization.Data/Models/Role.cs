using System.ComponentModel.DataAnnotations.Schema;

namespace DC.Web.Authorization.Data.Models
{
    public class Role
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RoleId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}