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
    public class DepartmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Dpartments
        public ActionResult Index()
        {
            return View(db.Department.ToList());
        }

        // GET: Dpartments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Department.Single(m => m.DepartmentID == id);

            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: Dpartments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dpartments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DepartmentID,Name")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Department.Add(department);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(department);
        }

        // GET: Dpartments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Department.Include(x => x.Templates).Single(m => m.DepartmentID == id);

            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Dpartments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DepartmentID,Name")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(department);
        }

        // GET: Dpartments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Department.Single(m => m.DepartmentID == id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Dpartments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Department department = db.Department.Single(m => m.DepartmentID == id);
            db.Department.Remove(department);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AddTemplate(int id)
        {
            var department = db.Department.SingleOrDefault(m => m.DepartmentID == id);
            if(department!=null)
            {
                ViewBag.DepartmentName = department.Name;
                //ViewData["DepartmentName"] = department.Name;
                Template template = new Template();
                template.DepartmentID = id;
                return View(template);
            }
            return Content("No such department has been found in database!");
        }

        [HttpPost, ActionName("AddTemplate")]
        public ActionResult AddTemplate(Template template)
        {
            if (ModelState.IsValid)
            {
                db.Template.Add(template);
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = template.DepartmentID });
            }
            return View(template);
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
