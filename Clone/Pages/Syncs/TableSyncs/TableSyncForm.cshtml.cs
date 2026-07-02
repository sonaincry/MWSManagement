using Indotalent.Applications.TableSyncs;
using Indotalent.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using MWSManagement.Models.Entities;

namespace MWSManagement.Pages.Syncs.TableSyncs
{
    public class TableSyncFormModel : BaseAxCrudPageModel<TableSync>
    {
        private readonly TableSyncService _tableSyncService;

        public TableSyncFormModel(TableSyncService service) : base(service)
        {
            _tableSyncService = service;
        }


        public List<string> AvailableTables { get; set; } = new();

        protected override Task LoadLookupsAsync()
        {
            try
            {
                AvailableTables = _tableSyncService.GetSystemTablesAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                AvailableTables = new List<string> { "Error loading tables" };
                ModelState.AddModelError(string.Empty, "Không thể tải danh sách bảng: " + ex.Message);
            }

            return Task.CompletedTask;
        }

        protected override TableSync CreateDefaultEntity()
        {
            return new TableSync
            {
                Code = string.Empty,
                SyncAction = 1,
                IsActive = true
            };
        }

        protected override void ValidateForm()
        {
            if (ActionMode == "delete")
            {
                if (Form.RecId <= 0) ModelState.AddModelError("Form.RecId", "Invalid RecId.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Form.Code))
                ModelState.AddModelError("Form.Code", "Code is required.");

            if (string.IsNullOrWhiteSpace(Form.TableName))
                ModelState.AddModelError("Form.TableName", "Table Name is required.");

            if (_tableSyncService.IsCodeExistsAsync(Form.Code, Form.RecId).GetAwaiter().GetResult())
            {
                ModelState.AddModelError("Form.Code", "Code already exists");
            };
        }

        protected override IActionResult RedirectToListPage() => RedirectToPage("/Syncs/TableSyncs/TableSyncList");
        protected override IActionResult RedirectToEditPage(long recId) => RedirectToPage("/Syncs/TableSyncs/TableSyncForm", new { recId, action = "edit" });
    }
}