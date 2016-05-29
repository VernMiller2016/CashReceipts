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

namespace CashReceipts.Controllers
{
    public class ReceiptHeadersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ReceiptHeaders
        public ActionResult Index()
        {
            var receiptHeader = db.ReceiptHeader.Include(r => r.Clerks);
            return View(receiptHeader.ToList());
        }

        // GET: ReceiptHeaders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiptHeader receiptHeader = db.ReceiptHeader.Include(x => x.Tenders).Include(x => x.ReceiptsBody).FirstOrDefault(x=> x.ReceiptHeaderID == id);
            if (receiptHeader == null)
            {
                return HttpNotFound();
            }
            return View(receiptHeader);
        }

        // GET: ReceiptHeaders/Create
        public ActionResult Create()
        {
            ViewBag.ClerkID = new SelectList(db.Clerk, "ClerkID", "FirstName");
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
                db.ReceiptHeader.Add(receiptHeader);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClerkID = new SelectList(db.Clerk, "ClerkID", "FirstName", receiptHeader.ClerkID);
            return View(receiptHeader);
        }

        // GET: ReceiptHeaders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiptHeader receiptHeader = db.ReceiptHeader.Include(x => x.Tenders).Include(x => x.ReceiptsBody).FirstOrDefault(x => x.ReceiptHeaderID == id);
            if (receiptHeader == null)
            {
                return HttpNotFound();
            }
            TempData["HeaderId"] = id;
            ViewBag.ClerkID = new SelectList(db.Clerk, "ClerkID", "FirstName", receiptHeader.ClerkID);
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
            ViewBag.ClerkID = new SelectList(db.Clerk, "ClerkID", "FirstName", receiptHeader.ClerkID);
            return View(receiptHeader);
        }

        // GET: ReceiptHeaders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiptHeader receiptHeader = db.ReceiptHeader.Find(id);
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
            ReceiptHeader receiptHeader = db.ReceiptHeader.Find(id);
            db.ReceiptHeader.Remove(receiptHeader);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AddReceiptBody(int id)
        {
            ReceiptBody receiptbody = new ReceiptBody();
            receiptbody.ReceiptHeaderID = id;
            ViewBag.DepartmentId = new SelectList(db.Department, "DepartmentID", "Name");
            ViewBag.TemplateID = new SelectList(db.Template, "TemplateID", "Description");
            return View(receiptbody);
        }

        [HttpPost, ActionName("AddReceiptBody")]
        public ActionResult AddReceiptBody(ReceiptBody receiptbody)
        {
            if (ModelState.IsValid)
            {
                db.ReceiptBody.Add(receiptbody);
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = receiptbody.ReceiptHeaderID });
            }
            ViewBag.DepartmentId = new SelectList(db.Department, "DepartmentID", "Name");
            ViewBag.TemplateID = new SelectList(db.Template, "TemplateID", "Description", receiptbody.TemplateID);
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
                db.Tender.Add(tenders);
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = tenders.ReceiptHeaderID });
            }
            return View(tenders);
        }

        [NoCache]
        public ActionResult GetDepartmentTemplates(int id)
        {
            var templates = db.Template.Where(x => x.DepartmentID == id).Select(x=>new { TemplateId = x.TemplateID,TemplateName =  x.Description}).ToList();
            return Json(templates, JsonRequestBehavior.AllowGet);
        }

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
