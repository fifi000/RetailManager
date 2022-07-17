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
        private readonly IConfiguration _config;

        public UserController(ApplicationDbContext context,
                              UserManager<IdentityUser> manager,
                              IConfiguration config)
        {
            _context = context;
            _manager = manager;
            _config = config;
        }

        [HttpGet]
        public UserModel GetById()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserData data = new UserData(_config);

            return data.GetUserById(userId).First();
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
            var userId = await _manager.FindByIdAsync(pair.UserId);
            await _manager.AddToRoleAsync(userId, pair.RoleName);

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Admin/RemoveRole")]
        public async Task RemoveRole(UserRolePairModel pair)
        {
            var userId = await _manager.FindByIdAsync(pair.UserId);
            await _manager.RemoveFromRoleAsync(userId, pair.RoleName);
        }

    }
}
