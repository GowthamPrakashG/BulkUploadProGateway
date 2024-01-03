using DBUtilityHub.Data;
using DBUtilityHub.Models;
using Microsoft.EntityFrameworkCore;
using SchemaCraftHub.Model.DTO;
using SchemaCraftHub.Service.IService;

namespace SchemaCraftHub.Service
{
    public class EntitySchemaService : IEntitySchemaService
    {
        private readonly ApplicationDbContext _context;

        public EntitySchemaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TableMetaDataDTO>> GetAllTablesAsync()
        {
            try
            {
                var tables = await _context.TableMetaDataEntity.ToListAsync();

                var tableDTOs = tables.Select(table => new TableMetaDataDTO
                {
                    Id = table.Id,
                    EntityName = table.EntityName,
                    HostName = table.HostName,
                    DatabaseName = table.DatabaseName,
                    Provider = table.Provider
                    // Map other properties as needed
                }).ToList();

                return tableDTOs;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching all tables.", ex);
            }
        }

        public async Task<TableMetaDataDTO> GetTableByIdAsync(int id)
        {
            try
            {
                var table = await _context.TableMetaDataEntity
                    .Where(t => t.Id == id)
                    .Select(table => new TableMetaDataDTO
                    {
                        Id = table.Id,
                        EntityName = table.EntityName,
                        HostName = table.HostName,
                        DatabaseName = table.DatabaseName,
                        Provider = table.Provider
                        // Map other properties as needed
                    })
                    .FirstOrDefaultAsync();

                return table;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching all tables.", ex);
            }
        }

        public async Task<List<TableMetaDataDTO>> GetTablesByHostProviderDatabaseAsync(string hostName, string provider, string databaseName)
        {
            try
            {
                var tables = await _context.TableMetaDataEntity
                    .Where(table => table.HostName.ToLower() == hostName.ToLower() && table.Provider.ToLower() == provider.ToLower() && table.DatabaseName.ToLower() == databaseName.ToLower())
                    .ToListAsync();

                var tableDTOs = tables.Select(table => new TableMetaDataDTO
                {
                    Id = table.Id,
                    EntityName = table.EntityName,
                    HostName = table.HostName,
                    DatabaseName = table.DatabaseName,
                    Provider = table.Provider
                    // Map other properties as needed
                }).ToList();

                return tableDTOs;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching all tables.", ex);
            }

        }

        public async Task<TableMetaDataDTO> GetTableByHostProviderDatabaseTableNameAsync(string hostName, string provider, string databaseName, string tableName)
        {
            try
            {
                var table = await _context.TableMetaDataEntity
                    .FirstOrDefaultAsync(t => t.HostName.ToLower() == hostName.ToLower() && t.Provider.ToLower() == provider.ToLower() && t.DatabaseName.ToLower() == databaseName.ToLower() && t.EntityName.ToLower() == tableName.ToLower());

                if (table == null)
                {
                    // Handle the case where the table with the given parameters is not found
                    return null;
                }

                var tableDTO = new TableMetaDataDTO
                {
                    Id = table.Id,
                    EntityName = table.EntityName,
                    HostName = table.HostName,
                    DatabaseName = table.DatabaseName,
                    Provider = table.Provider
                    // Map other properties as needed
                };

                return tableDTO;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching all tables.", ex);
            }
        }

        public async Task<List<ColumnMetaDataDTO>> GetAllColumnsAsync()
        {
            try
            {
                var columns = await _context.ColumnMetaDataEntity.ToListAsync();

                var columnDTOs = columns.Select(column => new ColumnMetaDataDTO
                {
                    Id = column.Id,
                    ColumnName = column.ColumnName,
                    Datatype = column.Datatype,
                    IsPrimaryKey = column.IsPrimaryKey,
                    IsForeignKey = column.IsForeignKey,
                    EntityId = column.EntityId,
                    ReferenceEntityID = column.ReferenceEntityID,
                    ReferenceColumnID = column.ReferenceColumnID,
                    Length = column.Length,
                    MinLength = column.MinLength,
                    MaxLength = column.MaxLength,
                    MaxRange = column.MaxRange,
                    MinRange = column.MinRange,
                    DateMinValue = column.DateMinValue,
                    DateMaxValue = column.DateMaxValue,
                    Description = column.Description,
                    IsNullable = column.IsNullable,
                    DefaultValue = column.DefaultValue,
                    True = column.True,
                    False = column.False,
                    // Include other properties as needed
                }).ToList();


                return columnDTOs;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching all tables.", ex);
            }
        }

