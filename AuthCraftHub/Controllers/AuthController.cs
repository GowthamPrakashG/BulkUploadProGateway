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
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userDetailsDTO = await _authService.AuthenticateAsync(model.Email, model.Password);
                if (userDetailsDTO != null)
                {
                    var responseModel = new APIResponse
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSuccess = true,
                        Result = userDetailsDTO.Token
                    };
                    return Ok(responseModel);
                }
                else
                {
                    var errorResponse = new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        IsSuccess = false,
                        ErrorMessage = "Invalid credentials"
                    };
                    return BadRequest(errorResponse);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("createUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO userModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorResponse = new
                    {
                        StatusCode = HttpStatusCode.SeeOther,
                        IsSuccess = false,
                        ErrorMessage = "Invalid credentials"
                    };
                    return BadRequest(ModelState);
                }

                var createdUser = await _authService.CreateUserAsync(userModel);

                if (createdUser != null)
                {
                    var errorResponse = new
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSuccess = true,
                        Result = createdUser
                    };
                    return Ok(errorResponse);
                }
                else
                {
                    var errorResponse = new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        IsSuccess = false,
                        ErrorMessage = "Failed to create user. Check role details and EmailId"
                    };
                    return BadRequest(errorResponse);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
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
                    var errorResponse = new
                    {
                        StatusCode = HttpStatusCode.NoContent,
                        IsSuccess = false,
                        ErrorMessage = "There are no users in the database. Please ensure that user records exist and try again."
                    };

                    return Ok(errorResponse);
                }

                var Response = new
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = getUserList
                };
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("GetUserbyId")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var getUser = await _authService.GetUserAsync(id);

                if (getUser == null)
                {
                    var errorResponse = new
                    {
                        StatusCode = HttpStatusCode.NoContent,
                        IsSuccess = false,
                        ErrorMessage = "The user with the specified ID does not exist in the database. Please check the provided user ID and try again."
                    };

                    return Ok(errorResponse);
                }

                var Response = new
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = getUser
                };
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("GetUserbyEmail")]
        public async Task<IActionResult> GetUser(string email)
        {
            try
            {
                var getUser = await _authService.GetUserAsync(email);

                if (getUser == null)
                {
                    var errorResponse = new
                    {
                        StatusCode = HttpStatusCode.NoContent,
                        IsSuccess = false,
                        ErrorMessage = "The user with the specified ID does not exist in the database. Please check the provided user ID and try again."
                    };

                    return Ok(errorResponse);
                }

                var Response = new
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = getUser
                };
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("UpdateUserbyId")]
        public async Task<IActionResult> UpdateUser([FromBody] UserDTO userTableModelDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorResponse = new
                    {
                        StatusCode = HttpStatusCode.SeeOther,
                        IsSuccess = false,
                        ErrorMessage = "Invalid User Details"
                    };
                    return BadRequest(ModelState);
                }

                var UpdatedUser = await _authService.UpdateUserAsync(userTableModelDTO);

                if (UpdatedUser != null)
                {
                    var errorResponse = new
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSuccess = true,
                        Result = UpdatedUser
                    };
                    return Ok(errorResponse);
                }
                else
                {
                    var errorResponse = new
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        IsSuccess = false,
                        ErrorMessage = "Failed to update user. Check role details and EmailId"
                    };
                    return BadRequest(errorResponse);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("getRoleIdsAndNames")]
        public ActionResult<List<RoleDTO>> GetRoleIdsAndNames()
        {
            try
            {
                var roleData = _authService.GetRoleIdsAndNames();

                if (roleData != null && roleData.Any())
                {
                    var response = new
                    {
                        StatusCode = 200, // OK
                        IsSuccess = true,
                        Result = roleData
                    };
                    return Ok(response);
                }
                else
                {
                    var response = new
                    {
                        StatusCode = 204, // No Content
                        IsSuccess = false,
                        ErrorMessage = "No roles found."
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
