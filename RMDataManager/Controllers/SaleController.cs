using Microsoft.AspNet.Identity;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;
using RMDataManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RMDataManager.Controllers
{
    [Authorize]
    public class SaleController : ApiController
    {
        [Authorize(Roles = "Cashier")]
        public void Post(SaleModel sale)
        {
            SaleData data = new SaleData();
            var userId = User.Identity.GetUserId();
            
            data.SaveSale(sale, userId);
        }

        [Route("GetSaleReports")]
        [Authorize(Roles = "Manager, Admin")]
        public List<SaleReportModel> GetSaleReports()
        {
            SaleData data = new SaleData();

            return data.GetSaleReports();
        }
    }
}
