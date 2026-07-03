using Indotalent.Applications.AX;
using Indotalent.Data;
using Microsoft.EntityFrameworkCore;
using MWSManagement.Models.Entities;

namespace Indotalent.Applications.JobSyncs
{
    public class JobSyncService : AxCrudService<JobSync>
    {
        public JobSyncService(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> IsCodeExistsAsync(string code, long currentRecId = 0)
        {
            return await _dbSet.AnyAsync(x => x.Code == code && x.RecId != currentRecId);
        }
    }
}