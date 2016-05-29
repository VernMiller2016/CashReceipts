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
    public class ClerksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Clerks
        public ActionResult Index()
        {
            return View(db.Clerk.ToList());
        }

        // GET: Clerks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clerk clerk = db.Clerk.Find(id);
            if (clerk == null)
            {
                return HttpNotFound();
            }
            return View(clerk);
        }

        // GET: Clerks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clerks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClerkID,FirstName,LastName")] Clerk clerk)
        {
            if (ModelState.IsValid)
            {
                db.Clerk.Add(clerk);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(clerk);
        }

        // GET: Clerks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clerk clerk = db.Clerk.Find(id);
            if (clerk == null)
            {
                return HttpNotFound();
            }
            return View(clerk);
        }

        // POST: Clerks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClerkID,FirstName,LastName")] Clerk clerk)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clerk).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(clerk);
        }

        // GET: Clerks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clerk clerk = db.Clerk.Find(id);
            if (clerk == null)
            {
                return HttpNotFound();
            }
            return View(clerk);
        }

        // POST: Clerks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Clerk clerk = db.Clerk.Find(id);
            db.Clerk.Remove(clerk);
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
