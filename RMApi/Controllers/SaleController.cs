using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;
using System.Security.Claims;

namespace RMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaleController : ControllerBase
    {
        private readonly IConfiguration _config;

        public SaleController(IConfiguration config)
        {
            _config = config;
        }

        [Authorize(Roles = "Cashier")]
        public void Post(SaleModel sale)
        {
            SaleData data = new SaleData(_config);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); ;

            data.SaveSale(sale, userId);
        }

        [Route("GetSaleReports")]
        [Authorize(Roles = "Manager, Admin")]
        public List<SaleReportModel> GetSaleReports()
        {
            SaleData data = new SaleData(_config);

            return data.GetSaleReports();
        }
    }

}

