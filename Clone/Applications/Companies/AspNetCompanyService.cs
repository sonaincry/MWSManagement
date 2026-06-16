using Indotalent.Data;
using Indotalent.Infrastructures.Repositories;
using Indotalent.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.Applications.Companies
{
    public class AspNetCompanyService : Repository<AspNetCompany>
    {
        public AspNetCompanyService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IAuditColumnTransformer auditColumnTransformer) :
                base(
                    context,
                    httpContextAccessor,
                    auditColumnTransformer)
        {
        }

        public async Task<AspNetCompany?> GetDefaultCompanyAsync()
        {
            return await _context.AspNetCompany.FirstOrDefaultAsync();
        }

    }
}
