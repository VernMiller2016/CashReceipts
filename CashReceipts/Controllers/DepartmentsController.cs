using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CashReceipts.Filters;
using CashReceipts.Helpers;
using CashReceipts.Models;
using CashReceipts.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace CashReceipts.Controllers
{
    [Authorize]
    public class DepartmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Dpartments
        public ActionResult Index()
        {
            return View(db.Departments.OrderBy(x=>x.Name).ToList());
        }

        // GET: Dpartments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Single(m => m.DepartmentID == id);

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
                db.Departments.Add(department);
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
            Department department = db.Departments.Include(x => x.Templates).Single(m => m.DepartmentID == id);

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
            Department department = db.Departments.Single(m => m.DepartmentID == id);
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
            Department department = db.Departments.Single(m => m.DepartmentID == id);
            db.Departments.Remove(department);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AddTemplate(int id)
        {
            var department = db.Departments.SingleOrDefault(m => m.DepartmentID == id);
            if (department != null)
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
                db.Templates.Add(template);
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

        #region Template Grid Actions
        [NoCache]
        public ActionResult ProjectTemplates_Read([DataSourceRequest] DataSourceRequest request, int departmentId)
        {
            var templates =
           db.Templates.Where(x => x.DepartmentID == departmentId).Select(
               p => new { p.DepartmentID, p.BaseElementObjectDetail, p.Dept, p.Description, p.Fund, p.Order, p.Program, p.Project, p.TemplateID }).ToList();

            return Json(templates.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ProjectTemplates_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<Template> templates, int departmentId)
        {
            var templateList = templates as List<Template> ?? templates.ToList();
            if (templateList.Any() && ModelState.IsValid)
            {
                foreach (var template in templateList)
                {
                    if ((template.DataSource==AccountDataSource.GrantCounty && !IsValidGCAccount(template))
                        || (template.DataSource == AccountDataSource.District && !IsValidDistAccount(template)))
                    {
                        ModelState.AddModelError("_addKey", "Invalid account, Please check GC Accounts page for a valid data!");
                    }
                    else
                    {
                        template.DepartmentID = departmentId;
                        db.Templates.Add(template);
                        try
                        {
                            if (db.SaveChanges() <= 0)
                            {
                                //todo supports localization
                                ModelState.AddModelError("_addKey", "Can't add this template to database");
                            }
                        }
                        catch (Exception e)
                        {
                            ModelState.AddModelError("_addKey", "Can't add this template to database");
                        }
                    }
                }
            }

            return Json(templateList.Select(
                    p => new { p.DepartmentID, p.BaseElementObjectDetail, p.Dept, p.Description, p.Fund, p.Order, p.Program, p.Project, p.TemplateID }).ToList().ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult ProjectTemplates_Update([DataSourceRequest] DataSourceRequest request, IEnumerable<Template> templates, int departmentId)
        {
            var templateList = templates as List<Template> ?? templates.ToList();
            if (templateList.Any() && ModelState.IsValid)
            {
                foreach (var template in templateList)
                {
                    if ((template.DataSource == AccountDataSource.GrantCounty && !IsValidGCAccount(template))
                        || (template.DataSource == AccountDataSource.District && !IsValidDistAccount(template)))
                    {
                        ModelState.AddModelError("_addKey",
                            "Invalid account, Please check GC Accounts page for a valid data!");
                    }
                    else
                    {
                        db.Entry(template).State = EntityState.Modified;
                        try
                        {
                            if (db.SaveChanges() <= 0)
                            {
                                ModelState.AddModelError("_updateKey", "Can't update this template to database");
                            }
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("_updateKey", "Can't update this template to database");
                        }
                    }
                }
            }
            return Json(templates.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult ProjectTemplates_Destroy([DataSourceRequest] DataSourceRequest request, IEnumerable<Template> templates, int departmentId)
        {
            var templateList = templates as List<Template> ?? templates.ToList();
            if (templateList.Any() && ModelState.IsValid)
            {
                foreach (var template in templateList)
                {
                    var templateInDb = db.Templates.SingleOrDefault(x => x.TemplateID == template.TemplateID);
                    if (templateInDb != null)
                    {
                        db.Templates.Remove(templateInDb);
                        try
                        {
                            if (db.SaveChanges() <= 0)
                            {
                                ModelState.AddModelError("_deleteKey", "Can't remove this template from database");
                            }
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("_deleteKey", "Can't remove this template from database");
                        }
                    }
                }
            }
            return Json(templates.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult ReorderTemplates(int currOrderId, int newOrderid)
        {
            var result = false;
            var firstTemplate = db.Templates.SingleOrDefault(x => x.TemplateID == currOrderId);
            var secondTemplate = db.Templates.SingleOrDefault(x => x.TemplateID == newOrderid);
            if (firstTemplate != null && secondTemplate != null)
            {
                var temp = firstTemplate.Order;
                firstTemplate.Order = secondTemplate.Order;
                secondTemplate.Order = temp;
                db.SaveChanges();
                result = true;
            }
            return Json(new { Result = result });
        }


        private bool IsValidGCAccount(Template template)
        {
            var dbName = new LookupHelper(db).GcDbName;
            var sqlQuery = $"Select count(*) from {dbName}dbo.GL00100 where Active = 1 and [ACTNUMBR_1] = '{template.Fund}'"
                + $" and [ACTNUMBR_2] = '{template.Dept}' and [ACTNUMBR_3] = '{template.Program}' and [ACTNUMBR_4] = '{template.Project}'"
                + $" and [ACTNUMBR_5] = '{template.BaseElementObjectDetail}'";
            return db.Database.SqlQuery<int>(sqlQuery).First() > 0;
        }

        private bool IsValidDistAccount(Template template)
        {
            var dbName = new LookupHelper(db).DistDbName;
            var sqlQuery = $"Select count(*) from {dbName}dbo.GL00100 where Active = 1 "
                + $"and [ACTNUMBR_1] = '{template.Fund}{template.Dept}{template.Program}'"
                + $" and [ACTNUMBR_2] = '{template.Project}'"
                + $" and [ACTNUMBR_3] = '{template.BaseElementObjectDetail}'";
            return db.Database.SqlQuery<int>(sqlQuery).First() > 0;
        }

        #endregion

        public ActionResult GetGcAccountDetails([DataSourceRequest] DataSourceRequest request, AutoCompleteViewModel model)
        {
            var results = new List<Template>();
            var rowsNum = 100;
            if (!string.IsNullOrEmpty(model.value))
            {
                ColumnOrders columnOrder;
                switch (model.field)
                {
                    case "Fund":
                        columnOrder = ColumnOrders.Fund;
                        break;
                    case "Dept":
                        columnOrder = ColumnOrders.Dept;
                        break;
                    case "Program":
                        columnOrder = ColumnOrders.Program;
                        break;
                    case "Project":
                        columnOrder = ColumnOrders.Project;
                        break;
                    case "BaseElementObjectDetail":
                        columnOrder = ColumnOrders.BaseElementObjectDetail;
                        break;
                    case "Description":
                        columnOrder = ColumnOrders.Description;
                        break;
                    default:
                        columnOrder = ColumnOrders.Description;
                        break;
                }
                results = db.GetGCAccounts(columnOrder, model.value, rowsNum, model.skip);
            }
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGcAccountDescription(Template template)
        {
            int resultsCount = 0;
            var result = db.FilterGlAccounts(0, 1, SearchAccountDataSource.Both, template.Fund, template.Dept, template.Program, template.Project,
                template.BaseElementObjectDetail, "", ref resultsCount);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
