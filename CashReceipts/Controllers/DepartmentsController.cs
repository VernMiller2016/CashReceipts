﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CashReceipts.Filters;
using CashReceipts.Models;
using CashReceipts.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace CashReceipts.Controllers
{
    [Authorize]
    public class DepartmentsController : BaseController
    {
        // GET: Dpartments
        [CanAccess((int)FeaturePermissions.DepartmentIndex)]
        public ActionResult Index()
        {
            ViewBag.isCreate = HasAccess(FeaturePermissions.CreateDepartment);
            ViewBag.isEdit = HasAccess(FeaturePermissions.EditDepartment);
            ViewBag.isDetails = HasAccess(FeaturePermissions.ViewDepartment);
            ViewBag.isDelete = HasAccess(FeaturePermissions.DeleteDepartment);
            return View(_db.Departments.OrderBy(x=>x.Name).ToList());
        }

        // GET: Dpartments/Details/5
        [CanAccess((int)FeaturePermissions.ViewDepartment)]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = _db.Departments.Single(m => m.DepartmentID == id);

            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: Dpartments/Create
        [CanAccess((int)FeaturePermissions.CreateDepartment)]
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
                _db.Departments.Add(department);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(department);
        }

        // GET: Dpartments/Edit/5
        [CanAccess((int)FeaturePermissions.EditDepartment)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = _db.Departments.Include(x => x.Templates).Single(m => m.DepartmentID == id);

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
                _db.Entry(department).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(department);
        }

        // GET: Dpartments/Delete/5
        [CanAccess((int)FeaturePermissions.DeleteDepartment)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = _db.Departments.Single(m => m.DepartmentID == id);
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
            Department department = _db.Departments.Single(m => m.DepartmentID == id);
            _db.Departments.Remove(department);
            try
            {
                _db.SaveChanges();
            return RedirectToAction("Index");
            }
            catch (Exception e)
            {
            }
            return
                Content("Sorry, Department couldn't be deleted because it's being referenced by other system objects.");
        }

        public ActionResult AddTemplate(int id)
        {
            var department = _db.Departments.SingleOrDefault(m => m.DepartmentID == id);
            if (department != null)
            {
                ViewBag.DepartmentName = department.Name;
                //ViewData["DepartmentName"] = department.Name;
                Template template = new Template {DepartmentID = id};
                return View(template);
            }
            return Content("No such department has been found in database!");
        }

        [HttpPost, ActionName("AddTemplate")]
        public ActionResult AddTemplate(Template template)
        {
            if (ModelState.IsValid)
            {
                _db.Templates.Add(template);
                _db.SaveChanges();
                return RedirectToAction("Edit", new { id = template.DepartmentID });
            }
            return View(template);
        }
        
        #region Template Grid Actions
        [NoCache]
        public ActionResult ProjectTemplates_Read([DataSourceRequest] DataSourceRequest request, int departmentId)
        {
            var templates =
           _db.Templates.Where(x => x.DepartmentID == departmentId).Select(
               p => new { p.DepartmentID, p.BaseElementObjectDetail, p.Dept, p.Description, p.Fund, p.Order, p.Program, p.Project, p.TemplateID, p.DataSource }).ToList();

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
                    if ((template.DataSource==AccountDataSource.GrantCounty && !_db.IsValidGCAccount(template))
                        || (template.DataSource == AccountDataSource.District && !_db.IsValidDistAccount(template)))
                    {
                        ModelState.AddModelError("_addKey", "Invalid account, Please check GC Accounts page for a valid data!");
                    }
                    else
                    {
                        template.DepartmentID = departmentId;
                        _db.Templates.Add(template);
                        try
                        {
                            if (_db.SaveChanges() <= 0)
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
                    p => new { p.DepartmentID, p.BaseElementObjectDetail, p.Dept, p.Description, p.Fund, p.Order, p.Program, p.Project, p.TemplateID, p.DataSource }).ToList().ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult ProjectTemplates_Update([DataSourceRequest] DataSourceRequest request, IEnumerable<Template> templates, int departmentId)
        {
            var templateList = templates as List<Template> ?? templates.ToList();
            if (templateList.Any() && ModelState.IsValid)
            {
                foreach (var template in templateList)
                {
                    if ((template.DataSource == AccountDataSource.GrantCounty && !_db.IsValidGCAccount(template))
                        || (template.DataSource == AccountDataSource.District && !_db.IsValidDistAccount(template)))
                    {
                        ModelState.AddModelError("_addKey",
                            "Invalid account, Please check GC/DIST Accounts page for a valid data!");
                    }
                    else
                    {
                        _db.Entry(template).State = EntityState.Modified;
                        try
                        {
                            if (_db.SaveChanges() <= 0)
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
                    var templateInDb = _db.Templates.SingleOrDefault(x => x.TemplateID == template.TemplateID);
                    if (templateInDb != null)
                    {
                        templateInDb.DepartmentID = null;
                        try
                        {
                            if (_db.SaveChanges() <= 0)
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
            var firstTemplate = _db.Templates.SingleOrDefault(x => x.TemplateID == currOrderId);
            var secondTemplate = _db.Templates.SingleOrDefault(x => x.TemplateID == newOrderid);
            if (firstTemplate != null && secondTemplate != null)
            {
                var temp = firstTemplate.Order;
                firstTemplate.Order = secondTemplate.Order;
                secondTemplate.Order = temp;
                _db.SaveChanges();
                result = true;
            }
            return Json(new { Result = result });
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
                    case "Account":
                        columnOrder = ColumnOrders.Account;
                        break;
                    default:
                        columnOrder = ColumnOrders.Description;
                        break;
                }
                results = _db.GetGCAccounts(columnOrder, model.value, rowsNum, model.skip);
            }
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGcAccountDescription(Template template)
        {
            int resultsCount = 0;
            var result = _db.FilterGlAccounts(0, 1, SearchAccountDataSource.Both, template.Fund, template.Dept, template.Program, template.Project,
                template.BaseElementObjectDetail, "", null, ref resultsCount);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
