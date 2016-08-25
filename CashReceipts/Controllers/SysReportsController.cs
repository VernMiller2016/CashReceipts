using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CashReceipts.Filters;
using CashReceipts.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace CashReceipts.Controllers
{
    [Authorize]
    public class SysReportsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public ActionResult SummaryReport()
        {
            return View();
        }

        [NoCache]
        public ActionResult DepartmentsSummary_Read([DataSourceRequest] DataSourceRequest request, DateTime date)
        {
            var summaryData = _db.ReceiptHeaders.Include(x => x.Department)
                .Where(x=> SqlFunctions.DateDiff("DAY", x.ReceiptDate, date) == 0)
                .GroupBy(x=>x.Department.DepartmentID)
                .Select(x => new { DepartmentId = x.Key, ReceiptNumber = x.FirstOrDefault().ReceiptNumber, DepartmentName = x.FirstOrDefault().Department.Name, Total = x.Sum(y => y.ReceiptTotal)}).ToList();

            return Json(summaryData.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult TendersSummary_Read([DataSourceRequest] DataSourceRequest request, DateTime date)
        {
            var summaryData = _db.ReceiptHeaders
                .Include(x => x.Tenders)
                .Include(x => x.Tenders.Select(y=>y.PaymentMethod))
                .Where(x => SqlFunctions.DateDiff("DAY", x.ReceiptDate, date) == 0)
                .SelectMany(x => x.Tenders).ToList();

            return Json(summaryData.GroupBy(x=>x.PaymentMethodId).Select(x=>new {PaymentMethodId = x.Key,
                PaymentMethod = x.First().PaymentMethod.Name, TotalAmount = x.Sum(y=>y.Amount)}).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }
    }
}