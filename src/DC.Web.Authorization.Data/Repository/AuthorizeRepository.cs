using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DC.Web.Authorization.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DC.Web.Authorization.Data.Repository
{
    public class AuthorizeRepository : IAuthorizeRepository
    {
        private readonly AuthorizeDbContext _context;

        public AuthorizeRepository(AuthorizeDbContext context)
        {
            _context = context;
        }

        public IEnumerable<RoleFeature> GetAllRoleFeatures()
        {
            return _context.RoleFeatures
                .Include(x => x.Feature)
                .Include(x => x.Role)
                .AsEnumerable();
        }
    }
}
