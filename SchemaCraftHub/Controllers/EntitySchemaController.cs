using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchemaCraftHub.Model.DTO;
using SchemaCraftHub.Service;
using SchemaCraftHub.Service.IService;

namespace SchemaCraftHub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EntitySchemaController : ControllerBase
    {
        private readonly IEntitySchemaService _entitySchemaService;
        private readonly ILogService _logService; // Inject the LogService

        public EntitySchemaController(IEntitySchemaService entitySchemaService, ILogService logService)
        {
            _entitySchemaService = entitySchemaService;
            _logService = logService;
        }

        [HttpGet("tables")]
        public async Task<IActionResult> GetAllTables()
        {
            try
            {
                var tables = await _entitySchemaService.GetAllTablesAsync();

                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = tables
                };
                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message },
                    Result = null
                };
                return StatusCode((int)responseModel.StatusCode, responseModel);
            }
        }

        [HttpGet("tables/{id}")]
        public async Task<IActionResult> GetTableById(int id)
        {
            try
            {
                var table = await _entitySchemaService.GetTableByIdAsync(id);
                if (table == null)
                {
                    return NotFound();
                }

                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = table
                };
                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message },
                    Result = null
                };
                return StatusCode((int)responseModel.StatusCode, responseModel);
            }
        }

        [HttpGet("tables/{hostName}/{provider}/{databaseName}")]
        public async Task<IActionResult> GetTablesByHostProviderDatabase(string hostName, string provider, string databaseName)
        {
            try
            {
                var tables = await _entitySchemaService.GetTablesByHostProviderDatabaseAsync(hostName, provider, databaseName);

                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = tables
                };

                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message },
                    Result = null
                };

                return StatusCode((int)responseModel.StatusCode, responseModel);
            }
        }

        [HttpGet("tables/{hostName}/{provider}/{databaseName}/{tableName}")]
        public async Task<IActionResult> GetTableByHostProviderDatabaseTableName(string hostName, string provider, string databaseName, string tableName)
        {
            try
            {
                var table = await _entitySchemaService.GetTableByHostProviderDatabaseTableNameAsync(hostName, provider, databaseName, tableName);
                if (table == null)
                {
                    return NotFound();
                }

                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = table
                };

                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message },
                    Result = null
                };

                return StatusCode((int)responseModel.StatusCode, responseModel);
            }
        }

        [HttpGet("columns")]
        public async Task<IActionResult> GetAllColumns()
        {
            try
            {
                var columns = await _entitySchemaService.GetAllColumnsAsync();

                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = columns
                };
                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message },
                    Result = null
                };
                return StatusCode((int)responseModel.StatusCode, responseModel);
            }
        }

        [HttpGet("columns/{id}")]
        public async Task<IActionResult> GetColumnById(int id)
        {
            try
            {
                var column = await _entitySchemaService.GetColumnByIdAsync(id);
                if (column == null)
                {
                    return NotFound();
                }

                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = column
                };
                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message },
                    Result = null
                };
                return StatusCode((int)responseModel.StatusCode, responseModel);
            }
        }

        [HttpGet("columns/{id}/{entityid}")]
        public async Task<IActionResult> GetColumnByIdAndEntityId(int id, int entityid)
        {
            try
            {
                var column = await _entitySchemaService.GetColumnByIdAndEntityIDAsync(id, entityid);
                if (column == null)
                {
                    return NotFound();
                }

                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = column
                };
                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message },
                    Result = null
                };
                return StatusCode((int)responseModel.StatusCode, responseModel);
            }
        }

        [HttpGet("columns/entity/{entityId}")]
        public async Task<IActionResult> GetColumnsByEntityId(int entityId)
        {
            try
            {
                var columns = await _entitySchemaService.GetColumnsByEntityIdAsync(entityId);

                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = columns
                };
                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message },
                    Result = null
                };
                return StatusCode((int)responseModel.StatusCode, responseModel);
            }
        }

        [HttpPost("createtables")]
        public async Task<IActionResult> InsertTable([FromQuery] DBConnectionDTO connectionDTO, [FromBody] TableRequest tableRequest)
        {
            try
            {
                var craetetable = await _entitySchemaService.convertandcallcreatetablemodel(connectionDTO, tableRequest);

                // Insert table
                var tableId = await _entitySchemaService.CreateTableAsync(tableRequest.Table);

                // Set the inserted table ID in each column
                foreach (var column in tableRequest.Columns)
                {
                    column.EntityId = tableId;
                }

                // Insert columns
                await _entitySchemaService.InsertColumnsAsync(tableRequest.Columns);

                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.Created,
                    IsSuccess = true,
                    Result = new { TableId = tableId }
                };
                return CreatedAtAction(nameof(GetTableById), new
                {
                    id = tableId
                }, responseModel);
            }
            catch (Exception ex)
            {
                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message },
                    Result = null
                };
                return StatusCode((int)responseModel.StatusCode, responseModel);
            }
        }

        [HttpGet("cliententity")]
        public async Task<IActionResult> ClientEntity([FromQuery] DBConnectionDTO connectionDTO)
        {
            try
            {
                // Assuming you have a valid base URL for the other project
                string otherApiBaseUrl = "https://localhost:7246/";

                // Create HttpClient instance
                using (var httpClient = new HttpClient())
                {
                    // Set the base address of the other API
                    httpClient.BaseAddress = new Uri(otherApiBaseUrl);

                    string encodedPassword = Uri.EscapeDataString(connectionDTO.Password);

                    // Call the other API to get table details
                    var response = await httpClient.GetAsync($"EntityMigrate/GetTableDetails?Provider={connectionDTO.Provider}&HostName={connectionDTO.HostName}&DataBase={connectionDTO.DataBase}&UserName={connectionDTO.UserName}&Password={encodedPassword}");

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content
                        var responseContent = await response.Content.ReadAsStringAsync();

                        // Parse the response content as needed (assuming it's JSON)
                        var tableDetails = JsonConvert.DeserializeObject<APIResponse>(responseContent);

                        if (!tableDetails.IsSuccess)
                        {
                            var responseModel = new APIResponse
                            {
                                StatusCode = HttpStatusCode.OK,
                                IsSuccess = false,
                                Result = tableDetails.Result
                            };
                            return Ok(responseModel);
                        }
                        else
                        {
                            var responcedetials = await _entitySchemaService.GetClientSchema(tableDetails, connectionDTO);

                            // Continue with your logic...

                            var responseModel = new APIResponse
                            {
                                StatusCode = HttpStatusCode.Created,
                                IsSuccess = true,
                                Result = tableDetails
                            };
                            return Ok(responseModel);
                        }
                    }
                    else
                    {
                        // Handle unsuccessful response
                        var responseModel = new APIResponse
                        {
                            StatusCode = response.StatusCode,
                            IsSuccess = false,
                            Result = null // You might want to include more details here
                        };

                        return StatusCode((int)responseModel.StatusCode, responseModel);
                    }
                }
            }
            catch (Exception ex)
            {
                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message },
                    Result = null
                };
                return StatusCode((int)responseModel.StatusCode, responseModel);
            }
        }

        [HttpPost("updatetables")]
        public async Task<IActionResult> UpdateTable([FromBody] List<ColumnDTO> columns)
        {
            try
            {
                await _entitySchemaService.UpdateColumnsAsync(columns);

                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = null  // You can assign a meaningful result here if needed
                };

                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message },
                    Result = null
                };

                return StatusCode((int)responseModel.StatusCode, responseModel);
            }
        }

        [HttpGet("columns/{hostName}/{provider}/{databaseName}/{tableName}")]
        public async Task<IActionResult> GetColumnsByHostProviderDatabaseTableName(string hostName, string provider, string databaseName, string tableName)
        {
            try
            {
                var table = await _entitySchemaService.GetColumnsByHostProviderDatabaseTableNameAsync(hostName, provider, databaseName, tableName);
                if (table == null)
                {
                    return NotFound();
                }

                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = table
                };

                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message },
                    Result = null
                };

                return StatusCode((int)responseModel.StatusCode, responseModel);
            }
        }

        [HttpGet("log/{logParentId}")]
        public async Task<IActionResult> GetLogByParentId(int logParentId)
        {
            try
            {
                var logDTO = await _logService.GetLogByParentIdAsync(logParentId);

                if (logDTO == null)
                {
                    return NotFound(); // or handle the case where log with the specified ID is not found
                }

                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = logDTO
                };

                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message },
                    Result = null
                };

                return StatusCode((int)responseModel.StatusCode, responseModel);
            }
        }

        [HttpGet("log/child/{logChildId}")]
        public async Task<IActionResult> GetLogByChildId(int logChildId)
        {
            try
            {
                var logDTO = await _logService.GetLogByChildIdAsync(logChildId);

                if (logDTO == null)
                {
                    return NotFound(); // or handle the case where log with the specified child ID is not found
                }

                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = logDTO
                };

                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message },
                    Result = null
                };

                return StatusCode((int)responseModel.StatusCode, responseModel);
            }
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetAllLogs()
        {
            try
            {
                var logDTOs = await _logService.GetAllLogsAsync();

                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = logDTOs
                };

                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message },
                    Result = null
                };

                return StatusCode((int)responseModel.StatusCode, responseModel);
            }
        }

        [HttpGet("logs/user/{userId}")]
        public async Task<IActionResult> GetLogsByUserId(int userId)
        {
            try
            {
                var logDTOs = await _logService.GetLogsByUserIdAsync(userId);

                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = logDTOs
                };

                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message },
                    Result = null
                };

                return StatusCode((int)responseModel.StatusCode, responseModel);
            }
        }

        [HttpGet("logs/entity/{entityId}")]
        public async Task<IActionResult> GetLogsByEntityId(int entityId)
        {
            try
            {
                var logDTOs = await _logService.GetLogsByEntityIdAsync(entityId);

                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = logDTOs
                };

                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message },
                    Result = null
                };

                return StatusCode((int)responseModel.StatusCode, responseModel);
            }
        }
    }
}