        public async Task<ColumnMetaDataDTO> GetColumnByIdAsync(int id)
        {
            try
            {
                var column = await _context.ColumnMetaDataEntity.FindAsync(id);

                if (column == null)
                {
                    // Handle the case where the column with the given ID is not found
                    return null;
                }

                var columnDTO = new ColumnMetaDataDTO
                {
                    Id = column.Id,
                    ColumnName = column.ColumnName,
                    Datatype = column.Datatype,
                    IsPrimaryKey = column.IsPrimaryKey,
                    IsForeignKey = column.IsForeignKey,
                    EntityId = column.EntityId,
                    ReferenceEntityID = column.ReferenceEntityID,
                    ReferenceColumnID = column.ReferenceColumnID,
                    Length = column.Length,
                    MinLength = column.MinLength,
                    MaxLength = column.MaxLength,
                    MaxRange = column.MaxRange,
                    MinRange = column.MinRange,
                    DateMinValue = column.DateMinValue,
                    DateMaxValue = column.DateMaxValue,
                    Description = column.Description,
                    IsNullable = column.IsNullable,
                    DefaultValue = column.DefaultValue,
                    True = column.True,
                    False = column.False,
                };

                return columnDTO;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching all tables.", ex);
            }
        }

        public async Task<ColumnMetaDataDTO> GetColumnByIdAndEntityIDAsync(int id, int entityId)
        {
            try
            {
                var column = await _context.ColumnMetaDataEntity.FirstOrDefaultAsync(x => x.Id == id && x.EntityId == entityId);

                if (column == null)
                {
                    // Handle the case where the column with the given ID is not found
                    return null;
                }

                var columnDTO = new ColumnMetaDataDTO
                {
                    Id = column.Id,
                    ColumnName = column.ColumnName,
                    Datatype = column.Datatype,
                    IsPrimaryKey = column.IsPrimaryKey,
                    IsForeignKey = column.IsForeignKey,
                    EntityId = column.EntityId,
                    ReferenceEntityID = column.ReferenceEntityID,
                    ReferenceColumnID = column.ReferenceColumnID,
                    Length = column.Length,
                    MinLength = column.MinLength,
                    MaxLength = column.MaxLength,
                    MaxRange = column.MaxRange,
                    MinRange = column.MinRange,
                    DateMinValue = column.DateMinValue,
                    DateMaxValue = column.DateMaxValue,
                    Description = column.Description,
                    IsNullable = column.IsNullable,
                    DefaultValue = column.DefaultValue,
                    True = column.True,
                    False = column.False,
                };

                return columnDTO;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching all tables.", ex);
            }
        }

        public async Task<List<ColumnMetaDataDTO>> GetColumnsByEntityIdAsync(int entityId)
        {
            try
            {
                var columns = await _context.ColumnMetaDataEntity
                    .Where(column => column.EntityId == entityId)
                    .ToListAsync();

                var columnDTOs = columns.Select(column => new ColumnMetaDataDTO
                {
                    Id = column.Id,
                    ColumnName = column.ColumnName,
                    Datatype = column.Datatype,
                    IsPrimaryKey = column.IsPrimaryKey,
                    IsForeignKey = column.IsForeignKey,
                    EntityId = column.EntityId,
                    ReferenceEntityID = column.ReferenceEntityID,
                    ReferenceColumnID = column.ReferenceColumnID,
                    Length = column.Length,
                    MinLength = column.MinLength,
                    MaxLength = column.MaxLength,
                    MaxRange = column.MaxRange,
                    MinRange = column.MinRange,
                    DateMinValue = column.DateMinValue,
                    DateMaxValue = column.DateMaxValue,
                    Description = column.Description,
                    IsNullable = column.IsNullable,
                    DefaultValue = column.DefaultValue,
                    True = column.True,
                    False = column.False,
                    // Include other properties as needed
                }).ToList();

                return columnDTOs;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching all tables.", ex);
            }
        }

