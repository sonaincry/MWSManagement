using Indotalent.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MWSManagement.Models.Entities;
using Indotalent.Applications.JobSyncs;
using MWSManagement.Applications.Locations;
using Indotalent.Applications.TableSyncs;
using System.Linq;
using System.Threading.Tasks;

namespace MWSManagement.Pages.Syncs.JobSyncs
{
    public class JobSyncFormModel : BaseAxCrudPageModel<JobSync>
    {
        private readonly JobSyncService _jobSyncService;
        private readonly LocationService _locationService;
        private readonly TableSyncService _tableConfigService;

        public JobSyncFormModel(
            JobSyncService service,
            LocationService locationService,
            TableSyncService tableConfigService) : base(service)
        {
            _jobSyncService = service;
            _locationService = locationService;
            _tableConfigService = tableConfigService;
        }

        public List<SelectListItem> AvailableTables { get; set; } = new();
        public List<SelectListItem> LocationOptions { get; set; } = new();

        protected override async Task LoadLookupsAsync()
        {
            // 1. Lấy danh sách Table từ TableSyncs
            var tableConfigs = await _tableConfigService.GetAllAsync();
            AvailableTables = tableConfigs
                .Select(t => t.TableName)
                .Distinct()
                .Select(t => new SelectListItem { Value = t, Text = t })
                .ToList();

            // 2. Lấy danh sách Location từ SyncLocations
            var locs = await _locationService.GetAllAsync();
            LocationOptions = locs.Select(l => new SelectListItem { Value = l.RecId.ToString(), Text = l.Name }).ToList();
        }

        protected override JobSync CreateDefaultEntity()
        {
            return new JobSync { Code = string.Empty };
        }

        protected override void ValidateForm()
        {
            if (ActionMode == "delete") return;

            if (string.IsNullOrWhiteSpace(Form.Code))
                ModelState.AddModelError("Form.Code", "Job Code is required.");

            if (_jobSyncService.IsCodeExistsAsync(Form.Code, Form.RecId).GetAwaiter().GetResult())
                ModelState.AddModelError("Form.Code", "Job Code already exists.");

            // Kiểm tra trực tiếp chuỗi text thuần được JS đẩy vào ẩn sau UI
            if (string.IsNullOrWhiteSpace(Form.SenderLocationIds))
                ModelState.AddModelError("Form.SenderLocationIds", "The Senders field is required.");

            if (string.IsNullOrWhiteSpace(Form.ReceiverLocationIds))
                ModelState.AddModelError("Form.ReceiverLocationIds", "The Receivers field is required.");

            if (string.IsNullOrWhiteSpace(Form.TableNames))
                ModelState.AddModelError("Form.TableNames", "The Tables field is required.");
        }

        protected override IActionResult RedirectToListPage() => RedirectToPage("/Syncs/JobSyncs/JobSyncList");
        protected override IActionResult RedirectToEditPage(long recId) => RedirectToPage("/Syncs/JobSyncs/JobSyncForm", new { recId, action = "edit" });
    }
}