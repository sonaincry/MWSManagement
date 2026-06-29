using Indotalent.Data;
using MWSManagement.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MWSManagement.Applications.TableSyncs
{
    public class TableSyncService
    {
        private readonly ApplicationDbContext _context;

        public TableSyncService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TableSyncDto>> GetConfiguredTablesAsync()
        {
            var list = new List<TableSyncDto>();

            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "SELECT Id, TableName, SyncAction, IsActive FROM [dbo].[TableSyncConfigs] ORDER BY Id DESC";
            command.CommandType = CommandType.Text;

            if (command.Connection!.State != ConnectionState.Open)
                await command.Connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new TableSyncDto
                {
                    Id = reader.GetInt32(0),
                    TableName = reader.GetString(1),
                    SyncAction = reader.GetInt32(2),
                    IsActive = reader.GetBoolean(3)
                });
            }
            return list;
        }

        public async Task<List<SelectListItem>> GetDatabaseTablesOptionsAsync()
        {
            var options = new List<SelectListItem>();

            string queryTables = @"
                SELECT TABLE_SCHEMA + '.' + TABLE_NAME AS FullTableName 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_TYPE = 'BASE TABLE' 
                  AND TABLE_NAME NOT LIKE 'sys%' 
                  AND TABLE_NAME NOT LIKE '%_tracking'
                  AND TABLE_NAME NOT LIKE 'scope_info%'
                ORDER BY TABLE_SCHEMA, TABLE_NAME";

            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = queryTables;
            command.CommandType = CommandType.Text;

            if (command.Connection!.State != ConnectionState.Open)
                await command.Connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var tableName = reader.GetString(0);
                options.Add(new SelectListItem { Value = tableName, Text = tableName });
            }

            return options;
        }

        public async Task SaveTableSyncConfigAsync(TableSyncDto input)
        {
            string insertQuery = @"
                IF NOT EXISTS (SELECT 1 FROM TableSyncConfigs WHERE TableName = @TableName)
                BEGIN
                    INSERT INTO TableSyncConfigs (TableName, SyncAction, IsActive) 
                    VALUES (@TableName, @SyncAction, 1)
                END
                ELSE
                BEGIN
                    UPDATE TableSyncConfigs 
                    SET SyncAction = @SyncAction, IsActive = 1 
                    WHERE TableName = @TableName
                END";

            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = insertQuery;
            command.CommandType = CommandType.Text;

            var pTableName = command.CreateParameter();
            pTableName.ParameterName = "@TableName";
            pTableName.Value = input.TableName;
            command.Parameters.Add(pTableName);

            var pSyncAction = command.CreateParameter();
            pSyncAction.ParameterName = "@SyncAction";
            pSyncAction.Value = input.SyncAction;
            command.Parameters.Add(pSyncAction);

            if (command.Connection!.State != ConnectionState.Open)
                await command.Connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }
    }
}