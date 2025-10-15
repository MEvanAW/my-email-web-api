using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyEmailWebApi.Constants;
using MyEmailWebApi.DTO.UserDto;
using System.Security.Claims;

namespace MyEmailWebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {
        #region consts
        private const string USER_CREATED_FORMAT = "User {0} has been created.";
        private const string DEPARTMENT = "department";
        private const string DEPARTMENTS_REQUIRED = "Departments field is required";
        private const string USER_UPDATED_FORMAT = "User {0} has been updated";
        #endregion

        private readonly ILogger<UserController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ILogger<UserController> logger, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost(Name = "RegisterUser")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            var user = new IdentityUser { UserName = request.Email, Email = request.Email };
            var result = await _userManager.CreateAsync(user, request.Password!);
            if (result.Succeeded)
            {
                var userCreatedMessage = string.Format(USER_CREATED_FORMAT, request.Email);
                _logger.LogInformation(userCreatedMessage);
                return Ok(userCreatedMessage);
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return BadRequest(ModelState);
        }

        [HttpPost(Name = "UpdateUser")]
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
                runningTask = _userManager.RemoveFromRolesAsync(user, excludedRoles);
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
    }
}
