using Indotalent.Data;
using Indotalent.DTOs;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Indotalent.AppSettings;
using Indotalent.Applications.AX;
using MWSManagement.Models.Entities; 

namespace MWSManagement.Applications.Locations
{
    public class LocationService : AxCrudService<Location>
    {
        public LocationService(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> IsNameExistsAsync(string name, long currentRecId)
        {
            return await _context.Set<Location>()
                .AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.RecId != currentRecId);
        }

    }
}