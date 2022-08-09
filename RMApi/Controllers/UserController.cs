using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RMApi.Data;
using RMApi.Models;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;
using System.Security.Claims;

namespace RMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _manager;
        private readonly IUserData _userData;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext context,
                              UserManager<IdentityUser> manager,
                              IUserData userData,
                              ILogger<UserController> logger)
        {
            _context = context;
            _manager = manager;
            _userData = userData;
            _logger = logger;
        }

        [HttpGet]
        public UserModel GetById()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return _userData.GetUserById(userId).First();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            var output = new List<ApplicationUserModel>();

            var users = _context.Users.ToList();
            var userRoles = _context
                .UserRoles
                .Join(_context.Roles,
                      x => x.RoleId,
                      x => x.Id,
                      (x, y) => new { x.UserId, x.RoleId, y.Name });

            foreach (var user in users)
            {
                var u = new ApplicationUserModel
                {
                    Id = user.Id,
                    EmailAddress = user.Email,
                };

                u.Roles = userRoles
                    .Where(x => x.UserId == u.Id)
                    .ToDictionary(k => k.RoleId, v => v.Name);

                output.Add(u);
            }
            

            return output;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("Admin/GetAllRoles")]
        public Dictionary<string, string> GetAllRols()
        {
            var roles = _context.Roles.ToDictionary(x => x.Id, x => x.Name);

            return roles;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Admin/AddRole")]
        public async Task AddRole(UserRolePairModel pair)
        {
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _manager.FindByIdAsync(pair.UserId);

            _logger.LogInformation("Admin {Admin} added user {User} to role {Role}",
                loggedInUserId,
                user.Id,
                pair.RoleName);


            await _manager.AddToRoleAsync(user, pair.RoleName);

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Admin/RemoveRole")]
        public async Task RemoveRole(UserRolePairModel pair)
        {
            var user = await _manager.FindByIdAsync(pair.UserId);
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation("Admin {Admin} removed user {User} from role {Role}",
                loggedInUserId,
                user.Id,
                pair.RoleName);

            await _manager.RemoveFromRoleAsync(user, pair.RoleName);
        }

    }
}
