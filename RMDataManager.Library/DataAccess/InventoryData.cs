﻿using RMDataManager.Library.Internal.DataAccess;
using RMDataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDataManager.Library.DataAccess
{
    public class InventoryData
    {
        public List<InventoryModel> GetInventory()
        {
            SqlDataAccess sql = new SqlDataAccess();

            var output = sql.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", new { }, "RMDAta");

            return output;
        }

        public void SaveInventoryRecord(InventoryModel inventory)
        {
            SqlDataAccess sql = new SqlDataAccess();
            var parameter = new
            {
                ProductId = inventory.ProductId,
                Quantity = inventory.Quantity,
                PurchasePrice = inventory.PurchasePrice,
                PurchaseDate = inventory.PurchaseDate
            };
            sql.SaveData("dbo.spInventory_Insert", parameter, "RMData");
        }
    }
}