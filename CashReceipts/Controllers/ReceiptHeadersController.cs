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

namespace CashReceipts.Controllers
{
    public class ReceiptHeadersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ReceiptHeaders
        public ActionResult Index()
        {
            var receiptHeader = db.ReceiptHeaders.Include(r => r.Clerks);
            return View(receiptHeader.ToList());
        }

        // GET: ReceiptHeaders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiptHeader receiptHeader = db.ReceiptHeaders.Include(x => x.Tenders).Include(x => x.ReceiptsBody).FirstOrDefault(x=> x.ReceiptHeaderID == id);
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
            ReceiptHeader receiptHeader = db.ReceiptHeaders.Include(x => x.Tenders).Include(x => x.ReceiptsBody).FirstOrDefault(x => x.ReceiptHeaderID == id);
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
            var templates = db.Templates.Where(x => x.DepartmentID == id).Select(x=>new { TemplateId = x.TemplateID,TemplateName =  x.Description}).ToList();
            return Json(templates, JsonRequestBehavior.AllowGet);
        }

#region Receipt Details Grid Actions
        [NoCache]
        public ActionResult GetTemplatesList()
        {
            var templatesList = db.Templates
                .Select(x => new {value = x.TemplateID, text = x.Description}).ToList();
            return Json(templatesList, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult ReceiptDetails_Read([DataSourceRequest] DataSourceRequest request, int receiptBodyId)
        {
            var receiptDetails =
          db.ReceiptDetails.Where(p => p.ReceiptBodyID == receiptBodyId).Select(
               p => new { p.ReceiptDetailID, p.ReceiptBodyID, p.CheckWarrant, p.CheckWarrantAmount }).ToList();
            return Json(receiptDetails.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ReceiptDetails_Create([DataSourceRequest] DataSourceRequest request, int receiptBodyId, IEnumerable<ReceiptDetail> receiptDetails)
        {
            var receiptDetailsList = receiptDetails as List<ReceiptDetail> ?? receiptDetails.ToList();
            if (receiptDetailsList.Any() && ModelState.IsValid)
            {
                foreach (var receiptDetail in receiptDetailsList)
                {
                    receiptDetail.ReceiptBodyID = receiptBodyId;
                    db.ReceiptDetails.Add(receiptDetail);
                    try
                    {
                        if (db.SaveChanges() <= 0)
                        {
                            //todo supports localization
                            ModelState.AddModelError("_addKey", "Can't add this receipt Detail to database");
                        }
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("_addKey", "Can't add this receipt Detail to database");
                    }
                }
            }

            return Json(receiptDetailsList.Select(
                    p => new { p.ReceiptDetailID, p.ReceiptBodyID, p.CheckWarrant, p.CheckWarrantAmount }).ToList().ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult ReceiptDetails_Update([DataSourceRequest] DataSourceRequest request, int receiptBodyId, IEnumerable<ReceiptDetail> receiptDetails)
        {
            var receiptDetailsList = receiptDetails as List<ReceiptDetail> ?? receiptDetails.ToList();
            if (receiptDetailsList.Any() && ModelState.IsValid)
            {
                foreach (var receiptDetail in receiptDetailsList)
                {
                    db.Entry(receiptDetail).State = EntityState.Modified;
                    try
                    {
                        if (db.SaveChanges() <= 0)
                        {
                            //todo supports localization
                            ModelState.AddModelError("_addKey", "Can't update this receipt Detail to database");
                        }
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("_addKey", "Can't update this receipt Detail to database");
                    }
                }
            }

            return Json(receiptDetailsList.Select(
                    p => new { p.ReceiptDetailID, p.ReceiptBodyID, p.CheckWarrant, p.CheckWarrantAmount }).ToList().ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult ReceiptDetails_Destroy([DataSourceRequest] DataSourceRequest request, int receiptBodyId, IEnumerable<ReceiptDetail> receiptDetails)
        {
            var receiptDetailsList = receiptDetails as List<ReceiptDetail> ?? receiptDetails.ToList();
            if (receiptDetailsList.Any() && ModelState.IsValid)
            {
                foreach (var receiptDetail in receiptDetailsList)
                {
                    var receiptDetailInDb = db.ReceiptDetails.SingleOrDefault(x => x.ReceiptDetailID == receiptDetail.ReceiptDetailID);
                    if (receiptDetailInDb != null)
                    {
                        db.ReceiptDetails.Remove(receiptDetailInDb);
                        try
                        {
                            if (db.SaveChanges() <= 0)
                            {
                                //todo supports localization
                                ModelState.AddModelError("_addKey", "Can't remove this receipt Detail to database");
                            }
                        }
                        catch (Exception e)
                        {
                            ModelState.AddModelError("_addKey", "Can't remove this receipt Detail to database");
                        }
                    }
                }
            }

            return Json(receiptDetailsList.Select(
                    p => new { p.ReceiptDetailID, p.ReceiptBodyID, p.CheckWarrant, p.CheckWarrantAmount }).ToList().ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Receipt Body Grid Actions
        [NoCache]
        public ActionResult ReceiptsBody_Read([DataSourceRequest] DataSourceRequest request, int receiptHeaderId)
        {
            var receiptBodies = db.ReceiptBodies.Where(x => x.ReceiptHeaderID == receiptHeaderId)
                .Select(x => new {x.ReceiptHeaderID, x.ReceiptBodyID, x.LineTotal, x.TemplateID}).ToList();
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
                    x => new { x.ReceiptHeaderID, x.ReceiptBodyID, x.LineTotal, x.TemplateID }).ToList().ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult ProjectTemplates_Update([DataSourceRequest] DataSourceRequest request, IEnumerable<ReceiptBody> receiptBodies, int receiptHeaderId)
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
                        {
                            ModelState.AddModelError("_updateKey", "Can't update this receipt body to database");
                        }
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("_updateKey", "Can't update this receipt body to database");
                    }
                }
            }
            return Json(receiptBodiesList.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult ProjectTemplates_Destroy([DataSourceRequest] DataSourceRequest request, IEnumerable<ReceiptBody> receiptBodies, int receiptHeaderId)
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
                            {
                                ModelState.AddModelError("_deleteKey", "Can't remove this receipt body from database");
                            }
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
