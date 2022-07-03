using System;
using System.Linq;
using System.Web.Mvc;
using BLL.Repositories;
using ClothesShop.Enums;
using ClothesShop.Infrastructure;
using ClothesShop.Models;
using DAL;

namespace ClothesShop.Controllers
{
    [Authentication]
    public class ProductsSummaryController : Controller
    {
        private readonly ProductsRepo _ProductsRepo;
        public ProductsSummaryController()
        {
            _ProductsRepo = new ProductsRepo();
        }
        // GET: ProductsSummary
        [Authorization("ProductsSummary", (RoleType.View))]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorization("ProductsSummary", (RoleType.View))]
        public ActionResult GetAll(SearchViewModel obj)
        {
            try
            {
                var pageIndex = obj.PageNumber > 0 ? obj.PageNumber - 1 : 0;
                var pageSize = obj.PageSize;

                var products = _ProductsRepo.GetAll().GroupBy(b => b.ID).Select(p => p.FirstOrDefault());
                var result = products.Select(n => GetProductSummaryViewModel(n));
                Filtering<ProductsSummaryViewModel> filtering = new Filtering<ProductsSummaryViewModel>();
                filtering.Columns = obj.FilteredColumns;

                result = filtering.Search(result);

                //Sorting    
                result = filtering.OrderBy(obj.OrderBy, result);

                int totalRecords = result.Count();

                var data = result.Skip(pageIndex * pageSize).Take(pageSize).ToList();

                return Json(new { TotalCount = totalRecords, Data = data });
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
                return Json(new { TotalCount = 0, Data = string.Empty});
            }
        }
        private ProductsSummaryViewModel GetProductSummaryViewModel(Product p)
        {
            try
            {
                var supplier = p.ProductSuppliers.Any(pp => pp.TransactionTypeID == (int)TransactionTypes.Incoming) ?
                    p.ProductSuppliers.Where(pp => pp.TransactionTypeID == (int)TransactionTypes.Incoming).OrderByDescending(pp => pp.CreatedOn).FirstOrDefault() : null;
                return new ProductsSummaryViewModel()
                {
                    ProductName = p.Name,
                    OriginalPrice = supplier != null ? supplier.OrginalPrice.Value : 0,
                    TotalIncoming = p.ProductSuppliers.Where(pp => pp.TransactionTypeID == (int)TransactionTypes.Incoming).Sum(x => x.NumberOfPieces).Value,
                    TotalReturned = p.ProductSuppliers.Where(pp => pp.TransactionTypeID == (int)TransactionTypes.Returned || pp.TransactionTypeID == (int)TransactionTypes.Lossses).Sum(x => x.NumberOfPieces).Value,
                    TotalSelled = p.Transactions.Sum(t=>t.NumberOfPieces).Value,
                };
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
                return null;
            }

        }
    }
}