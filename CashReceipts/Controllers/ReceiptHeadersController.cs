using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CashReceipts.Models;
using CashReceipts.Filters;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using CashReceipts.Helpers;
using System.Data.Entity.Infrastructure;

namespace CashReceipts.Controllers
{
    public class ReceiptHeadersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private LookupHelper _lookupHelper;

        public ReceiptHeadersController()
        {
            _lookupHelper = new LookupHelper(db);
        }

        // GET: ReceiptHeaders
        public ActionResult Index()
        {
            return View();
        }

        // GET: ReceiptHeaders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiptHeader receiptHeader = db.ReceiptHeaders.Include(x => x.Tenders).Include(x => x.ReceiptBodyRecords).FirstOrDefault(x => x.ReceiptHeaderID == id);
            if (receiptHeader == null)
            {
                return HttpNotFound();
            }
            return View(receiptHeader);
        }

        // GET: ReceiptHeaders/Create
        public ActionResult Create()
        {
            ViewBag.ClerkID = new SelectList(db.Clerks, "ClerkID", "FirstName");
            return View();
        }

        // POST: ReceiptHeaders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReceiptHeaderID,ReceiptDate,ReceiptTotal,ClerkID")] ReceiptHeader receiptHeader)
        {
            if (ModelState.IsValid)
            {
                db.ReceiptHeaders.Add(receiptHeader);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClerkID = new SelectList(db.Clerks, "ClerkID", "FirstName", receiptHeader.ClerkID);
            return View(receiptHeader);
        }

        // GET: ReceiptHeaders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiptHeader receiptHeader = db.ReceiptHeaders.Include(x => x.Tenders).Include(x => x.ReceiptBodyRecords).FirstOrDefault(x => x.ReceiptHeaderID == id);
            if (receiptHeader == null)
            {
                return HttpNotFound();
            }
            TempData["HeaderId"] = id;
            ViewBag.ClerkID = new SelectList(db.Clerks, "ClerkID", "FirstName", receiptHeader.ClerkID);
            return View(receiptHeader);
        }

        // POST: ReceiptHeaders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReceiptHeaderID,ReceiptDate,ReceiptTotal,ClerkID")] ReceiptHeader receiptHeader)
        {
            if (ModelState.IsValid)
            {
                db.Entry(receiptHeader).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClerkID = new SelectList(db.Clerks, "ClerkID", "FirstName", receiptHeader.ClerkID);
            return View(receiptHeader);
        }

        // GET: ReceiptHeaders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiptHeader receiptHeader = db.ReceiptHeaders.Find(id);
            if (receiptHeader == null)
            {
                return HttpNotFound();
            }
            return View(receiptHeader);
        }

        // POST: ReceiptHeaders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReceiptHeader receiptHeader = db.ReceiptHeaders.Find(id);
            db.ReceiptHeaders.Remove(receiptHeader);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AddReceiptBody(int id)
        {
            ReceiptBody receiptbody = new ReceiptBody();
            receiptbody.ReceiptHeaderID = id;
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentID", "Name");
            ViewBag.TemplateID = new SelectList(db.Templates, "TemplateID", "Description");
            return View(receiptbody);
        }

        [HttpPost, ActionName("AddReceiptBody")]
        public ActionResult AddReceiptBody(ReceiptBody receiptbody)
        {
            if (ModelState.IsValid)
            {
                db.ReceiptBodies.Add(receiptbody);
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = receiptbody.ReceiptHeaderID });
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentID", "Name");
            ViewBag.TemplateID = new SelectList(db.Templates, "TemplateID", "Description", receiptbody.TemplateID);
            return View(receiptbody);
        }

        public ActionResult AddTenders(int id)
        {
            Tender tenders = new Tender();
            tenders.ReceiptHeaderID = id;
            return View(tenders);
        }

        [HttpPost, ActionName("AddTenders")]
        public ActionResult AddTenders(Tender tenders)
        {
            if (ModelState.IsValid)
            {
                db.Tenders.Add(tenders);
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = tenders.ReceiptHeaderID });
            }
            return View(tenders);
        }

        [NoCache]
        public ActionResult GetDepartmentTemplates(int id)
        {
            var templates = db.Templates.Where(x => x.DepartmentID == id).Select(x => new { TemplateId = x.TemplateID, TemplateName = x.Description }).ToList();
            return Json(templates, JsonRequestBehavior.AllowGet);
        }

        #region Receipt Header Grid Actions
        [NoCache]
        public ActionResult GetDepartmentsList()
        {
            var departmentsList = db.Departments.ToList()
                .Select(x => new { value = x.DepartmentID, text = x.Name }).ToList();
            return Json(departmentsList, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult GetClerksList()
        {
            var clerksList = db.Clerks
                .Select(x => new { value = x.ClerkID, text = x.LastName + ", " + x.FirstName }).ToList();
            return Json(clerksList, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult ReceiptHeaders_Read([DataSourceRequest] DataSourceRequest request)
        {
            var receiptHeaders = db.ReceiptHeaders
                .Select(x => new { x.ReceiptHeaderID, x.ClerkID, x.ReceiptDate, x.ReceiptTotal, x.ReceiptNumber, x.DepartmentID }).ToList();
            return Json(receiptHeaders.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ReceiptHeaders_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<ReceiptHeader> receiptHeaders)
        {
            var lastReciptId = _lookupHelper.LastReceiptId;
            var receiptHeadersList = receiptHeaders as List<ReceiptHeader> ?? receiptHeaders.ToList();
            if (receiptHeadersList.Any() && ModelState.IsValid)
            {
                foreach (var receiptHeader in receiptHeadersList)
                {
                    if (db.ReceiptHeaders.Any(x => x.ReceiptNumber == lastReciptId + 1))
                    {
                        ModelState.AddModelError("_addKey", "A receipt with same number is found in database!");
                        break;
                    }
                    receiptHeader.ReceiptNumber = lastReciptId + 1;
                    _lookupHelper.LastReceiptId = receiptHeader.ReceiptNumber;
                    var templatesList = db.Templates.Where(x => x.DepartmentID == receiptHeader.DepartmentID).ToList();
                    foreach (var template in templatesList)
                    {
                        receiptHeader.ReceiptBodyRecords.Add(new ReceiptBody
                        {
                            LineTotal = 0,
                            TemplateID = template.TemplateID
                        });
                    }
                    db.ReceiptHeaders.Add(receiptHeader);
                    try
                    {
                        if (db.SaveChanges() <= 0)
                        {
                            //todo supports localization
                            ModelState.AddModelError("_addKey", "Can't add this receipt header to database");
                        }
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        ModelState.AddModelError("_addKey", "A receipt with same number is found in database!");
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("_addKey", "Can't add this receipt header to database");
                    }
                }
            }

            return Json(receiptHeadersList.Select(
                    x => new { x.ReceiptHeaderID, x.ClerkID, x.ReceiptDate, x.ReceiptTotal, x.ReceiptNumber, x.DepartmentID }).ToList().ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult ReceiptHeaders_Update([DataSourceRequest] DataSourceRequest request, IEnumerable<ReceiptHeader> receiptHeaders)
        {
            var receiptHeadersList = receiptHeaders as List<ReceiptHeader> ?? receiptHeaders.ToList();
            if (receiptHeadersList.Any() && ModelState.IsValid)
            {
                foreach (var receiptHeader in receiptHeadersList)
                {
                    db.Entry(receiptHeader).State = EntityState.Modified;
                    try
                    {
                        if (db.SaveChanges() <= 0)
                        {
                            ModelState.AddModelError("_updateKey", "Can't update this receipt header to database");
                        }
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("_updateKey", "Can't update this receipt header to database");
                    }
                }
            }
            return Json(receiptHeadersList.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult ReceiptHeaders_Destroy([DataSourceRequest] DataSourceRequest request, IEnumerable<ReceiptHeader> receiptHeaders)
        {
            var receiptHeadersList = receiptHeaders as List<ReceiptHeader> ?? receiptHeaders.ToList();
            if (receiptHeadersList.Any() && ModelState.IsValid)
            {
                foreach (var receiptHeader in receiptHeadersList)
                {
                    var receiptHeaderInDb = db.ReceiptHeaders.SingleOrDefault(x => x.ReceiptHeaderID == receiptHeader.ReceiptHeaderID);
                    if (receiptHeaderInDb != null)
                    {
                        db.ReceiptHeaders.Remove(receiptHeaderInDb);
                        try
                        {
                            if (db.SaveChanges() <= 0)
                            {
                                ModelState.AddModelError("_deleteKey", "Can't remove this receipt header from database");
                            }
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("_deleteKey", "Can't remove this receipt header from database");
                        }
                    }
                }
            }
            return Json(receiptHeadersList.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Receipt Body Grid Actions
        [NoCache]
        public ActionResult Departments_Read([DataSourceRequest] DataSourceRequest request)
        {
            var departmentsList = db.Departments.ToList()
                .Select(x => new { x.DepartmentID, Description = x.Name }).ToList();
            return Json(departmentsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult Templates_Read([DataSourceRequest] DataSourceRequest request, int? departmentId)
        {
            var templatesList = db.Templates.Where(x => !departmentId.HasValue || x.DepartmentID == departmentId).ToList()
                .Select(x => new { x.TemplateID, Description = GetTemplateText(x) }).ToList();
            return Json(templatesList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult GetTemplatesList()
        {
            var templatesList = db.Templates.Include(x=>x.Department).ToList()
                .Select(x => new
                {
                    value = x.TemplateID, text = x.Description,
                    DepartmentId = x.DepartmentID,
                    DepartmentName = x.Department.Name
                }).ToList();
            return Json(templatesList, JsonRequestBehavior.AllowGet);
        }

        private string GetTemplateText(Template template)
        {
            return string.Format("{0} | {1}.{2}.{3}.{4}.{5}", template.Description, template.Fund,
                template.Dept, template.Program, template.Project, template.BaseElementObjectDetail);
        }

        private int GetTemplateOrder(int templateID)
        {
            var template = db.Templates.SingleOrDefault(x => x.TemplateID == templateID);
            return template != null ? template.Order : 0;
        }
        private string GetTemplateAccountNumber(int templateID)
        {
            var template = db.Templates.SingleOrDefault(x => x.TemplateID == templateID);
            if (template != null)
                return string.Format("{0}.{1}.{2}.{3}.{4}", template.Fund,
                    template.Dept, template.Program, template.Project, template.BaseElementObjectDetail);
            return string.Empty;
        }

        private string GetTemplateAccountNumber(Template template)
        {
            return string.Format("{0}.{1}.{2}.{3}.{4}", template.Fund,
                template.Dept, template.Program, template.Project, template.BaseElementObjectDetail);
        }

        [NoCache]
        public ActionResult ReceiptsBody_Read([DataSourceRequest] DataSourceRequest request)
        {
            var receiptBodies = db.ReceiptBodies.Include(x => x.Template).ToList()
                .Select(x => new
                {
                    x.ReceiptHeaderID,
                    x.ReceiptBodyID,
                    x.LineTotal,
                    x.TemplateID,
                    x.Template.DepartmentID,
                    AccountNumber = GetTemplateAccountNumber(x.Template),
                    TemplateOrder = x.Template.Order
                }).ToList();
            return Json(receiptBodies.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ReceiptsBody_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<ReceiptBody> receiptBodies, int receiptHeaderId)
        {
            var receiptBodiesList = receiptBodies as List<ReceiptBody> ?? receiptBodies.ToList();
            if (receiptBodiesList.Any() && ModelState.IsValid)
            {
                foreach (var receiptBody in receiptBodiesList)
                {
                    receiptBody.ReceiptHeaderID = receiptHeaderId;
                    db.ReceiptBodies.Add(receiptBody);
                    try
                    {
                        if (db.SaveChanges() <= 0)
                        {
                            //todo supports localization
                            ModelState.AddModelError("_addKey", "Can't add this receipt body to database");
                        }
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("_addKey", "Can't add this receipt body to database");
                    }
                }
            }

            return Json(receiptBodiesList.Select(
                    x => new { x.ReceiptHeaderID, x.ReceiptBodyID, x.LineTotal, x.TemplateID, AccountNumber = GetTemplateAccountNumber(x.TemplateID),
                        TemplateOrder = GetTemplateOrder(x.TemplateID) }).ToList().ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult ReceiptsBody_Update([DataSourceRequest] DataSourceRequest request, IEnumerable<ReceiptBody> receiptBodies)
        {
            var receiptBodiesList = receiptBodies as List<ReceiptBody> ?? receiptBodies.ToList();
            if (receiptBodiesList.Any() && ModelState.IsValid)
            {
                foreach (var receiptBody in receiptBodiesList)
                {
                    db.Entry(receiptBody).State = EntityState.Modified;
                    try
                    {
                        if (db.SaveChanges() <= 0)
                            ModelState.AddModelError("_updateKey", "Can't update this receipt body to database");
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("_updateKey", "Can't update this receipt body to database");
                    }
                }
            }
            return Json(receiptBodiesList.Select(
                    x => new
                    {
                        x.ReceiptHeaderID, x.ReceiptBodyID, x.LineTotal, x.TemplateID, AccountNumber = GetTemplateAccountNumber(x.TemplateID),
                        TemplateOrder = GetTemplateOrder(x.TemplateID)
                    }).ToList().ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult ReceiptsBody_Destroy([DataSourceRequest] DataSourceRequest request, IEnumerable<ReceiptBody> receiptBodies)
        {
            var receiptBodiesList = receiptBodies as List<ReceiptBody> ?? receiptBodies.ToList();
            if (receiptBodiesList.Any() && ModelState.IsValid)
            {
                foreach (var receiptBody in receiptBodiesList)
                {
                    var receiptBodyInDb = db.ReceiptBodies.SingleOrDefault(x => x.ReceiptBodyID == receiptBody.ReceiptBodyID);
                    if (receiptBodyInDb != null)
                    {
                        db.ReceiptBodies.Remove(receiptBodyInDb);
                        try
                        {
                            if (db.SaveChanges() <= 0)
                                ModelState.AddModelError("_deleteKey", "Can't remove this receipt body from database");
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("_deleteKey", "Can't remove this receipt body from database");
                        }
                    }
                }
            }
            return Json(receiptBodiesList.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Receipt Tenders Grid Actions
        [NoCache]
        public ActionResult GetPaymentMethods()
        {
            var paymentMethods = db.TenderPaymentMethods
                .Select(x => new { value = x.Id, text = x.Name }).ToList();
            return Json(paymentMethods, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult ReceiptsTenders_Read([DataSourceRequest] DataSourceRequest request)
        {
            var receiptTenders = db.Tenders
                .Select(x => new { x.ReceiptHeaderID, x.Amount, x.Description, x.TenderID, x.PaymentMethodId }).ToList();
            return Json(receiptTenders.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ReceiptsTenders_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<Tender> receiptTenders, int receiptHeaderId)
        {
            var receiptTendersList = receiptTenders as List<Tender> ?? receiptTenders.ToList();
            if (receiptTendersList.Any() && ModelState.IsValid)
            {
                foreach (var receiptTender in receiptTendersList)
                {
                    receiptTender.ReceiptHeaderID = receiptHeaderId;
                    db.Tenders.Add(receiptTender);
                    try
                    {
                        if (db.SaveChanges() <= 0)
                        {
                            //todo supports localization
                            ModelState.AddModelError("_addKey", "Can't add this tender to database");
                        }
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("_addKey", "Can't add this tender to database");
                    }
                }
            }

            return Json(receiptTendersList.Select(
                    x => new { x.ReceiptHeaderID, x.Amount, x.Description, x.TenderID, x.PaymentMethodId }).ToList().ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult ReceiptsTenders_Update([DataSourceRequest] DataSourceRequest request, IEnumerable<Tender> receiptTenders)
        {
            var receiptTendersList = receiptTenders as List<Tender> ?? receiptTenders.ToList();
            if (receiptTendersList.Any() && ModelState.IsValid)
            {
                foreach (var receiptTender in receiptTendersList)
                {
                    db.Entry(receiptTender).State = EntityState.Modified;
                    try
                    {
                        if (db.SaveChanges() <= 0)
                            ModelState.AddModelError("_updateKey", "Can't update this tender to database");
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("_updateKey", "Can't update this tender to database");
                    }
                }
            }
            return Json(receiptTendersList.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult ReceiptsTenders_Destroy([DataSourceRequest] DataSourceRequest request, IEnumerable<Tender> receiptTenders)
        {
            var receiptTendersList = receiptTenders as List<Tender> ?? receiptTenders.ToList();
            if (receiptTendersList.Any() && ModelState.IsValid)
            {
                foreach (var receiptTender in receiptTendersList)
                {
                    var receiptTenderInDb = db.Tenders.SingleOrDefault(x => x.TenderID == receiptTender.TenderID);
                    if (receiptTenderInDb != null)
                    {
                        db.Tenders.Remove(receiptTenderInDb);
                        try
                        {
                            if (db.SaveChanges() <= 0)
                                ModelState.AddModelError("_deleteKey", "Can't remove this tender from database");
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("_deleteKey", "Can't remove this tender from database");
                        }
                    }
                }
            }
            return Json(receiptTendersList.ToDataSourceResult(request, ModelState));
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
