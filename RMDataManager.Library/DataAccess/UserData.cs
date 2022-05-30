using Dapper;
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
        public List<UserModel> GetUserById(string Id) 
        {
            List<UserModel> output;
            SqlDataAccess sql = new SqlDataAccess();

            var p = new { Id = Id };

            // TODO defaultConnection --> TestConnection 
            // TODO in Web.config added TestConnection
            output = sql.LoadData<UserModel, dynamic>("dbo.spUserLookup", p, "RMData");

            return output;
        }
    }
}
