using AuthCraftHub.Models.DTO;
using System.Net;
using AuthCraftHub.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DBUtilityHub.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthCraftHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        protected APIResponse _response;
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
            _response = new();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = ModelState.Values.SelectMany(c => c.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(_response);
                }

                var userDetailsDTO = await _authService.AuthenticateAsync(model);
                if (userDetailsDTO != null)
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = userDetailsDTO;
                    return Ok(_response);
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "Username or password is incorrect" };
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { $"An error occurred while processing your request: {Environment.NewLine} {ex.Message} " };
                return StatusCode(500, _response);
            }
        }

        [HttpPost("createUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserMaintenanceDTO userModel)
        {
            try
            {
                if (!ModelState.IsValid || userModel == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = ModelState.Values.SelectMany(c => c.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(_response);
                }

                var IsUserExists = await _authService.GetUserAsync(userModel.Email);
                if (IsUserExists != null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "Failed to create user. EmailId already exist" };
                    return BadRequest(_response);
                }

                var createdUser = await _authService.CreateUserAsync(userModel);

                if (createdUser != null)
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = createdUser;
                    return Ok(_response);
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "Failed to create user. Check role details" };
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { $"An error occurred while processing your request: {Environment.NewLine} {ex.Message} " };
                return StatusCode(500, _response);
            }
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var getUserList = await _authService.GetUsersAsync();

                if (getUserList == null)
                {
                    _response.StatusCode = HttpStatusCode.NoContent;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "There are no users in the database. Please ensure that user records exist and try again." };
                    return StatusCode(204, _response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = getUserList;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { $"An error occurred while processing your request: {Environment.NewLine} {ex.Message} " };
                return StatusCode(500, _response);
            }
        }

        [HttpGet("GetUserbyId")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            { 
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadGateway;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "The 'id' parameter cannot be 0 or null." };
                    return BadRequest(_response);
                }

                var getUser = await _authService.GetUserAsync(id);

                if (getUser == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "The user with the specified ID does not exist in the database. Please check the provided user ID and try again." };
                    return NotFound(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = getUser;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { $"An error occurred while processing your request: {Environment.NewLine} {ex.Message} " };
                return StatusCode(500, _response);
            }
        }

        [HttpGet("GetUserbyRoleId")]
        public async Task<IActionResult> GetUserByRole(int Roleid)
        {
            try
            {
                if (Roleid == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadGateway;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "The 'id' parameter cannot be 0 or null." };
                    return BadRequest(_response);
                }

                var getUser = await _authService.GetUserByRoleAsync(Roleid);

                if (getUser == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "The user with the specified ID does not exist in the database. Please check the provided user ID and try again." };
                    return NotFound(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = getUser;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { $"An error occurred while processing your request: {Environment.NewLine} {ex.Message} " };
                return StatusCode(500, _response);
            }
        }

        [HttpGet("GetUserbyEmail")]
        public async Task<IActionResult> GetUser(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "The 'email' parameter cannot be empty or null." };
                    return BadRequest(_response);
                }
                var getUser = await _authService.GetUserAsync(email);

                if (getUser == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "The user with the specified email does not exist in the database. Please check the provided user email and try again." };
                    return NotFound(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = getUser;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { $"An error occurred while processing your request: {Environment.NewLine} {ex.Message} " };
                return StatusCode(500, _response);
            }
        }

        [HttpPut("UpdateUserbyId")]
        public async Task<IActionResult> UpdateUser([FromBody] UserMaintenanceDTO userModel)
        {
            try
            {
                if (!ModelState.IsValid || userModel == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = ModelState.Values.SelectMany(c => c.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(_response);
                }

                var UpdatedUser = await _authService.UpdateUserAsync(userModel);

                if (UpdatedUser != null)
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = UpdatedUser;
                    return Ok(_response);
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "Failed to update user. Check role details and EmailId" };
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { $"An error occurred while processing your request: {Environment.NewLine} {ex.Message} " };
                return StatusCode(500, _response);
            }
        }

        // Role Endpoints

        [HttpGet("getRoles")]
        public ActionResult<List<RoleDTO>> GetRoles()
        {
            try
            {
                var roleData = _authService.GetRoles();

                if (roleData != null)
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = roleData;
                    return Ok(_response);
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NoContent;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "There are no roles in the database. Please ensure that roles records exist and try again." };
                    return StatusCode(204, _response);
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { $"An error occurred while processing your request: {Environment.NewLine} {ex.Message} " };
                return StatusCode(500, _response);
            }
        }

        [HttpGet("getRoleById")]
        public ActionResult<RoleDTO> GetRole(int roleID)
        {
            try
            {
                if (roleID == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadGateway;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "The 'roleID' parameter cannot be 0 or null." };
                    return BadRequest(_response);
                }

                var getRole = _authService.GetRoleById(roleID).Result;

                if (getRole == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "The Role with the specified roleID does not exist in the database. Please check the provided role ID and try again." };
                    return NotFound(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = getRole;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { $"An error occurred while processing your request: {Environment.NewLine} {ex.Message} " };
                return StatusCode(500, _response);
            }
        }

        [HttpPost("createNewRole")]
        public async Task<ActionResult<APIResponse>> CreateRole(RoleDTO roleDTO)
        {
            try
            {
                if (roleDTO.RoleName == null)
                {
                    _response.StatusCode = HttpStatusCode.BadGateway;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "The roleDTO parameter cannot be null." };
                    return BadRequest(_response);
                }

                var IsroleExists = await _authService.GetRoleByName(roleDTO.RoleName);

                if (IsroleExists != null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "The RoleName  already exists" };
                    return NotFound(_response);
                }

                var getRole = await _authService.CreateRole(roleDTO);

                if (getRole == false)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "Couldnot able to create this role" };
                    return NotFound(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = getRole;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { $"An error occurred while processing your request: {Environment.NewLine} {ex.Message} " };
                return StatusCode(500, _response);
            }
        }

        [HttpPut("UpdateRoleById/{roleId}")]
        public async Task<IActionResult> UpdateRole(int roleId, [FromBody] RoleDTO roleModel)
        {
            try
            {
                if (!ModelState.IsValid || roleModel == null || roleId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = ModelState.Values.SelectMany(c => c.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(_response);
                }

                var updatedRole = await _authService.UpdateRoleNameAsync(roleId, roleModel.RoleName);

                if (updatedRole != null)
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = updatedRole;
                    return Ok(_response);
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "Role not found or the updated role name already exists." };
                    return NotFound(_response);
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { $"An error occurred while processing your request: {Environment.NewLine} {ex.Message} " };
                return StatusCode(500, _response);
            }
        }

        //Screens

        [HttpGet("getScreens")]
        public ActionResult<List<ScreenDTO>> GetScreens()
        {
            try
            {
                var ScreenData = _authService.GetScreens();


                if (ScreenData != null)
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = ScreenData;
                    return Ok(_response);
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NoContent;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "There are no Screens in the database. Please ensure that Screens records exist and try again." };
                    return StatusCode(204, _response);
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { $"An error occurred while processing your request: {Environment.NewLine} {ex.Message} " };
                return StatusCode(500, _response);
            }
        }

        [HttpGet("getScreenById")]
        public ActionResult<ScreenDTO> GetScreen(int ScreenID)
        {
            try
            {
                if (ScreenID == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadGateway;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "The 'ScreenID' parameter cannot be 0 or null." };
                    return BadRequest(_response);
                }

                var getScreen = _authService.GetScreenById(ScreenID).Result;

                if (getScreen == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "The Screen with the specified ScreenID does not exist in the database. Please check the provided Screen ID and try again." };
                    return NotFound(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = getScreen;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { $"An error occurred while processing your request: {Environment.NewLine} {ex.Message} " };
                return StatusCode(500, _response);
            }
        }

        [HttpPost("createNewScreen")]
        public async Task<ActionResult<APIResponse>> CreateScreen(ScreenDTO ScreenDTO)
        {
            try
            {
                if (ScreenDTO.ScreenName == null)
                {
                    _response.StatusCode = HttpStatusCode.BadGateway;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "The ScreenDTO parameter cannot be null." };
                    return BadRequest(_response);
                }

                var IsScreenExists = await _authService.GetScreenByName(ScreenDTO.ScreenName);

                if (IsScreenExists != null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "The ScreenName is already exists" };
                    return NotFound(_response);
                }

                var getScreen = await _authService.CreateScreen(ScreenDTO);

                if (getScreen == false)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "Couldnot able to create this Screen" };
                    return NotFound(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = getScreen;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { $"An error occurred while processing your request: {Environment.NewLine} {ex.Message} " };
                return StatusCode(500, _response);
            }
        }

        [HttpPut("UpdateScreenById/{ScreenId}")]
        public async Task<IActionResult> UpdateScreen(int ScreenId, [FromBody] ScreenDTO ScreenModel)
        {
            try
            {
                if (!ModelState.IsValid || ScreenModel == null || ScreenId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = ModelState.Values.SelectMany(c => c.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(_response);
                }

                var updatedScreen = await _authService.UpdateScreenNameAsync(ScreenId, ScreenModel);

                if (updatedScreen != null)
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = updatedScreen;
                    return Ok(_response);
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "Screen not found or the updated Screen name already exists." };
                    return NotFound(_response);
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { $"An error occurred while processing your request: {Environment.NewLine} {ex.Message} " };
                return StatusCode(500, _response);
            }
        }

        //RoleSreenMapping

        [HttpGet("GetRoleScreens")]
        public async Task<IActionResult> GetRoleScreens()
        {
            try
            {
                var roleScreenData = await _authService.GetRolesScreens();

                if (roleScreenData != null)
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = roleScreenData;
                    return Ok(_response);
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    return NotFound(_response);
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { $"An error occurred: {ex.Message}" };
                return StatusCode(500, _response);
            }
        }

        [HttpGet("GetRoleScreenByMapId/{id}")]
        public async Task<IActionResult> GetRoleScreenByMapId(int id)
        {
            try
            {
                var screen = await _authService.GetRoleScreenByMapId(id);

                if (screen != null)
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = screen;
                    return Ok(_response);
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    return NotFound(_response);
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { $"An error occurred: {ex.Message}" };
                return StatusCode(500, _response);
            }
        }

        [HttpGet("GetRoleScreenByRoleId/{id}")]
        public async Task<IActionResult> GetRoleScreenByRoleId(int id)
        {
            try
            {
                var screens = await _authService.GetRoleScreenByRoleId(id);

                if (screens != null)
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = screens;
                    return Ok(_response);
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    return NotFound(_response);
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { $"An error occurred: {ex.Message}" };
                return StatusCode(500, _response);
            }
        }

        [HttpGet("GetRoleScreenByScreenId/{id}")]
        public async Task<IActionResult> GetRoleScreenByScreenId(int id)
        {
            try
            {
                var screens = await _authService.GetRoleScreenByScreenId(id);

                if (screens != null)
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = screens;
                    return Ok(_response);
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    return NotFound(_response);
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { $"An error occurred: {ex.Message}" };
                return StatusCode(500, _response);
            }
        }

        [HttpPost("CreateRoleScreen")]

        public async Task<IActionResult> CreateRoleScreen([FromBody] RoleScreenMaintainanceDTO roleScreenModel)
        {
            try
            {
                if (!ModelState.IsValid || roleScreenModel == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = ModelState.Values.SelectMany(c => c.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(_response);
                }

                var createdrole = await _authService.CreateRoleScreenAsync(roleScreenModel);
                if (createdrole != null)
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = createdrole;
                    return Ok(_response);
                }

                else
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "Failed to create role screen mapping." };
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { $"An error occurred while processing your request: {Environment.NewLine} {ex.Message} " };
                return StatusCode(500, _response);
            }

        }
        [HttpPut("UpdateRoleScreenById/{id}")]
        public async Task<IActionResult> UpdateRoleScreen(int id, [FromBody] RoleScreenMappingDTO newScreenName)
        {
            try
            {
                if (!ModelState.IsValid || newScreenName == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = ModelState.Values.SelectMany(c => c.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(_response);
                }

                var updatedScreen = await _authService.UpdateRoleScreenNameAsync(id, newScreenName);

                if (updatedScreen != null)
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    _response.Result = updatedScreen;
                    return Ok(_response);
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessage = new List<string>() { "Role screen mapping not found or the updated screen name already exists." };
                    return NotFound(_response);
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string>() { $"An error occurred: {ex.Message}" };
                return StatusCode(500, _response);
            }
        }


    }
}
