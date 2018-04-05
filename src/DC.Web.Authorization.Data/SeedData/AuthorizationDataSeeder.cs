using System.Linq;
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
                new Role { RoleId = 1, Name = RoleNames.DAA, Description = "Data Collections Admin" },
                new Role { RoleId = 2, Name = RoleNames.DCS, Description = "Data Collections Support" },
                new Role { RoleId = 3, Name = RoleNames.BI, Description = "Reports and BI" },
                new Role { RoleId = 4, Name = RoleNames.DCI, Description = "Data Collections Information Officer" },
            };
            foreach (var role in roles)
            {
                context.Roles.Add(role);
            }

            context.SaveChanges();

            var features = new Feature[]
            {
                new Feature() { FeatureId = 1, Name = FeatureNames.FileSubmission, Description = "File Submission" },
                new Feature() { FeatureId = 2, Name = FeatureNames.ReportViewing, Description = "Reports Viewer" }
            };
            foreach (var p in features)
            {
                context.Features.Add(p);
            }

            context.SaveChanges();

            var roleFeatures = new RoleFeature[]
            {
                new RoleFeature { FeatureId = 1, RoleId = 1 },
                new RoleFeature { FeatureId = 2, RoleId = 1 },
            };
            foreach (var rp in roleFeatures)
            {
                context.RoleFeatures.Add(rp);
            }

            context.SaveChanges();
        }
    }
}