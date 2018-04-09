using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DC.Web.Authorization.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DC.Web.Authorization.Data.Repository
{
    public class AuthorizeRepository : IAuthorizeRepository, IDisposable
    {
        private readonly AuthorizeDbContext _context;
        private bool _disposed = false;

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }

            _disposed = true;
        }
    }
}
