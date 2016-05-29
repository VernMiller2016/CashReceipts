using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CashReceipts.Models;
using System.Data.Entity;

namespace CashReceipts.Controllers
{
    public class TemplatesController : Controller
    {
        private ApplicationDbContext db;
        public TemplatesController(): this(new ApplicationDbContext())
        {

        }
        public TemplatesController(ApplicationDbContext context)
        {
            db = context;
        }

        // GET: Templates
        public ActionResult Index(int? SelectedDepartment)
        {
            var departments = db.Department.OrderBy(q => q.Name).ToList();
            ViewBag.SelectedDepartment = new SelectList(departments, "DepartmentID", "Name", SelectedDepartment);
            int departmentID = SelectedDepartment.GetValueOrDefault();

            IQueryable<Template> templates = db.Template
                .Where(c => !SelectedDepartment.HasValue || c.DepartmentID == departmentID)
                .OrderBy(d => d.TemplateID)
                .Include(d => d.Departments);
            var sql = templates.ToString();
            return View(templates.ToList());
        }

        // GET: Templates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Template template = db.Template.Include(x=>x.Departments).Single(m => m.TemplateID == id);

            if (template == null)
            {
                return HttpNotFound();
            }
            return View(template);
        }

        // GET: Templates/Create
        public ActionResult Create()
        {
            PopulateDepartmentsDropDownList();
            return View();
        }

        // POST: Templates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Template template)
        {
            if (ModelState.IsValid)
            {
                db.Template.Add(template);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateDepartmentsDropDownList();
            return View(template);
        }

        // GET: Templates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Template template = db.Template.Single(m => m.TemplateID == id);
            if (template == null)
            {
                return HttpNotFound();
            }
            PopulateDepartmentsDropDownList(template.DepartmentID);
            return View(template);
        }

        // POST: Templates/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Template template)
        {
            if (ModelState.IsValid)
            {
                db.Entry(template).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateDepartmentsDropDownList();
            return View(template);
        }

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in db.Department
                                   orderby d.Name
                                   select d;
            ViewBag.DepartmentID = new SelectList(departmentsQuery, "DepartmentID", "Name", selectedDepartment);
        }

        // GET: Templates/Delete/5
        [ActionName("Delete")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Template template = db.Template.Single(m => m.TemplateID == id);
            if (template == null)
            {
                return HttpNotFound();
            }

            return View(template);
        }

        // POST: Templates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Template template = db.Template.Single(m => m.TemplateID == id);
            db.Template.Remove(template);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