        //public async Task<Dictionary<string, List<TableDetailsDTO>>> GetClientSchema(DBConnectionDTO connectionDTO)
        //{
        //    try
        //    {
        //        var tabledetails = await _generalDatabaseService.GetTableDetailsForAllTablesAsync(connectionDTO);

        //        foreach (var tabledetailsDTO in tabledetails)
        //        {
        //            foreach(var table in tabledetailsDTO.Value)
        //            {
        //                var tablename = new TableMetaDataDTO
        //                {
        //                    EntityName = table.TableName,
        //                    HostName = connectionDTO.HostName,
        //                    DatabaseName = connectionDTO.DataBase,
        //                    Provider = connectionDTO.Provider
        //                };

        //               await CreateTableAsync(tablename);
        //            }
        //        }

        //        foreach (var tabledetailsDTO in tabledetails)
        //        {
        //            foreach (var table in tabledetailsDTO.Value)
        //            {
        //                var tasks = table.Columns.Select(async columnDTO => new ColumnMetaDataDTO
        //                {
        //                    ColumnName = columnDTO.ColumnName,
        //                    Datatype = columnDTO.DataType,
        //                    IsPrimaryKey = columnDTO.IsPrimaryKey,
        //                    IsForeignKey = columnDTO.HasForeignKey,
        //                    EntityId = GetTableByHostProviderDatabaseTableNameAsync(connectionDTO.HostName, connectionDTO.Provider, connectionDTO.DataBase, table.TableName).Result.Id,
        //                    ReferenceEntityID = GetTableByHostProviderDatabaseTableNameAsync(connectionDTO.HostName, connectionDTO.Provider, connectionDTO.DataBase, columnDTO.ReferencedTable).Result.Id,
        //                    // Map other properties as needed
        //                }).ToList();

        //                var columnEntities = await Task.WhenAll(tasks);

        //                await InsertColumnsAsync(columnEntities.ToList());
        //            }
        //        }

        //        return tabledetails;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException(ex.Message, ex);
        //    }
        //}

        public async Task<int> CreateTableAsync(TableMetaDataDTO tableDTO)
        {
            try
            {
                var table = new TableMetaDataEntity
                {
                    EntityName = tableDTO.EntityName,
                    HostName = tableDTO.HostName,
                    DatabaseName = tableDTO.DatabaseName,
                    Provider = tableDTO.Provider
                    // Map other properties as needed
                };

                _context.TableMetaDataEntity.Add(table);
                await _context.SaveChangesAsync();
                return table.Id;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching all tables.", ex);
            }
        }

        public async Task InsertColumnsAsync(List<ColumnMetaDataDTO> columns)
        {
            try
            {
                var columnEntities = columns.Select(columnDTO => new ColumnMetaDataEntity
                {
                    Id = columnDTO.Id,
                    ColumnName = columnDTO.ColumnName,
                    Datatype = columnDTO.Datatype,
                    IsPrimaryKey = columnDTO.IsPrimaryKey,
                    IsForeignKey = columnDTO.IsForeignKey,
                    EntityId = columnDTO.EntityId,
                    ReferenceEntityID = columnDTO.ReferenceEntityID,
                    ReferenceColumnID = columnDTO.ReferenceColumnID,
                    Length = columnDTO.Length,
                    MinLength = columnDTO.MinLength,
                    MaxLength = columnDTO.MaxLength,
                    MaxRange = columnDTO.MaxRange,
                    MinRange = columnDTO.MinRange,
                    DateMinValue = columnDTO.DateMinValue,
                    DateMaxValue = columnDTO.DateMaxValue,
                    Description = columnDTO.Description,
                    IsNullable = columnDTO.IsNullable,
                    DefaultValue = columnDTO.DefaultValue,
                    True = columnDTO.True,
                    False = columnDTO.False
                    // Map other properties as needed
                }).ToList();

                _context.ColumnMetaDataEntity.AddRange(columnEntities);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching all tables.", ex);
            }
        }

    }
}
