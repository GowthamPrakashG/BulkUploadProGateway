using Microsoft.AspNetCore.Mvc;
using ClientSchemaHub.Models.DTO;
using ClientSchemaHub.Service.IService;
using System.Net;
using Swashbuckle.AspNetCore.Annotations;
using Newtonsoft.Json;
using System.Web;
//using System.Web;

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
                // Decode URL-encoded strings
                connectionDto.HostName = HttpUtility.UrlDecode(connectionDto.HostName);
                connectionDto.DataBase = HttpUtility.UrlDecode(connectionDto.DataBase);
                connectionDto.AccessKey = HttpUtility.UrlDecode(connectionDto.AccessKey);
                connectionDto.SecretKey = HttpUtility.UrlDecode(connectionDto.SecretKey);
                connectionDto.Provider = HttpUtility.UrlDecode(connectionDto.Provider);

                // Step 1: Inspect request data
                // Log or debug the received connectionDto to inspect its content
                // Example: System.Console.WriteLine("Received connectionDto: " + connectionDto);

                // Step 2: Check for transformation
                // Check if the HostName property of connectionDto contains any unexpected transformations
                //if (connectionDto.HostName.Contains("\\\\"))
                //{
                //    // Handle transformation (if needed)
                //    connectionDto.HostName = connectionDto.HostName.Replace("\\\\", "\\");
                //}

                // Now the connectionDto is ready to use without unintended transformations

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
        public async Task<ActionResult<APIResponse>> CreateTable([FromQuery] DBConnectionDTO connectionDto, string query)
        {
            try
            {
                connectionDto.HostName = HttpUtility.UrlDecode(connectionDto.HostName);
                connectionDto.DataBase = HttpUtility.UrlDecode(connectionDto.DataBase);
                connectionDto.AccessKey = HttpUtility.UrlDecode(connectionDto.AccessKey);
                connectionDto.SecretKey = HttpUtility.UrlDecode(connectionDto.SecretKey);
                connectionDto.Provider = HttpUtility.UrlDecode(connectionDto.Provider);

                var tabledetails = await _generalDatabaseService.CreateTable(connectionDto, query);

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
        public async Task<ActionResult<APIResponse>> InsertData(
    [FromQuery] string? ConnectionDTO,
    [FromQuery] string? ConvertedDataList,
    [FromQuery] string? BooleanColumns,
    [FromQuery] string? TableName)
        {
            try
            {

                if (ConnectionDTO == null || TableName == null)
                {
                    return BadRequest("Missing required parameters.");
                }

                // Decode URL-encoded ConnectionDTO string
                string decodedConnectionDTO = Uri.UnescapeDataString(ConnectionDTO);

                // Log the decoded ConnectionDTO for debugging purposes
                Console.WriteLine($"Decoded ConnectionDTO: {decodedConnectionDTO}");

                // Ensure proper escaping of JSON string
                if (decodedConnectionDTO.Contains("\\") && !decodedConnectionDTO.Contains("\\\\"))
                {
                    decodedConnectionDTO = decodedConnectionDTO.Replace("\\", "\\\\");
                }

                // Log the properly escaped ConnectionDTO
                Console.WriteLine($"Escaped ConnectionDTO: {decodedConnectionDTO}");

                // Deserialize the DBConnectionDTO object with error handling
                DBConnectionDTO connection;
                try
                {

                    connection = JsonConvert.DeserializeObject<DBConnectionDTO>(decodedConnectionDTO);
                }
                catch (JsonReaderException jsonEx)
                {
                    return BadRequest($"Invalid JSON format in ConnectionDTO: {jsonEx.Message}");
                }

                // Deserialize the BooleanColumns string to List<ColumnMetaDataDTO> with error handling
                List<ColumnMetaDataDTO> booleanColumns = new List<ColumnMetaDataDTO>();
                if (!string.IsNullOrEmpty(BooleanColumns))
                {
                    try
                    {
                        booleanColumns = JsonConvert.DeserializeObject<List<ColumnMetaDataDTO>>(BooleanColumns);
                    }
                    catch (JsonReaderException jsonEx)
                    {
                        return BadRequest($"Invalid JSON format in BooleanColumns: {jsonEx.Message}");
                    }
                }

                // Deserialize the ConvertedDataList string to List<Dictionary<string, string>> with error handling
                List<Dictionary<string, string>> convertedDataList = new List<Dictionary<string, string>>();
                if (!string.IsNullOrEmpty(ConvertedDataList))
                {
                    try
                    {
                        convertedDataList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(ConvertedDataList);
                    }
                    catch (JsonReaderException jsonEx)
                    {
                        return BadRequest($"Invalid JSON format in ConvertedDataList: {jsonEx.Message}");
                    }
                }

                // Call the service to insert data
                var success = await _generalDatabaseService.InsertdataGeneral(connection, convertedDataList, booleanColumns, TableName);

                // Prepare the response model
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
                connectionDto.AccessKey = HttpUtility.UrlDecode(connectionDto.AccessKey);
                connectionDto.SecretKey = HttpUtility.UrlDecode(connectionDto.SecretKey);
                connectionDto.Provider = HttpUtility.UrlDecode(connectionDto.Provider);
                connectionDto.HostName = HttpUtility.UrlDecode(connectionDto.HostName);
                connectionDto.DataBase = HttpUtility.UrlDecode(connectionDto.DataBase);

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
        public async Task<ActionResult<APIResponse>> GetTabledata([FromQuery]DBConnectionDTO connectionDto, string tableName)
        {
            try
            {
                connectionDto.HostName = HttpUtility.UrlDecode(connectionDto.HostName);
                connectionDto.DataBase = HttpUtility.UrlDecode(connectionDto.DataBase);
                connectionDto.AccessKey = HttpUtility.UrlDecode(connectionDto.AccessKey);
                connectionDto.SecretKey = HttpUtility.UrlDecode(connectionDto.SecretKey);
                connectionDto.Provider = HttpUtility.UrlDecode(connectionDto.Provider);

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
        public async Task<ActionResult<APIResponse>> GetPrimaryColumnData([FromQuery]DBConnectionDTO connectionDto, string tableName)
        {
            try
            {
                connectionDto.HostName = HttpUtility.UrlDecode(connectionDto.HostName);
                connectionDto.DataBase = HttpUtility.UrlDecode(connectionDto.DataBase);
                connectionDto.AccessKey = HttpUtility.UrlDecode(connectionDto.AccessKey);
                connectionDto.SecretKey = HttpUtility.UrlDecode(connectionDto.SecretKey);
                connectionDto.Provider = HttpUtility.UrlDecode(connectionDto.Provider);

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


        [HttpGet("HashValue")]
        public async Task<ActionResult<APIResponse>> ReceiveHashFromPort([FromQuery] DBConnectionDTO dBConnection)
        {
            try
            {
                // Decode URL-encoded strings
                //connectionDto.HostName = HttpUtility.UrlDecode(connectionDto.HostName);
                //connectionDto.DataBase = HttpUtility.UrlDecode(connectionDto.DataBase);
                //connectionDto.AccessKey = HttpUtility.UrlDecode(connectionDto.AccessKey);
                //connectionDto.SecretKey = HttpUtility.UrlDecode(connectionDto.SecretKey);
                //connectionDto.Provider = HttpUtility.UrlDecode(connectionDto.Provider);

                // Step 1: Inspect request data
                // Log or debug the received connectionDto to inspect its content
                // Example: System.Console.WriteLine("Received connectionDto: " + connectionDto);

                // Step 2: Check for transformation
                // Check if the HostName property of connectionDto contains any unexpected transformations
                //if (connectionDto.HostName.Contains("\\\\"))
                //{
                //    // Handle transformation (if needed)
                //    connectionDto.HostName = connectionDto.HostName.Replace("\\\\", "\\");
                //}

                // Now the connectionDto is ready to use without unintended transformations

                var tabledetails = await _generalDatabaseService.ReceiveHashFromPort(dBConnection);

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

    }
}
