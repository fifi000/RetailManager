﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDesktopUI.Library.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string EmailAddress { get; set; }
        public Dictionary<string, string> Roles { get; set; } = new Dictionary<string, string>();
        public string RolesList
        {
            get 
            {
                string output = String.Join(", ", Roles.Select(x => x.Value));
                return output;

            }
        }

    }
}
