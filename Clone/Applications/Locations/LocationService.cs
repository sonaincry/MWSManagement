using Indotalent.Data;
using Indotalent.DTOs;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Indotalent.AppSettings; // THÊM NAMESPACE NÀY

namespace MWSManagement.Applications.Locations
{
    public class LocationService
    {
        private readonly ApplicationDbContext _context;

        public LocationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<LocationDto>> GetAllLocationsAsync()
        {
            var list = new List<LocationDto>();

            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "SELECT Id, Name, Server, DatabaseName, Username, Password, IsWindowsAuth FROM [dbo].[SyncLocations] ORDER BY Id DESC";
            command.CommandType = CommandType.Text;

            if (command.Connection!.State != ConnectionState.Open)
                await command.Connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new LocationDto
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Server = reader.GetString(2),
                    DatabaseName = reader.GetString(3),
                    Username = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Password = reader.IsDBNull(5) ? null : reader.GetString(5),
                    IsWindowsAuth = reader.GetBoolean(6)
                });
            }
            return list;
        }

        public async Task<LocationDto?> GetLocationByIdAsync(int id)
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "SELECT Id, Name, Server, DatabaseName, Username, Password, IsWindowsAuth FROM [dbo].[SyncLocations] WHERE Id = @Id";
            command.Parameters.Add(new SqlParameter("@Id", id));
            command.CommandType = CommandType.Text;

            if (command.Connection!.State != ConnectionState.Open)
                await command.Connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new LocationDto
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Server = reader.GetString(2),
                    DatabaseName = reader.GetString(3),
                    Username = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Password = reader.IsDBNull(5) ? null : reader.GetString(5),
                    IsWindowsAuth = reader.GetBoolean(6)
                };
            }
            return null;
        }

        public async Task InsertLocationAsync(LocationDto loc)
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = @"INSERT INTO [dbo].[SyncLocations] (Name, Server, DatabaseName, Username, Password, IsWindowsAuth) 
                                    VALUES (@Name, @Server, @DatabaseName, @Username, @Password, @IsWindowsAuth)";

            command.Parameters.Add(new SqlParameter("@Name", loc.Name));
            command.Parameters.Add(new SqlParameter("@Server", loc.Server));
            command.Parameters.Add(new SqlParameter("@DatabaseName", loc.DatabaseName));
            command.Parameters.Add(new SqlParameter("@Username", (object?)loc.Username ?? DBNull.Value));
            command.Parameters.Add(new SqlParameter("@Password", (object?)loc.Password ?? DBNull.Value));
            command.Parameters.Add(new SqlParameter("@IsWindowsAuth", loc.IsWindowsAuth));

            if (command.Connection!.State != ConnectionState.Open)
                await command.Connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateLocationAsync(LocationDto loc)
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = @"UPDATE [dbo].[SyncLocations] 
                                    SET Name = @Name, Server = @Server, DatabaseName = @DatabaseName, 
                                        Username = @Username, Password = @Password, IsWindowsAuth = @IsWindowsAuth 
                                    WHERE Id = @Id";

            command.Parameters.Add(new SqlParameter("@Id", loc.Id));
            command.Parameters.Add(new SqlParameter("@Name", loc.Name));
            command.Parameters.Add(new SqlParameter("@Server", loc.Server));
            command.Parameters.Add(new SqlParameter("@DatabaseName", loc.DatabaseName));
            command.Parameters.Add(new SqlParameter("@Username", (object?)loc.Username ?? DBNull.Value));
            command.Parameters.Add(new SqlParameter("@Password", (object?)loc.Password ?? DBNull.Value));
            command.Parameters.Add(new SqlParameter("@IsWindowsAuth", loc.IsWindowsAuth));

            if (command.Connection!.State != ConnectionState.Open)
                await command.Connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteLocationAsync(int id)
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "DELETE FROM [dbo].[SyncLocations] WHERE Id = @Id";
            command.Parameters.Add(new SqlParameter("@Id", id));

            if (command.Connection!.State != ConnectionState.Open)
                await command.Connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }
    }
}