using ClientSchemaHub.Models.DTO;
using DBUtilityHub.Data;
using DBUtilityHub.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SchemaCraftHub.Model.DTO;
using SchemaCraftHub.Service.IService;
using System.Net;
using System.Text;

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

        public async Task<Dictionary<string, List<TableDetailsDTO>>> GetClientSchema(ClientSchemaHub.Models.DTO.APIResponse tabledetails1, DBConnectionDTO connectionDTO)
        {
            try
            {
                if (tabledetails1.Result != null)
                {
                    try
                    {
                        var jsonString = JsonConvert.SerializeObject(tabledetails1.Result);
                        var tableDetailsDict = JsonConvert.DeserializeObject<Dictionary<string, List<TableDetailsDTO>>>(jsonString);

                        foreach (var tabledetailsDTO in tableDetailsDict)
                        {
                            foreach (var table in tabledetailsDTO.Value)
                            {
                                var tablename = new TableMetaDataDTO
                                {
                                    EntityName = table.TableName,
                                    HostName = connectionDTO.HostName,
                                    DatabaseName = connectionDTO.DataBase,
                                    Provider = connectionDTO.Provider
                                };

                                await CreateTableAsync(tablename);
                            }
                        }

                        foreach (var tabledetailsDTO in tableDetailsDict)
                        {
                            foreach (var table in tabledetailsDTO.Value)
                            {
                                var tasks = table.Columns.Select(async columnDTO => new ColumnMetaDataDTO
                                {
                                    ColumnName = columnDTO.ColumnName,
                                    Datatype = columnDTO.DataType,
                                    IsPrimaryKey = columnDTO.IsPrimaryKey,
                                    IsForeignKey = columnDTO.HasForeignKey,
                                    EntityId = (await GetTableByHostProviderDatabaseTableNameAsync(connectionDTO.HostName, connectionDTO.Provider, connectionDTO.DataBase, table.TableName)).Id,
                                    ReferenceEntityID = (await GetTableByHostProviderDatabaseTableNameAsync(connectionDTO.HostName, connectionDTO.Provider, connectionDTO.DataBase, columnDTO.ReferencedTable)).Id,
                                    // Map other properties as needed
                                }).ToList();

                                var columnEntities = await Task.WhenAll(tasks);

                                await InsertColumnsAsync(columnEntities.ToList());
                            }
                        }

                        return tableDetailsDict;
                    }
                    catch (JsonException)
                    {
                        // Handle the case where JSON deserialization fails
                        throw new ApplicationException("Failed to deserialize Result into the expected format");
                    }
                }
                else
                {
                    // Handle the case where Result is null
                    throw new ApplicationException("Result is null");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }


        public async Task<int> CreateTableAsync(TableMetaDataDTO tableDTO)
        {
            try
            {
               var tableexists = await GetTableByHostProviderDatabaseTableNameAsync(tableDTO.HostName, tableDTO.Provider, tableDTO.DatabaseName, tableDTO.EntityName);
               if(tableexists == null)
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
                return tableexists.Id;
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

        public async Task<ClientSchemaHub.Models.DTO.APIResponse> convertandcallcreatetablemodel(DBConnectionDTO connectionDTO, TableRequest tableRequest)
        {
            TableDetailsDTO maptable = new TableDetailsDTO
            {
                TableName = tableRequest.Table.EntityName,
                Columns = await MapColumns(tableRequest.Columns)
            };

            var createquery = GenerateCreateTableSql(maptable);

            string otherApiBaseUrl = "https://localhost:7246/EntityMigrate";

            // Create HttpClient instance
            using (var httpClient = new HttpClient())
            {
                // Set the base address of the other API
                httpClient.BaseAddress = new Uri(otherApiBaseUrl);

                // Call the other API to get table details
                var response = await httpClient.GetAsync($"CreateTable?Provider={connectionDTO.Provider}&HostName={connectionDTO.HostName}&DataBase={connectionDTO.DataBase}&UserName={connectionDTO.UserName}&Password={connectionDTO.Password}");

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Parse the response content as needed (assuming it's JSON)
                    var tableDetails = JsonConvert.DeserializeObject<ClientSchemaHub.Models.DTO.APIResponse>(responseContent);

                    // Continue with your logic...

                    return tableDetails;
                }
                else
                {
                    // Handle unsuccessful response
                    var responseModel = new ClientSchemaHub.Models.DTO.APIResponse
                    {
                        StatusCode = response.StatusCode,
                        IsSuccess = false,
                        Result = null // You might want to include more details here
                    };

                    return responseModel;
                }
            }

        }

        //private List<ColumnDetailsDTO> MapColumns(List<ColumnMetaDataDTO> columns)
        //{
        //    // Map only the required properties from ColumnMetaDataDTO to ColumnDetailsDTO
        //    return columns.Select(column => new ColumnDetailsDTO
        //    {
        //        ColumnName = column.ColumnName,
        //        DataType = column.Datatype,
        //        IsPrimaryKey = column.IsPrimaryKey,
        //        HasForeignKey = column.IsForeignKey,
        //        ReferencedTable = GetTableByIdAsync(column.ReferenceEntityID ?? 0).Result.EntityName,
        //        ReferencedColumn = GetColumnByIdAsync(column.ReferenceColumnID ?? 0).Result.ColumnName
        //        // Add other properties as needed
        //    }).ToList();
        //}

        private async Task<List<ColumnDetailsDTO>> MapColumns(List<ColumnMetaDataDTO> columns)
        {
            var columnDetailsList = new List<ColumnDetailsDTO>();

            foreach (var column in columns)
            {
                var referencedTable = await GetTableByIdAsync(column.ReferenceEntityID ?? 0);
                var referencedColumn = await GetColumnByIdAsync(column.ReferenceColumnID ?? 0);

                var columnDetails = new ColumnDetailsDTO
                {
                    ColumnName = column.ColumnName,
                    DataType = column.Datatype,
                    IsPrimaryKey = column.IsPrimaryKey,
                    HasForeignKey = column.IsForeignKey,
                    ReferencedTable = referencedTable?.EntityName,
                    ReferencedColumn = referencedColumn?.ColumnName
                    // Add other properties as needed
                };

                columnDetailsList.Add(columnDetails);
            }

            return columnDetailsList;
        }





        private string GenerateCreateTableSql(TableDetailsDTO mapTable)
        {
            StringBuilder sqlBuilder = new StringBuilder();

            // Basic SQL statement to create a table
            sqlBuilder.Append($"CREATE TABLE {mapTable.TableName} (");

            // Generate columns
            foreach (var column in mapTable.Columns)
            {
                sqlBuilder.Append($"{column.ColumnName} {column.DataType}");

                if (column.IsPrimaryKey)
                {
                    sqlBuilder.Append(" PRIMARY KEY");
                }

                if (column.HasForeignKey)
                {
                    // Assuming that ReferencedTable and ReferencedColumn are foreign key references
                    sqlBuilder.Append($" REFERENCES {column.ReferencedTable}({column.ReferencedColumn})");
                }

                // Add other column properties as needed
                sqlBuilder.Append(",");
            }

            // Remove the trailing comma
            sqlBuilder.Length--;

            // Close the CREATE TABLE statement
            sqlBuilder.Append(");");

            return sqlBuilder.ToString();
        }
    }
}
