using Microsoft.AspNetCore.Mvc;
using ClientSchemaHub.Models.DTO;
using ClientSchemaHub.Service.IService;
using System.Net;

namespace ClientSchemaHub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EntityMigrateController : ControllerBase
    {
        private readonly IGeneralDatabaseService _generalDatabaseService;
        public EntityMigrateController(IGeneralDatabaseService generalDatabaseService)
        {
            _generalDatabaseService = generalDatabaseService;
        }

        [HttpGet("GetTableDetails")]
        public async Task<ActionResult<APIResponse>> GetTableDetails([FromQuery] DBConnectionDTO connectionDto)
        {
            try
            {
                var tabledetails = await _generalDatabaseService.GetTableDetailsForAllTablesAsync(connectionDto);

                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = tabledetails
                };
                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message },
                    Result = ex.Message
                };
                return StatusCode((int)responseModel.StatusCode, responseModel);
            }
        }

        [HttpGet("CreateTable")]
        public async Task<ActionResult<APIResponse>> CreateTable([FromQuery] DBConnectionDTO connectionDto,string query)
        {
            try
            {
                var tabledetails = await _generalDatabaseService.CreateTable(connectionDto,query);

                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = tabledetails
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

        [HttpPost("InsertData")]
        public async Task<ActionResult<APIResponse>> InsertData([FromBody] InsertDataRequest request)
        {
            try
            {
                var success = await _generalDatabaseService.Insertdata(
                    request.ConnectionDTO,
                    request.ConvertedDataList,
                    request.BooleanColumns,
                    request.TableName
                );

                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = success,
                    Result = null
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

        [HttpGet("IsTableExists")]
        public async Task<ActionResult<APIResponse>> IsTableExists([FromQuery] DBConnectionDTO connectionDto, string tableName)
        {
            try
            {
                var isTableExists = await _generalDatabaseService.IsTableExists(connectionDto, tableName);

                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = isTableExists
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

        [HttpGet("GetTabledata")]
        public async Task<ActionResult<APIResponse>> GetTabledata([FromQuery] DBConnectionDTO connectionDto, string tableName)
        {
            try
            {
                var tableData = await _generalDatabaseService.GetTabledata(connectionDto, tableName);

                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = tableData
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

        [HttpGet("GetPrimaryColumnData")]
        public async Task<ActionResult<APIResponse>> GetPrimaryColumnData([FromQuery] DBConnectionDTO connectionDto, string tableName)
        {
            try
            {
                var primaryColumnData = await _generalDatabaseService.GetPrimaryColumnDataAsync(connectionDto, tableName);

                var responseModel = new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = primaryColumnData
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
