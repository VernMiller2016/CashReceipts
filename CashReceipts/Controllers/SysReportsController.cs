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
using CashReceipts.ViewModels;
using ServiceStack;
using CashReceipts.Helpers;

namespace CashReceipts.Controllers
{
    [Authorize]
    public class SysReportsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        private readonly AccessHelper _access;

        public SysReportsController()
        {
            _access = new AccessHelper();
        }

        [CanAccess((int)FeaturePermissions.DaySummaryReportIndex)]
        public ActionResult SummaryReport()
        {
            ViewBag.HasExportAccess = _access.UserFeatures.FirstOrDefault(f => f.FeatureId == (int)FeaturePermissions.ExportAndPrintSummary) != null;
            ViewBag.HasLockReceiptsAccess = _access.UserFeatures.FirstOrDefault(f => f.FeatureId == (int)FeaturePermissions.LockReceipts) != null;
            return View();
        }

        [NoCache]
        public ActionResult DepartmentsSummary_Read([DataSourceRequest] DataSourceRequest request, DateTime? startDate, DateTime? endDate, int? clerkId)
        {
            var summaryData = _db.ReceiptHeaders.Include(x => x.Department)
                .Where(x => !x.IsDeleted && (!clerkId.HasValue || x.ClerkID == clerkId))
                .Where(x => !startDate.HasValue || SqlFunctions.DateDiff("DAY", x.ReceiptDate, startDate) <= 0)
                .Where(x => !endDate.HasValue || SqlFunctions.DateDiff("DAY", x.ReceiptDate, endDate) >= 0)
                .OrderBy(x => x.ReceiptNumber)
                .ToList()
                .Select(
                    x =>
                        new
                        {
                            x.ReceiptHeaderID,
                            DepartmentId = x.DepartmentID,
                            ReceiptNumber = x.ReceiptNumber,
                            DepartmentName = x.Department.Name,
                            Total = x.ReceiptTotal,
                            Locked = x.IsPosted ? "Yes" : "No"
                        }).ToList();

            return Json(summaryData.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult TendersSummary_Read([DataSourceRequest] DataSourceRequest request, DateTime? startDate, DateTime? endDate, int? clerkId)
        {
            var summaryData = _db.ReceiptHeaders
                .Include(x => x.Tenders)
                .Include(x => x.Tenders.Select(y => y.PaymentMethod))
                .Where(x => !x.IsDeleted && (!clerkId.HasValue || x.ClerkID == clerkId))
                .Where(x => !startDate.HasValue || SqlFunctions.DateDiff("DAY", x.ReceiptDate, startDate) <= 0)
                .Where(x => !endDate.HasValue || SqlFunctions.DateDiff("DAY", x.ReceiptDate, endDate) >= 0)
                .SelectMany(x => x.Tenders).ToList();

            return Json(summaryData.GroupBy(x => x.PaymentMethodId).Select(x => new
            {
                PaymentMethodId = x.Key,
                PaymentMethod = x.First().PaymentMethod.Name,
                TotalAmount = x.Sum(y => y.Amount)
            }).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }

        [CanAccess((int)FeaturePermissions.ReceiptsExportIndex)]
        public ActionResult ReceiptsExport()
        {
            ViewBag.isExport = _access.UserFeatures.FirstOrDefault(f => f.FeatureId == (int)FeaturePermissions.ReceiptsExport) != null;
            ViewBag.isExportLineItems = _access.UserFeatures.FirstOrDefault(f => f.FeatureId == (int)FeaturePermissions.LineItemsExport) != null;
            ViewBag.isExportTenders = _access.UserFeatures.FirstOrDefault(f => f.FeatureId == (int)FeaturePermissions.TendersExport) != null;
            ViewBag.isExportAll = _access.UserFeatures.FirstOrDefault(f => f.FeatureId == (int)FeaturePermissions.ReceiptsDetailsExport) != null;
            return View();
        }


        [HttpPost]
        public ActionResult DownloadReceipts(string StartDate, string EndDate)
        {
            DateTime startDate = DateTime.ParseExact(StartDate, "M/d/yyyy", null),
                endDate = DateTime.ParseExact(EndDate, "M/d/yyyy", null);

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
             WHERE  rh.isdeleted = 0 and rb.linetotal != 0
             AND CONVERT(DATE, rh.receiptdate) BETWEEN @startDate AND @endDate 
             ORDER  BY rh.receiptnumber, [AccountNumber] 
";
            var result = _db.Database.SqlQuery<LineItemCsv>(query,
                new SqlParameter { ParameterName = "@StartDate", Value = DateTime.ParseExact(StartDate, "M/d/yyyy", CultureInfo.InvariantCulture) },
                new SqlParameter { ParameterName = "@EndDate", Value = DateTime.ParseExact(EndDate, "M/d/yyyy", CultureInfo.InvariantCulture) }
                ).ToList();
            return File(Encoding.UTF8.GetBytes(result.ToCsv()), "text/csv", "LineItems.csv");
        }

        [HttpPost]
        public ActionResult DownloadTenders(string StartDate, string EndDate)
        {
            var query = @"SELECT 
             rh.ReceiptNumber, 
             pm.NAME as PaymentMethod, 
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
                new SqlParameter { ParameterName = "@StartDate", Value = DateTime.ParseExact(StartDate, "M/d/yyyy", CultureInfo.InvariantCulture) },
                new SqlParameter { ParameterName = "@EndDate", Value = DateTime.ParseExact(EndDate, "M/d/yyyy", CultureInfo.InvariantCulture) }
                ).ToList();
            return File(Encoding.UTF8.GetBytes(result.ToCsv()), "text/csv", "Tenders.csv");
        }

        [HttpPost]
        public ActionResult DownloadAll(string StartDate, string EndDate)
        {
            var query = @"SELECT 
             [receiptnumber]                 AS [ReceiptNumber], 
             FORMAT([receiptdate], 'MM/d/yyyy')    AS [ReceiptDate], 
             d.NAME                          AS [Department], 
             c.lastname + ', ' + c.firstname AS [Clerk], 
             [receipttotal]                  AS [ReceiptTotal], 
             COALESCE([comments], '')        AS [ReceivedFrom], 
             COALESCE([receivedfor], '')     AS [ReceivedFor],
             t.fund + '.' + t.dept + '.' + t.program + '.' + t.project 
             + '.' + t.baseelementobjectdetail AS [AccountNumber], 
             rb.linetotal                      AS [LineTotal], 
             rb.accountdescription             AS [Template],
             pm.NAME as PaymentMethod, 
             tenders.[Description], 
             tenders.Amount 
			 FROM   [dbo].[receiptheaders] rh 
             INNER JOIN dbo.clerks c 
                     ON c.clerkid = rh.clerkid 
             INNER JOIN dbo.departments d 
                     ON rh.departmentid = d.departmentid 
			 INNER JOIN dbo.receiptbodies rb 
                     ON rh.receiptheaderid = rb.receiptheaderid 
             INNER JOIN dbo.templates t 
                     ON t.templateid = rb.templateid 
			 INNER JOIN dbo.tenders 
                     ON rh.receiptheaderid = Tenders.receiptheaderid 
             INNER JOIN [dbo].[paymentmethods] pm 
                     ON pm.id = tenders.paymentmethodid 
             WHERE  rh.isdeleted = 0 and rb.linetotal != 0
			 AND CONVERT(DATE, rh.receiptdate) BETWEEN @StartDate AND @EndDate
			 ORDER  BY rh.receiptnumber, [AccountNumber] 
            ";
            var result = _db.Database.SqlQuery<ReceiptDetailsCsv>(query,
                new SqlParameter { ParameterName = "@StartDate", Value = DateTime.ParseExact(StartDate, "M/d/yyyy", CultureInfo.InvariantCulture) },
                new SqlParameter { ParameterName = "@EndDate", Value = DateTime.ParseExact(EndDate, "M/d/yyyy", CultureInfo.InvariantCulture) }
                ).ToList();
            return File(Encoding.UTF8.GetBytes(result.ToCsv()), "text/csv", "ReceiptsWithDetails.csv");
        }

        public ActionResult GetAllClerks()
        {
            var data = _db.Clerks.Select(x => new {Id = x.ClerkID, ClerkName = x.LastName + ", " + x.FirstName}).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}