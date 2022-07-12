using Microsoft.AspNet.Identity;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RMDataManager.Controllers
{
    [Authorize]
    public class InventoryController : ApiController
    {
        [HttpPost]
        public void Post(InventoryModel inventory)
        {
            InventoryData data = new InventoryData();
            var userId = User.Identity.GetUserId();

            data.SaveInventoryRecord(inventory);
        }

        public List<InventoryModel> Get()
        {
            InventoryData data = new InventoryData();

            return data.GetInventory();
        }
    }
}
