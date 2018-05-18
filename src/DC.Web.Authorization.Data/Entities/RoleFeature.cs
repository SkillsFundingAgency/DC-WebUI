using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DC.Web.Authorization.Data.Entities
{
    public class RoleFeature : BaseEntity
    {
        public int RoleId { get; set; }

        public int FeatureId { get; set; }

        public virtual Role Role { get; set; }

        public virtual Feature Feature { get; set; }
    }
}