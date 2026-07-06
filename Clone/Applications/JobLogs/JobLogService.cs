using Indotalent.Applications.AX;
using Indotalent.Data;
using Microsoft.EntityFrameworkCore;
using MWSManagement.Models;
using System.Text.RegularExpressions;

namespace MWSManagement.Applications.JobLogs
{
    public class JobLogService : AxCrudService<JobLog>
    {
        public JobLogService(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<JobLog>> GetLogsByJobIdAsync(long jobRecId)
        {
            return await _context.Set<JobLog>()
                .AsNoTracking()
                .Where(x => x.JobRecId == jobRecId)
                .OrderByDescending(x => x.RunDate)
                .ThenByDescending(x => x.RunTime)
                .ToListAsync();
        }

        public static string ExtractCleanLog(string htmlDescription)
        {
            if (string.IsNullOrEmpty(htmlDescription)) return string.Empty;

            var match = Regex.Match(htmlDescription, @"(?:<span[^>]*>\[OK\]<\/span>\s*)([^<]+)");

            if (match.Success)
            {

                return $"[OK] {match.Groups[1].Value.Trim()}";
            }

            return Regex.Replace(htmlDescription, "<.*?>", string.Empty).Trim();
        }
    }
}