using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using CashReceipts.Models;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using CashReceipts.Filters;
using CashReceipts.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace CashReceipts.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private ApplicationDbContext db;
        public AccountsController() : this(new ApplicationDbContext())
        {

        }
        public AccountsController(ApplicationDbContext context)
        {
            db = context;
        }

        // GET: Templates
        [CanAccess((int)FeaturePermissions.SystemAccountIndex)]
        public ActionResult Index(int? SelectedDepartment)
        {
            var departments = db.Departments.OrderBy(q => q.Name).ToList();
            ViewBag.SelectedDepartment = new SelectList(departments, "DepartmentID", "Name", SelectedDepartment);
            int departmentID = SelectedDepartment.GetValueOrDefault();

            IQueryable<Template> templates = db.Templates
                .Where(c => !SelectedDepartment.HasValue || c.DepartmentID == departmentID)
                .OrderBy(d => d.TemplateID)
                .Include(d => d.Department);
            var sql = templates.ToString();
            return View(templates.ToList());
        }

        // GET: Templates/Details/5
        [CanAccess((int)FeaturePermissions.ViewSystemAccount)]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Template template = db.Templates.Include(x => x.Department).Single(m => m.TemplateID == id);

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
                db.Templates.Add(template);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateDepartmentsDropDownList();
            return View(template);
        }

        // GET: Templates/Edit/5
        [CanAccess((int)FeaturePermissions.EditSystemAccount)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Template template = db.Templates.Single(m => m.TemplateID == id);
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
            var departmentsQuery = from d in db.Departments
                                   orderby d.Name
                                   select d;
            ViewBag.DepartmentID = new SelectList(departmentsQuery, "DepartmentID", "Name", selectedDepartment);
        }

        // GET: Templates/Delete/5
        [ActionName("Delete")]
        [CanAccess((int)FeaturePermissions.DeleteSystemAccount)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Template template = db.Templates.Single(m => m.TemplateID == id);
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
            Template template = db.Templates.Single(m => m.TemplateID == id);
            db.Templates.Remove(template);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [CanAccess((int)FeaturePermissions.GrantCountyAccountIndex)]
        public ActionResult GcAccounts()
        {
            return View();
        }

        [NoCache]
        public ActionResult GcAccounts_Read([DataSourceRequest] DataSourceRequest request, int? skip, int? take,
            string fund, string dept, string program, string project, string baseElementObjectDetail, string description)
        {
            int accountsValidResultsCount = 0;
            var accounts = db.FilterGlAccounts(skip, take, SearchAccountDataSource.GrantCounty, fund, dept, program, project, baseElementObjectDetail, description, null, ref accountsValidResultsCount);
            return Json(new { Data = accounts, Total = accountsValidResultsCount }, JsonRequestBehavior.AllowGet);
        }

        [CanAccess((int)FeaturePermissions.DistrictsAccountIndex)]
        public ActionResult DistAccounts()
        {
            return View();
        }

        [NoCache]
        public ActionResult DistAccounts_Read([DataSourceRequest] DataSourceRequest request, int? skip, int? take,
            string fund, string dept, string program, string project, string baseElementObjectDetail, string description)
        {
            int accountsValidResultsCount = 0;
            var accounts = db.FilterGlAccounts(skip, take, SearchAccountDataSource.District, fund, dept, program, project, baseElementObjectDetail, description, null, ref accountsValidResultsCount);
            return Json(new { Data = accounts, Total = accountsValidResultsCount }, JsonRequestBehavior.AllowGet);
        }
        
    }
}
