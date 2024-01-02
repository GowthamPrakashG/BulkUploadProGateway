using Microsoft.AspNetCore.Mvc;
using ClientSchemaHub.Models.DTO;
using ClientSchemaHub.Service;
using ClientSchemaHub.Service.IService;
using System.Net;

namespace ClientSchemaHub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EntityMigrateController : ControllerBase
    {
        private readonly GeneralDatabaseService _generalDatabaseService;
        public EntityMigrateController(IGeneralDatabaseService generalDatabaseService)
        {
             generalDatabaseService = _generalDatabaseService;
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
