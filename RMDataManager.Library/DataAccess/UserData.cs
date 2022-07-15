using Dapper;
using Microsoft.Extensions.Configuration;
using RMDataManager.Library.Internal.DataAccess;
using RMDataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDataManager.Library.DataAccess
{
    public class UserData
    {
        private readonly IConfiguration _config;

        public UserData(IConfiguration config)
        {
            _config = config;
        }
        public List<UserModel> GetUserById(string Id) 
        {
            List<UserModel> output;
            SqlDataAccess sql = new SqlDataAccess(_config);

            // TODO defaultConnection --> TestConnection 
            // TODO in Web.config added TestConnection
            output = sql.LoadData<UserModel, dynamic>("dbo.spUser_Lookup", new { Id }, "RMData");

            return output;
        }
    }
}
