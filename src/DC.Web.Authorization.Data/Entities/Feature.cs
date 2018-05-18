using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DC.Web.Authorization.Data.Entities
{
    public class Feature : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}