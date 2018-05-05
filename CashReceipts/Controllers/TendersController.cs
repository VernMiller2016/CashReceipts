using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CashReceipts.Filters;
using CashReceipts.Models;
using CashReceipts.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace CashReceipts.Controllers
{
    [Authorize]
    public class TendersController : BaseController
    {
        // GET: Tenders
        public ActionResult Index()
        {
            return View(_db.Tenders.ToList());
        }

        // GET: Tenders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tender tender = _db.Tenders.Find(id);
            if (tender == null)
            {
                return HttpNotFound();
            }
            return View(tender);
        }

        // GET: Tenders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tenders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TenderID,Description,Amount")] Tender tender)
        {
            if (ModelState.IsValid)
            {
                _db.Tenders.Add(tender);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tender);
        }

        // GET: Tenders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tender tender = _db.Tenders.Find(id);
            if (tender == null)
            {
                return HttpNotFound();
            }
            return View(tender);
        }

        // POST: Tenders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TenderID,Description,Amount")] Tender tender)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(tender).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tender);
        }

        // GET: Tenders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tender tender = _db.Tenders.Find(id);
            if (tender == null)
            {
                return HttpNotFound();
            }
            return View(tender);
        }

        // POST: Tenders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tender tender = _db.Tenders.Find(id);
            _db.Tenders.Remove(tender);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [CanAccess((int)FeaturePermissions.SearchTenders)]
        public ActionResult Search()
        {
            var permissions = new Dictionary<string, bool>
            {
                {"hasExportPermission", HasAccess(FeaturePermissions.ExportTenders)},
            };
            ViewBag.Permissions = permissions;
            return View();
        }

        [NoCache]
        public ActionResult Tenders_Read([DataSourceRequest] DataSourceRequest request, DateTime? fromDate = null, DateTime? toDate = null, decimal? amount = null, int? tenderType = null)
        {
            var receiptBodies = _db.Tenders
                .Include(x => x.ReceiptHeader)
                .Include(x => x.ReceiptHeader.Department)
                .Where(x => !amount.HasValue || x.Amount == amount)
                .Where(x => !tenderType.HasValue || x.PaymentMethodId == tenderType)
                .Where(x => !fromDate.HasValue || SqlFunctions.DateDiff("DAY", x.ReceiptHeader.ReceiptDate, fromDate) <= 0)
                .Where(x => !toDate.HasValue || SqlFunctions.DateDiff("DAY", x.ReceiptHeader.ReceiptDate, toDate) >= 0)
                .ToList()
                .Select(x => new
                {
                    x.ReceiptHeaderID,
                    x.ReceiptHeader.ReceiptDate,
                    ReceiptHeaderNumber = x.ReceiptHeader.ReceiptNumber,
                    ReceiptDepartment = x.ReceiptHeader.Department.Name,
                    x.PaymentMethodId,
                    x.Amount,
                    x.Description,
                    x.TenderID
                }).ToList();
            return Json(receiptBodies.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}
