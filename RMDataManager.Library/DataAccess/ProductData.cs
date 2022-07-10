using RMDataManager.Library.Internal.DataAccess;
using RMDataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDataManager.Library.DataAccess
{
    public class ProductData
    {
       public List<ProductModel> GetProducts()
        {
            List<ProductModel> output;
            SqlDataAccess sql = new SqlDataAccess();

            output = sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetAll", new { }, "RMDAta");

            return output;

        }
    }
}
