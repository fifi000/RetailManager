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
    public class InventoryData : IInventoryData
    {
        private readonly ISqlDataAccess _sql;

        public InventoryData(ISqlDataAccess sql)
        {
            _sql = sql;
        }
        public List<InventoryModel> GetInventory()
        {
            var output = _sql.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", new { }, "RMDAta");

            return output;
        }

        public void SaveInventoryRecord(InventoryModel inventory)
        {
            var parameter = new
            {
                ProductId = inventory.ProductId,
                Quantity = inventory.Quantity,
                PurchasePrice = inventory.PurchasePrice,
                PurchaseDate = inventory.PurchaseDate
            };
            _sql.SaveData("dbo.spInventory_Insert", parameter, "RMData");
        }
    }
}