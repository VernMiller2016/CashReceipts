using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CashReceipts.Filters;
using CashReceipts.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using ServiceStack;

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
                .Where(x => !x.IsDeleted)
                .Where(x => SqlFunctions.DateDiff("DAY", x.ReceiptDate, date) == 0)
                .OrderBy(x => x.ReceiptNumber)
                .ToList()
                .Select(x => new { x.ReceiptHeaderID, DepartmentId = x.DepartmentID, ReceiptNumber = x.ReceiptNumber, DepartmentName = x.Department.Name, Total = x.ReceiptTotal }).ToList();

            return Json(summaryData.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult TendersSummary_Read([DataSourceRequest] DataSourceRequest request, DateTime date)
        {
            var summaryData = _db.ReceiptHeaders
                .Include(x => x.Tenders)
                .Include(x => x.Tenders.Select(y => y.PaymentMethod))
                .Where(x => !x.IsDeleted)
                .Where(x => SqlFunctions.DateDiff("DAY", x.ReceiptDate, date) == 0)
                .SelectMany(x => x.Tenders).ToList();

            return Json(summaryData.GroupBy(x => x.PaymentMethodId).Select(x => new
            {
                PaymentMethodId = x.Key,
                PaymentMethod = x.First().PaymentMethod.Name,
                TotalAmount = x.Sum(y => y.Amount)
            }).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }

        public ActionResult ReceiptsExport()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DownloadReceipts(string StartDate, string EndDate)
        {
            DateTime startDate = DateTime.ParseExact(StartDate, "MM/d/yyyy", CultureInfo.InvariantCulture),
                endDate = DateTime.ParseExact(EndDate, "MM/d/yyyy", CultureInfo.InvariantCulture);

            var query = @"SELECT 
             [receiptnumber]                 AS [ReceiptNumber], 
             FORMAT([receiptdate], 'MM/d/yyyy')    AS [ReceiptDate], 
             d.NAME                          AS [Department], 
             c.lastname + ', ' + c.firstname AS [Clerk], 
             [receipttotal]                  AS [ReceiptTotal], 
             COALESCE([comments], '')        AS [ReceivedFrom], 
             COALESCE([receivedfor], '')     AS [ReceivedFor] 
             FROM   [dbo].[receiptheaders] rh 
             INNER JOIN dbo.clerks c 
                     ON c.clerkid = rh.clerkid 
             INNER JOIN dbo.departments d 
                     ON rh.departmentid = d.departmentid 
             WHERE  rh.isdeleted = 0 
             AND CONVERT(DATE, rh.receiptdate) BETWEEN @StartDate AND @EndDate";
            var result = _db.Database.SqlQuery<ReceiptCsv>(query,
                new SqlParameter { ParameterName = "@StartDate", Value = startDate },
                new SqlParameter { ParameterName = "@EndDate", Value = endDate }
                ).ToList();
            return File(Encoding.UTF8.GetBytes(result.ToCsv()), "text/csv", "Receipts.csv");
        }

        [HttpPost]
        public ActionResult DownloadLineItems(string StartDate, string EndDate)
        {
            var query = @"SELECT 
             rh.receiptnumber                  AS [ReceiptNumber], 
             t.fund + '.' + t.dept + '.' + t.program + '.' + t.project 
             + '.' + t.baseelementobjectdetail AS [AccountNumber], 
             rb.linetotal                      AS [LineTotal], 
             rb.accountdescription             AS [Template] 
             FROM   dbo.receiptbodies rb 
             INNER JOIN dbo.templates t 
                     ON t.templateid = rb.templateid 
             INNER JOIN dbo.receiptheaders rh 
                     ON rh.receiptheaderid = rb.receiptheaderid 
             WHERE  rh.isdeleted = 0 
             AND CONVERT(DATE, rh.receiptdate) BETWEEN @startDate AND @endDate 
             ORDER  BY rh.receiptnumber, [AccountNumber] 
";
            var result = _db.Database.SqlQuery<LineItemCsv>(query,
                new SqlParameter { ParameterName = "@StartDate", Value = DateTime.ParseExact(StartDate, "MM/d/yyyy", CultureInfo.InvariantCulture) },
                new SqlParameter { ParameterName = "@EndDate", Value = DateTime.ParseExact(EndDate, "MM/d/yyyy", CultureInfo.InvariantCulture) }
                ).ToList();
            return File(Encoding.UTF8.GetBytes(result.ToCsv()), "text/csv", "LineItems.csv");
        }

        [HttpPost]
        public ActionResult DownloadTenders(string StartDate, string EndDate)
        {
            var query = @"SELECT 
             rh.ReceiptNumber, 
             pm.NAME as Name, 
             t.[Description], 
             t.Amount 
             FROM   dbo.tenders t 
             INNER JOIN dbo.receiptheaders rh 
                     ON rh.receiptheaderid = t.receiptheaderid 
             INNER JOIN [dbo].[paymentmethods] pm 
                     ON pm.id = t.paymentmethodid 
             WHERE  rh.isdeleted = 0 
             AND CONVERT(DATE, rh.receiptdate) BETWEEN @startDate AND @endDate 
             ORDER  BY rh.receiptnumber 
";
            var result = _db.Database.SqlQuery<TenderCsv>(query,
                new SqlParameter { ParameterName = "@StartDate", Value = DateTime.ParseExact(StartDate, "MM/d/yyyy", CultureInfo.InvariantCulture) },
                new SqlParameter { ParameterName = "@EndDate", Value = DateTime.ParseExact(EndDate, "MM/d/yyyy", CultureInfo.InvariantCulture) }
                ).ToList();
            return File(Encoding.UTF8.GetBytes(result.ToCsv()), "text/csv", "Tenders.csv");
        }
    }

    public class ExpotParams
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class ReceiptCsv
    {
        public int ReceiptNumber { get; set; }
        public string ReceiptDate { get; set; }
        public string Department { get; set; }
        public string Clerk { get; set; }
        public decimal ReceiptTotal { get; set; }
        public string ReceivedFrom { get; set; }
        public string ReceivedFor { get; set; }
    }

    public class LineItemCsv
    {
        public int ReceiptNumber { get; set; }
        public string AccountNumber { get; set; }
        public decimal LineTotal { get; set; }
        public string Template { get; set; }
    }

    public class TenderCsv
    {
        public int ReceiptNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }

}