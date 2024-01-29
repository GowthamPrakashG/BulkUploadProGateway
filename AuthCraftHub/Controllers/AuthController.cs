using AuthCraftHub.Models.DTO;
using System.Net;
using AuthCraftHub.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DBUtilityHub.Models;

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
                if (IsUserExists  != null)
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
                if(id == 0)
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
    }
}
