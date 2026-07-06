using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MWSManagement.Applications.JobLogs;
using MWSManagement.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MWSManagement.Pages.Syncs.JobSyncs
{
    public class JobLogListModel : PageModel
    {
        private readonly JobLogService _jobLogService;

        public JobLogListModel(JobLogService jobLogService)
        {
            _jobLogService = jobLogService;
        }

        public List<JobLog> FormattedLogs { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public long? JobId { get; set; }
        public async Task OnGetAsync()
        {
            var allLogs = await _jobLogService.GetAllAsync();


            if (JobId.HasValue && JobId.Value > 0)
            {
                FormattedLogs = allLogs.Where(x => x.JobRecId == JobId.Value)
                                       .OrderByDescending(x => x.RunDate)
                                       .ThenByDescending(x => x.RunTime)
                                       .ToList();
            }
            else
            {
                FormattedLogs = allLogs.OrderByDescending(x => x.RunDate)
                                       .ThenByDescending(x => x.RunTime)
                                       .ToList();
            }
        }
    }
   
}