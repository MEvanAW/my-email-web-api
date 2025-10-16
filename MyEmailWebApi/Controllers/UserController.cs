using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyEmailWebApi.Constants;
using MyEmailWebApi.Data;
using MyEmailWebApi.DTO.UserDto;
using System.Security.Claims;

namespace MyEmailWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(ILogger<UserController> logger, SignInManager<IdentityUser> signInManager, UserContext userContext, UserManager<IdentityUser> userManager) : ControllerBase
    {
        #region consts
        private const string USER_CREATED_FORMAT = "User {0} has been created.";
        private const string DEPARTMENT = "department";
        private const string DEPARTMENTS_REQUIRED = "Departments field is required.";
        private const string FAILED_TO_FIND_USER_DATA = "Failed to find user data.";
        private const string INVALID_LOGIN_ATTEMPT = "Invalid login attempt.";
        private const string USER_LOCKED_OUT_FORMAT = "User {0} account locked out.";
        private const string USER_LOGIN_FORMAT = "User {0} logged in.";
        private const string USER_UPDATED_FORMAT = "User {0} has been updated";
        #endregion

        private readonly ILogger<UserController> _logger = logger;
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;
        private readonly UserContext _userContext = userContext;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        [HttpPost("[action]", Name = "RegisterUser")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            var user = new IdentityUser { UserName = request.Email, Email = request.Email };
            var result = await _userManager.CreateAsync(user, request.Password!);
            if (result.Succeeded)
            {
                var userCreatedMessage = string.Format(USER_CREATED_FORMAT, request.Email);
                _logger.LogInformation(userCreatedMessage);
                return StatusCode(201, userCreatedMessage);
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("[action]", Name = "UpdateUser")]
        public async Task<IActionResult> Update([FromBody] UpdateUserRequest request)
        {
            var userTask = _userManager.Users
                .Where((user) => user.NormalizedUserName == request.Email!.ToUpperInvariant())
                .SingleAsync();
            if (request.Departments is null || request.Departments.Length <= 0)
            {
                return BadRequest(DEPARTMENTS_REQUIRED);
            }
            var roles = new List<string>();
            var excludedRoles = new List<string>();
            // Updating roles. IdentityUser can hold several IdentityRoles by design
            (request.IsAdmin ? roles : excludedRoles).Add(RoleNames.ADMIN);
            (request.IsEmployee ? roles : excludedRoles).Add(RoleNames.EMPLOYEE);
            (request.IsManager ? roles : excludedRoles).Add(RoleNames.MANAGER);
            Task runningTask = Task.CompletedTask;
            var user = await userTask;
            if (roles.Count > 0)
            {
                runningTask = _userManager.AddToRolesAsync(user, roles);
            }
            if (excludedRoles.Count > 0)
            {
                await runningTask;
                runningTask = _userManager.RemoveFromRolesAsync(user, excludedRoles.Intersect(await _userManager.GetRolesAsync(user)));
            }
            await runningTask;
            var departmentClaims = (await _userManager.GetClaimsAsync(user)).Where((claim) => claim.Type == DEPARTMENT);
            // Updating departments. IdentityUser can hold several IdentityClaims by design
            var excludedClaims = departmentClaims.Where((claim) => !request.Departments.Contains(claim.Value));
            runningTask = _userManager.RemoveClaimsAsync(user, excludedClaims);
            var newDepartments = request.Departments.Where((department) => !departmentClaims.Select((claim) => claim.Value).Contains(department));
            await runningTask;
            await _userManager.AddClaimsAsync(user, newDepartments.Select((department) => new Claim(DEPARTMENT, department)));
            return Ok(string.Format(USER_UPDATED_FORMAT, request.Email));
        }

        [HttpPost("[action]", Name = "LoginUser")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _signInManager.PasswordSignInAsync(request.Email!, request.Password!, false, false);
            if (result.IsLockedOut)
            {
                string userLockedOutMessage = string.Format(USER_LOCKED_OUT_FORMAT, request.Email);
                _logger.LogWarning(userLockedOutMessage);
                return BadRequest(userLockedOutMessage);
            }
            if (result.Succeeded)
            {
                string userLoggedInMessage = string.Format(USER_LOGIN_FORMAT, request.Email);
                _logger.LogInformation(userLoggedInMessage);
                // Identity automatically returns cookie
                return Ok(userLoggedInMessage);
            }
            return BadRequest(INVALID_LOGIN_ATTEMPT);
        }

        [HttpGet("users", Name = "Users")]
        public async Task<IActionResult> Users()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return Unauthorized();
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(RoleNames.ADMIN))
            {
                return RedirectToAction(nameof(GetAll));
            }
            if (roles.Contains(RoleNames.MANAGER))
            {
                return RedirectToAction(nameof(DepartmentUsers));
            }
            if (roles.Contains(RoleNames.EMPLOYEE))
            {
                return RedirectToAction(nameof(Profile));
            }
            return Unauthorized();
        }

        // Without sufficient authorization, automatically returns 404 code
        [Authorize(Roles = "Admin,Manager,Employee")]
        [HttpGet("[action]", Name = "UserProfile")]
        public async Task<IActionResult> Profile()
        {
            var userTask = _userManager.GetUserAsync(User);
            var response = new UserProfileResponse
            {
                Departments = User.Claims.Where((claim) => claim.Type == DEPARTMENT).Select((claim) => claim.Value)
            };
            var user = await userTask;
            if (user is null)
            {
                return StatusCode(500, FAILED_TO_FIND_USER_DATA);
            }
            var rolesTask = _userManager.GetRolesAsync(user);
            response.Email = user.UserName;
            response.Roles = await rolesTask;
            return Ok(response);
        }

        // Without sufficient authorization, automatically returns 404 code
        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("[action]", Name = "DepartmentUsers")]
        public async Task<IActionResult> DepartmentUsers()
        {
            var userDepartments = User.Claims.Where((claim) => claim.Type == DEPARTMENT).Select((claim) => claim.Value);
            var rolesTask = _userContext.Roles
                .Select((role) => new IdentityRole
                {
                    Id = role.Id,
                    Name = role.Name
                })
                .ToDictionaryAsync((role) => role.Id);
            var userClaimsQueryable = _userContext.UserClaims
                .Where((userClaim) => userClaim.ClaimType == DEPARTMENT && userDepartments.Contains(userClaim.ClaimValue))
                .Select((userClaim) => new IdentityUserClaim<string>
                {
                    UserId = userClaim.UserId,
                    ClaimValue = userClaim.ClaimValue
                });
            var rolesDictionary = await rolesTask;
            var userClaims = await userClaimsQueryable.ToArrayAsync();
            var userIds = userClaims.Select((userClaim) => userClaim.UserId).Distinct();
            var usersTask = _userManager.Users
                .Where((user) => userIds.Contains(user.Id))
                .Select((user) => new IdentityUser
                {
                    Id = user.Id,
                    UserName = user.UserName
                })
                .ToArrayAsync();
            var userRolesQueryable = _userContext.UserRoles
                .Where((userRole) => userIds.Contains(userRole.UserId));
            var users = await usersTask;
            var userRoles = await userRolesQueryable.ToArrayAsync();
            var responseList = new List<UserProfileResponse>();
            foreach (var userId in userIds)
            {
                responseList.Add(new UserProfileResponse
                {
                    Departments = userClaims
                        .Where((userClaim) => userClaim.UserId == userId)
                        .Select((userClaim) => userClaim.ClaimValue ?? string.Empty),
                    Email = users.Where((user) => user.Id == userId).Select((user) => user.UserName).Single(),
                    Roles = userRoles
                        .Where((userRole) => userRole.UserId == userId)
                        .Select((userRole) =>
                        {
                            rolesDictionary.TryGetValue(userRole.RoleId, out var role);
                            return role?.Name ?? string.Empty;
                        })
                });
            }
            return Ok(responseList);
        }

        // Without sufficient authorization, automatically returns 404 code
        [Authorize(Roles = "Admin")]
        [HttpGet("[action]", Name = "GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var rolesTask = _userContext.Roles
                .Select((role) => new IdentityRole
                {
                    Id = role.Id,
                    Name = role.Name
                })
                .ToDictionaryAsync((role) => role.Id);
            var userClaimsQueryable = _userContext.UserClaims
                .Where((userClaim) => userClaim.ClaimType == DEPARTMENT)
                .Select((userClaim) => new IdentityUserClaim<string>
                {
                    UserId = userClaim.UserId,
                    ClaimValue = userClaim.ClaimValue
                });
            var rolesDictionary = await rolesTask;
            var userClaims = await userClaimsQueryable.ToArrayAsync();
            var userIds = userClaims.Select((userClaim) => userClaim.UserId).Distinct();
            var usersTask = _userManager.Users
                .Where((user) => userIds.Contains(user.Id))
                .Select((user) => new IdentityUser
                {
                    Id = user.Id,
                    UserName = user.UserName
                })
                .ToArrayAsync();
            var userRolesQueryable = _userContext.UserRoles
                .Where((userRole) => userIds.Contains(userRole.UserId));
            var users = await usersTask;
            var userRoles = await userRolesQueryable.ToArrayAsync();
            var responseList = new List<UserProfileResponse>();
            foreach (var userId in userIds)
            {
                responseList.Add(new UserProfileResponse
                {
                    Departments = userClaims
                        .Where((userClaim) => userClaim.UserId == userId)
                        .Select((userClaim) => userClaim.ClaimValue ?? string.Empty),
                    Email = users.Where((user) => user.Id == userId).Select((user) => user.UserName).Single(),
                    Roles = userRoles
                        .Where((userRole) => userRole.UserId == userId)
                        .Select((userRole) =>
                        {
                            rolesDictionary.TryGetValue(userRole.RoleId, out var role);
                            return role?.Name ?? string.Empty;
                        })
                });
            }
            return Ok(responseList);
        }
    }
}
