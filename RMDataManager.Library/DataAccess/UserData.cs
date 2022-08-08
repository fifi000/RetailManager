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
    public class UserData : IUserData
    {
        private readonly ISqlDataAccess _sql;

        public UserData(ISqlDataAccess sql)
        {
            _sql = sql;
        }
        public List<UserModel> GetUserById(string Id)
        {
            List<UserModel> output;

            // TODO defaultConnection --> TestConnection 
            // TODO in Web.config added TestConnection
            output = _sql.LoadData<UserModel, dynamic>("dbo.spUser_Lookup", new { Id }, "RMData");

            return output;
        }
    }
}
