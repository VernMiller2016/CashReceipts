using System.Linq;
using System.Web.Mvc;
using CashReceipts.Models;
using System.Data.Entity;
using CashReceipts.Filters;
using CashReceipts.ViewModels;
using Kendo.Mvc.UI;

namespace CashReceipts.Controllers
{
    [Authorize]
    public class AccountsController : BaseController
    {
        // GET: Templates
        [CanAccess((int)FeaturePermissions.SystemAccountIndex)]
        public ActionResult Index(int? SelectedDepartment)
        {
            var departments = _db.Departments.OrderBy(q => q.Name).ToList();
            ViewBag.SelectedDepartment = new SelectList(departments, "DepartmentID", "Name", SelectedDepartment);
            int departmentId = SelectedDepartment.GetValueOrDefault();
            var templates = _db.Templates
                .Where(c => !SelectedDepartment.HasValue || c.DepartmentID == departmentId)
                .OrderBy(d => d.TemplateID)
                .Include(d => d.Department);
            ViewBag.isEdit = HasAccess(FeaturePermissions.EditSystemAccount);
            ViewBag.isDetails = HasAccess(FeaturePermissions.ViewSystemAccount);
            ViewBag.isDelete = HasAccess(FeaturePermissions.DeleteSystemAccount);
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
            Template template = _db.Templates.Include(x => x.Department).Single(m => m.TemplateID == id);

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
                _db.Templates.Add(template);
                _db.SaveChanges();
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

            Template template = _db.Templates.Single(m => m.TemplateID == id);
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
                _db.Entry(template).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateDepartmentsDropDownList();
            return View(template);
        }

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in _db.Departments
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

            Template template = _db.Templates.Single(m => m.TemplateID == id);
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
            Template template = _db.Templates.Single(m => m.TemplateID == id);
            _db.Templates.Remove(template);
            _db.SaveChanges();
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
            var accounts = _db.FilterGlAccounts(skip, take, SearchAccountDataSource.GrantCounty, fund, dept, program, project, baseElementObjectDetail, description, null, ref accountsValidResultsCount);
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
            var accounts = _db.FilterGlAccounts(skip, take, SearchAccountDataSource.District, fund, dept, program, project, baseElementObjectDetail, description, null, ref accountsValidResultsCount);
            return Json(new { Data = accounts, Total = accountsValidResultsCount }, JsonRequestBehavior.AllowGet);
        }
    }
}
