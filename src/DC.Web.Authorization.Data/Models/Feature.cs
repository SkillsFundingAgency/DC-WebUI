using System.ComponentModel.DataAnnotations.Schema;

namespace DC.Web.Authorization.Data.Models
{
    public class Feature
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FeatureId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}