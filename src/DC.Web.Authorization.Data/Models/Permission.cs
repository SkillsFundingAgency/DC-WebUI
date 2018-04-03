using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

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
