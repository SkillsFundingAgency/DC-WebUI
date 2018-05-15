using System.ComponentModel.DataAnnotations.Schema;

namespace DC.Web.Authorization.Data.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}