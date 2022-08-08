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
        private readonly ISaleData _saleData;

        public SaleController(ISaleData saleData)
        {
            _saleData = saleData;
        }

        [Authorize(Roles = "Cashier")]
        [HttpPost]
        public void Post(SaleModel sale)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); ;

            _saleData.SaveSale(sale, userId);
        }

        [Route("GetSaleReports")]
        [HttpGet]
        [Authorize(Roles = "Manager, Admin")]
        public List<SaleReportModel> GetSaleReports()
        {
            return _saleData.GetSaleReports();
        }
    }

}

