using AuthCraftHub.Models.DTO;
using System.Security.Claims;
using System.Security.Cryptography;
using DBUtilityHub.Data;
using DBUtilityHub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace AuthCraftHub.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private IEnumerable<RoleScreenMappingDTO> screens;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Auth JWT
        internal async Task<UserTokenDTO> AuthenticateAsync(LoginDTO user)
        {
            var userModel = await _context.UserEntity.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (userModel == null || !VerifyPassword(userModel.Password, user.Password))
            {
                return null;
            }

            var userDetailsDTO = new UserTokenDTO
            {
                UserName = userModel.Name,
                Token = GenerateJwtToken((UserDTO)userModel)
            };

            return userDetailsDTO;
        }

        private bool VerifyPassword(string storedHashedPassword, string enteredPassword)
        {
            var passwordHasher = new PasswordHasher<UserDTO>();
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(null, storedHashedPassword, enteredPassword);
            return passwordVerificationResult == PasswordVerificationResult.Success;
        }

        private string HashPassword(string password)
        {
            var passwordHasher = new PasswordHasher<UserDTO>();
            return passwordHasher.HashPassword(null, password);
        }
        private string GenerateJwtToken(UserDTO user)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = GenerateRandomKey();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                     new Claim(ClaimTypes.Name, user.Id.ToString()),
                     new Claim(ClaimTypes.NameIdentifier, user.Name.ToString()), // Use ClaimTypes.NameIdentifier for userId
                     new Claim(ClaimTypes.Email, user.Email),
                     new Claim(ClaimTypes.Role, GetRoleName(user.RoleId).ToString())
                }),

                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private byte[] GenerateRandomKey()
        {
            var key = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }
            return key;
        }

        private string GetRoleName(int roleId)
        {
            string? roleName = _context.RoleEntity.Where(r => r.Id == roleId).Select(r => r.RoleName).FirstOrDefault();
            if (string.IsNullOrEmpty(roleName))
            {
                return string.Empty;
            }
            return roleName;
        }

        
        // User CRUD
        internal async Task<UserMaintenanceDTO> CreateUserAsync(UserMaintenanceDTO userModel)
        {
            var role = await _context.RoleEntity.FirstOrDefaultAsync(r => r.Id == userModel.RoleId);

            if (role != null)
            {
                UserEntity newUser = new UserEntity();
                userModel.Password = HashPassword(userModel.Password);

                _context.Entry(newUser).CurrentValues.SetValues(userModel);
                _context.UserEntity.Add(newUser);
                await _context.SaveChangesAsync();

                UserMaintenanceDTO userMaintenanceDTO = (UserMaintenanceDTO)newUser;
                return userMaintenanceDTO;
            }
            return null;
        }

        internal async Task<IEnumerable<UserDTO>> GetUsersAsync()
        {
            var users = _context.UserEntity.Include(c => c.Role).ToList();

            var userDTO = new List<UserDTO>();

            userDTO.AddRange(users.Select(c => (UserDTO)c));

            return userDTO;
        }

        internal async Task<UserDTO?> GetUserAsync(int id)
        {

            var user = await _context.UserEntity.Include(c => c.Role).FirstOrDefaultAsync(c => c.Id == id);

            if (user == null)
            {
                return null;
            }

            UserDTO userDTO = (UserDTO)user;

            return userDTO;
        }

        internal async Task<UserDTO?> GetUserAsync(string email)
        {
            var user = await _context.UserEntity.FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());

            if (user == null)
            {
                return null;
            }

            UserDTO userDTO = (UserDTO)user;

            return userDTO;
        }

        internal async Task<IEnumerable<UserDTO?>> GetUserByRoleAsync(int roleID)
        {
            var users = _context.UserEntity.Include(c => c.Role).Where(c => c.RoleId == roleID).ToList();

            var userDTO = new List<UserDTO>();

            userDTO.AddRange(users.Select(c => (UserDTO)c));

            return userDTO;
        }

        internal async Task<UserMaintenanceDTO> UpdateUserAsync(UserMaintenanceDTO userTableModelDTO)
        {
            UserEntity? user = await _context.UserEntity.FirstOrDefaultAsync(c => c.Id == userTableModelDTO.Id);
            var role = await _context.RoleEntity.FirstOrDefaultAsync(r => r.Id == userTableModelDTO.RoleId);

            if (user != null && role != null)
            {
                if (user.Password == userTableModelDTO.Password)
                {
                    userTableModelDTO.Password = user.Password;
                }
                else if(!string.IsNullOrEmpty(userTableModelDTO.Password))
                {
                    userTableModelDTO.Password = HashPassword(userTableModelDTO.Password);
                }
                else
                {
                    userTableModelDTO.Password = user.Password;
                }
                
                _context.Entry<UserEntity>(user).CurrentValues.SetValues(userTableModelDTO);
                _context.SaveChanges();

                UserMaintenanceDTO userMaintenanceDTO = (UserMaintenanceDTO)user;
                return userMaintenanceDTO;
            }

            return null;

        }

        // Role GetEndpoints
        internal async Task<List<RoleDTO>> GetRoles()
        {
            try
            {
                var roleData = _context.RoleEntity
                    .Select(role => new RoleDTO
                    {
                        Id = role.Id,
                        RoleName = role.RoleName
                    })
                    .ToList();

                return roleData;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
                // You might want to throw an exception or return an error response here
                return null;
            }
        }

        internal async Task<RoleDTO?> GetRoleById(int id)
        {
            var role = _context.RoleEntity.FirstOrDefaultAsync(c => c.Id == id).Result;

            if (role == null)
            {
                return null;
            }

            RoleDTO RoleDTO = (RoleDTO)role;

            return RoleDTO;
        }

        internal async Task<bool> CreateRole(RoleDTO roleDTO)
        {
            try
            {
                await _context.RoleEntity.AddAsync(roleDTO);

                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
                // You might want to throw an exception or return an error response here
                return false;
            }
        }

        internal async Task<RoleDTO?> GetRoleByName(string rolename)
        {
            var role = _context.RoleEntity.FirstOrDefaultAsync(c => c.RoleName.ToLower() == rolename.ToLower()).Result;

            if (role == null)
            {
                return null;
            }

            RoleDTO RoleDTO = (RoleDTO)role;

            return RoleDTO;
        }

        internal async Task<RoleDTO?> UpdateRoleNameAsync(int roleId, string newRoleName)
        {
            try
            {
                // Check if the new role name already exists
                var existingRole = await _context.RoleEntity
                    .FirstOrDefaultAsync(r => r.RoleName.ToLower() == newRoleName.ToLower() && r.Id != roleId);

                if (existingRole != null)
                {
                    // The new role name already exists, return null or handle as needed
                    return null;
                }

                // Retrieve the role to be updated
                var roleToUpdate = await _context.RoleEntity.FirstOrDefaultAsync(r => r.Id == roleId);

                if (roleToUpdate == null)
                {
                    // Role not found, return null or handle as needed
                    return null;
                }

                // Update the role name
                roleToUpdate.RoleName = newRoleName;

                // Save changes to the database
                _context.SaveChanges();

                // Return the updated RoleDTO
                return new RoleDTO
                {
                    Id = roleToUpdate.Id,
                    RoleName = roleToUpdate.RoleName
                };
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
                // You might want to throw an exception or return an error response here
                return null;
            }
        }

        // Screens
        internal async Task<List<ScreenDTO>> GetScreens()
        {
            try
            {
                var ScreenData = _context.ScreenEntity
                    .Select(Screen => new ScreenDTO
                    {
                        Id = Screen.Id,
                        ScreenName = Screen.ScreenName,
                        RouteURL = Screen.RouteURL
                    })
                    .ToList();

                return ScreenData;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
                // You might want to throw an exception or return an error response here
                return null;
            }
        }

        internal async Task<ScreenDTO?> GetScreenById(int id)
        {
            var Screen = _context.ScreenEntity.FirstOrDefaultAsync(c => c.Id == id).Result;

            if (Screen == null)
            {
                return null;
            }

            ScreenDTO ScreenDTO = (ScreenDTO)Screen;

            return ScreenDTO;
        }

        internal async Task<bool> CreateScreen(ScreenDTO ScreenDTO)
        {
            try
            {
                await _context.ScreenEntity.AddAsync(ScreenDTO);

                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
                // You might want to throw an exception or return an error response here
                return false;
            }
        }

        internal async Task<ScreenDTO?> GetScreenByName(string Screenname)
        {
            var Screen = _context.ScreenEntity.FirstOrDefaultAsync(c => c.ScreenName.ToLower() == Screenname.ToLower()).Result;

            if (Screen == null)
            {
                return null;
            }

            ScreenDTO ScreenDTO = (ScreenDTO)Screen;

            return ScreenDTO;
        }

        internal async Task<ScreenDTO?> UpdateScreenNameAsync(int ScreenId, ScreenDTO newScreenName)
        {
            try
            {
                // Check if the new Screen name already exists
                var existingScreen = await _context.ScreenEntity
                    .FirstOrDefaultAsync(r => r.ScreenName.ToLower() == newScreenName.ScreenName.ToLower() && r.Id != ScreenId);

                if (existingScreen != null)
                {
                    // The new Screen name already exists, return null or handle as needed
                    return null;
                }

                // Retrieve the Screen to be updated
                var ScreenToUpdate = await _context.ScreenEntity.FirstOrDefaultAsync(r => r.Id == ScreenId);

                if (ScreenToUpdate == null)
                {
                    // Screen not found, return null or handle as needed
                    return null;
                }

                // Update the Screen name
                ScreenToUpdate.ScreenName = newScreenName.ScreenName;
                ScreenToUpdate.RouteURL = newScreenName.RouteURL;

                // Save changes to the database
                _context.SaveChanges();

                // Return the updated ScreenDTO
                return new ScreenDTO
                {
                    Id = ScreenToUpdate.Id,
                    ScreenName = ScreenToUpdate.ScreenName,
                    RouteURL = ScreenToUpdate.RouteURL
                };
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
                // You might want to throw an exception or return an error response here
                return null;
            }
        }

        //RoleScreen
        internal async Task<List<RoleScreenMappingDTO>> GetRolesScreens()
        {
            try
            {

                var roleScreen = _context.RoleScreenMapping.Include(c => c.Role).Include(c => c.Screen).ToList();
                var roleScreenMappingDTO = new List<RoleScreenMappingDTO>();
                roleScreenMappingDTO.AddRange(roleScreen.Select(c => (RoleScreenMappingDTO)c));
                
                return roleScreenMappingDTO;
              
    }
            
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
                // You might want to throw an exception or return an error response here
                return null;
            }

        }

        internal async Task<RoleScreenMappingDTO?> GetRoleScreenByMapId(int id)
        {
            var Screen = _context.RoleScreenMapping.FirstOrDefaultAsync(c => c.Id == id).Result;

            if (Screen == null)
            {
                return null;
            }

            RoleScreenMappingDTO ScreenDTO = (RoleScreenMappingDTO)Screen;

            return ScreenDTO;
        }

        internal async Task<List<RoleScreenMappingDTO>?> GetRoleScreenByRoleId(int id)
        {
            var screens = await _context.RoleScreenMapping.Where(c => c.RoleId == id).ToListAsync();

            if (screens == null || screens.Count == 0)
            {
                return null;
            }

            List<RoleScreenMappingDTO> screenDTOs = screens.Select(screen => (RoleScreenMappingDTO)screen).ToList();

            return screenDTOs;
        }

        internal async Task<List<RoleScreenMappingDTO>?> GetRoleScreenByScreenId(int id)
        {
            var screens = await _context.RoleScreenMapping.Where(c => c.ScreenId == id).ToListAsync();

            if (screens == null || screens.Count == 0)
            {
                return null;
            }

            List<RoleScreenMappingDTO> screenDTOs = screens.Select(screen => (RoleScreenMappingDTO)screen).ToList();

            return screenDTOs;
        }

        internal async Task<RoleScreenMaintainanceDTO> CreateRoleScreenAsync(RoleScreenMaintainanceDTO roleScreenModel)
        {
            var existingMapping = await _context.RoleScreenMapping
                .FirstOrDefaultAsync(r => r.RoleId == roleScreenModel.RoleId && r.ScreenId == roleScreenModel.ScreenId);

            if (existingMapping != null)
            {
                RoleScreenMaintainanceDTO roleScreenMaintainanceDTO = new RoleScreenMaintainanceDTO
                {
                    RoleId = existingMapping.RoleId,
                    ScreenId = existingMapping.ScreenId,
                    Id = existingMapping.Id
                };

                return roleScreenMaintainanceDTO;

            }
            else
            {
                RoleScreenMapping newRoleScreen = new RoleScreenMapping
                {
                    RoleId = roleScreenModel.RoleId,
                    ScreenId = roleScreenModel.ScreenId

                };
                _context.RoleScreenMapping.Add(newRoleScreen);
                await _context.SaveChangesAsync();

                RoleScreenMaintainanceDTO roleScreenMaintainanceDTO = (RoleScreenMaintainanceDTO)newRoleScreen;
                return roleScreenMaintainanceDTO;
            }
        }

        internal async Task<RoleScreenMappingDTO?> UpdateRoleScreenNameAsync(int Id, RoleScreenMappingDTO newScreenName)
        {
            try
            {
                // Check if the new Screen name already exists
                var existingScreen = await _context.RoleScreenMapping
                    .FirstOrDefaultAsync(r => r.ScreenId == newScreenName.ScreenId && r.Id != Id && r.RoleId != newScreenName.RoleId);

                if (existingScreen != null)
                {
                    // The new Screen name already exists, return null or handle as needed
                    return null;
                }

                // Retrieve the Screen to be updated
                var ScreenToUpdate = await _context.RoleScreenMapping.FirstOrDefaultAsync(r => r.Id == Id);

                if (ScreenToUpdate == null)
                {
                    // Screen not found, return null or handle as needed
                    return null;
                }

                // Update the Screen name
                //ScreenToUpdate.ScreenId = newScreenName.ScreenId;
                ScreenToUpdate.RoleId = newScreenName.RoleId;

                // Save changes to the database
                _context.SaveChanges();

                // Return the updated ScreenDTO
                return new RoleScreenMappingDTO
                {
                    Id = ScreenToUpdate.Id,
                    RoleId = ScreenToUpdate.RoleId,
                    //ScreenId = ScreenToUpdate.ScreenId
                };
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
                // You might want to throw an exception or return an error response here
                return null;
            }
        }
    }
}

