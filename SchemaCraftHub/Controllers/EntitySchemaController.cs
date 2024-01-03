using System.Net;
using ClientSchemaHub.Models.DTO;
using ClientSchemaHub.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchemaCraftHub.Model.DTO;
using SchemaCraftHub.Service.IService;

namespace SchemaCraftHub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EntitySchemaController : ControllerBase
    {
        private readonly IEntitySchemaService _entitySchemaService;

        public EntitySchemaController(IEntitySchemaService entitySchemaService)
        {
            _entitySchemaService = entitySchemaService;
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

        [HttpGet("columns/{entityId}")]
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
        public async Task<IActionResult> InsertTable([FromQuery]DBConnectionDTO connectionDTO, [FromBody] TableRequest tableRequest)
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

        [HttpPost("cliententity")]
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

                        var responcedetials = await _entitySchemaService.GetClientSchema(tableDetails,connectionDTO);

                        // Continue with your logic...

                        var responseModel = new APIResponse
                        {
                            StatusCode = HttpStatusCode.Created,
                            IsSuccess = true,
                            Result = tableDetails
                        };

                        return Ok(responseModel);
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
    }
}
