using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DC.Web.Authorization.Data.Constants;
using DC.Web.Authorization.Data.Models;

namespace DC.Web.Authorization.Data.SeedData
{
    public class AuthorizationDataSeeder
    {
        public static void Initialize(AuthorizeDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Roles.Any())
            {
                return;   // DB has been seeded
            }

            var roles = new Role[]
            {
                new Role {RoleId = 1, Name = RoleNames.DAA,Description = "Data Collections Admin"},
                new Role {RoleId = 2, Name = RoleNames.DCS,Description = "Data Collections Support"},
                new Role {RoleId = 3, Name = RoleNames.BI,Description = "Reports and BI"},
                new Role {RoleId = 4, Name = RoleNames.DCI,Description = "Data Collections Information Officer"},
            };
            foreach (var role in roles)
            {
                context.Roles.Add(role);
            }
            context.SaveChanges();

            var permissions = new Permission[]
            {
                new Permission {PermissionId = 1, Name = PermissionNames.SubmissionAllowed ,Description = "File Submission Allowed"},
                new Permission {PermissionId = 2, Name = PermissionNames.ReportViewing ,Description = "Reports Viewer"}
            };
            foreach (var p in permissions)
            {
                context.Permissions.Add(p);
            }
            context.SaveChanges();

            var rolePermissions = new RolePermission[]
            {
                new RolePermission {PermissionId = 1, RoleId = 1},
                new RolePermission {PermissionId = 2, RoleId = 1},
            };
            foreach (var rp in rolePermissions)
            {
                context.RolePermissions.Add(rp);
            }
            context.SaveChanges();
         
        }
    }
}
