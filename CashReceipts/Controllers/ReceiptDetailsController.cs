using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CashReceipts.Models;

namespace CashReceipts.Controllers
{
    public class ReceiptDetailsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ReceiptDetails
        public ActionResult Index()
        {
            var receiptDetails = db.ReceiptDetails.Include(r => r.ReceiptBodys);
            return View(receiptDetails.ToList());
        }

        // GET: ReceiptDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiptDetail receiptDetail = db.ReceiptDetails.Find(id);
            if (receiptDetail == null)
            {
                return HttpNotFound();
            }
            return View(receiptDetail);
        }

        // GET: ReceiptDetails/Create
        public ActionResult Create()
        {
            ViewBag.ReceiptBodyID = new SelectList(db.ReceiptBodies, "ReceiptBodyID", "ReceiptBodyID");
            return View();
        }

        // POST: ReceiptDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReceiptDetailID,CheckWarrant,CheckWarrantAmount,ReceiptBodyID")] ReceiptDetail receiptDetail)
        {
            if (ModelState.IsValid)
            {
                db.ReceiptDetails.Add(receiptDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ReceiptBodyID = new SelectList(db.ReceiptBodies, "ReceiptBodyID", "ReceiptBodyID", receiptDetail.ReceiptBodyID);
            return View(receiptDetail);
        }

        // GET: ReceiptDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiptDetail receiptDetail = db.ReceiptDetails.Find(id);
            if (receiptDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReceiptBodyID = new SelectList(db.ReceiptBodies, "ReceiptBodyID", "ReceiptBodyID", receiptDetail.ReceiptBodyID);
            return View(receiptDetail);
        }

        // POST: ReceiptDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReceiptDetailID,CheckWarrant,CheckWarrantAmount,ReceiptBodyID")] ReceiptDetail receiptDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(receiptDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ReceiptBodyID = new SelectList(db.ReceiptBodies, "ReceiptBodyID", "ReceiptBodyID", receiptDetail.ReceiptBodyID);
            return View(receiptDetail);
        }

        // GET: ReceiptDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiptDetail receiptDetail = db.ReceiptDetails.Find(id);
            if (receiptDetail == null)
            {
                return HttpNotFound();
            }
            return View(receiptDetail);
        }

        // POST: ReceiptDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReceiptDetail receiptDetail = db.ReceiptDetails.Find(id);
            db.ReceiptDetails.Remove(receiptDetail);
            db.SaveChanges();
            return RedirectToAction("Index");
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
