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
    public class SaleData
    {
        private readonly IConfiguration _config;

        public SaleData(IConfiguration config)
        {
            _config = config;
        }
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            // TODO - make this code better (eg create a method that calculates the tax and total)

            // fill in the sale detail db models 
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
            ProductData productData = new ProductData(_config);
            decimal taxRate = (decimal)ConfigHelper.GetTaxRate() / 100;

            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    
                };

                ProductModel p = productData.GetProductById(detail.ProductId);

                if (p == null)
                {
                    throw new Exception($"The product Id of {detail.ProductId} could not be found in the database.");
                }

                detail.PurchasePrice = p.RetailPrice * detail.Quantity;

                if (p.IsTaxable)
                {
                    detail.Tax = detail.PurchasePrice * taxRate;
                }
                else
                {
                    detail.Tax = 0;
                }

                details.Add(detail);
            }

            // Create a sale model
            SaleDBModel sale = new SaleDBModel
            {
                CashierId = cashierId,
                SubTotal = details.Sum(x => x.PurchasePrice),
                Tax = details.Sum(x => x.Tax)
            };
            sale.Total = sale.SubTotal + sale.Tax;

            using (SqlDataAccess sql = new SqlDataAccess(_config))
            {
                try
                {
                    sql.StartTransaction("RMData");

                    // Save sale record
                    // get the sale id
                    sale.Id = sql.LoadDataInTransaction<int, object>("dbo.spSale_Insert", sale).First();

                    // Finish filling details data - sale id
                    // Save sale details
                    foreach (var detail in details)
                    {
                        detail.SaleId = sale.Id;
                        sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", detail);
                    }
                }
                catch
                {
                    sql.RollBackTransaction();
                    throw new OperationCanceledException();
                }
            }
        }

        public List<SaleReportModel> GetSaleReports()
        {
            SqlDataAccess sql = new SqlDataAccess(_config);

            var output = sql.LoadData<SaleReportModel, object>("dbo.spSale_SaleReport", new { }, "RMData");

            return output;
        }
    }
}
