using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;
using RMDataManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace RMDataManager.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {
        [HttpGet]
        public UserModel GetById()
        {
            string userId = RequestContext.Principal.Identity.GetUserId();
            UserData data = new UserData();

            return data.GetUserById(userId).First();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("Api/User/Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            var output = new List<ApplicationUserModel>();

            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var users = userManager.Users.ToList();
                var roles = context.Roles.ToList();

                foreach (var user in users)
                {
                    var u = new ApplicationUserModel
                    {
                        Id = user.Id,
                        EmailAddress = user.Email,
                    };

                    foreach (var r in user.Roles)
                    {
                        u.Roles.Add(r.RoleId, roles.First(x => x.Id == r.RoleId).Name);
                    }

                    output.Add(u);
                }
            }

            return output;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("Api/User/Admin/GetAllRoles")]
        public Dictionary<string, string> GetAllRols()
        {
            using (var context = new ApplicationDbContext())
            {
                var roles = context.Roles.ToDictionary(x => x.Id, x => x.Name);

                return roles;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Api/User/Admin/AddRole")]
        public void AddRole(UserRolePairModel pair)
        {
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                userManager.AddToRole(pair.UserId, pair.RoleName);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Api/User/Admin/RemoveRole")]
        public void RemoveRole(UserRolePairModel pair)
        {
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                userManager.RemoveFromRole(pair.UserId, pair.RoleName);
            }

        }
    }
}
