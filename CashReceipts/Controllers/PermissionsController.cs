using CashReceipts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data;

namespace CashReceipts.Controllers
{
    public class PermissionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Permissions
        public ActionResult Index()
        {
            var Roles = db.Roles.ToList();
            ViewBag.Roles = new SelectList(Roles, "Id", "Name");
            return View();
        }
        [HttpPost]
        public JsonResult GetScreens()
        {
            var screens = db.Screens.Include(s=>s.Features).ToList();
            TreeViewModel tv = new TreeViewModel();
            List<TreeViewModel> tvList = new List<TreeViewModel>();
            int count = 1;
            foreach (var item in screens)
            {
                tv = new TreeViewModel();
                tv.id = item.Id;
                tv.text = item.Name;
                tv.spriteCssClass = "folder";
                tv.expanded = true;
                if (item.Features != null)
                {
                    tv.items = new List<TreeModel>();

                    foreach (var feature in item.Features)
                    {
                        TreeModel tv2 = new TreeModel();
                        tv2.id = feature.Id;
                        tv2.text = feature.Name;
                        //tv2.spriteCssClass = "folder";
                        //tv2.expanded = true;
                        tv.items.Add( tv2);
                    }
                }
                tvList.Add(tv);
            }
            return Json(tvList);
            
        }

        [HttpPost]
        public JsonResult GetRoleValues(string roleId)
        {
            var screenFeatures = db.ScreenFeatures.Include(r => r.Roles).Where(r => r.Roles.Any(ro => ro.RoleId == roleId)).ToList();
            var AllScreenFeatures = db.ScreenFeatures.ToList();
            var AllScreens = db.Screens.ToList();
            if (screenFeatures.Count > 0)
            {
                Permissions permission = new Permissions();
                foreach (var item in screenFeatures)
                {
                    permission.SelectedFeatures.Add(item.Name);
                    if (permission.SelectedScreens.Where(s => s == item.Screen.Name).FirstOrDefault() == null)
                        permission.SelectedScreens.Add(item.Screen.Name);
                }
                foreach (var item in AllScreenFeatures)
                {
                    if (permission.SelectedFeatures.Where(s => s == item.Name).FirstOrDefault() == null)
                        permission.UnSelectedFeatures.Add(item.Name);
                }
                foreach (var item in AllScreens)
                {
                    if (permission.SelectedScreens.Where(s => s == item.Name).FirstOrDefault() == null)
                        permission.UnSelectedScreens.Add(item.Name);
                }

                return Json(permission);

            }
            else return Json("new");

        }

        [HttpPost]
        public bool SavePermissions(string nodeIds,string roleId)
        {
            string[] featureIds = nodeIds.Split(',').Distinct().ToArray();
            var existingFeatures = db.ScreenFeatures.Include(r => r.Roles).Where(r => r.Roles.Any(ro => ro.RoleId == roleId)).ToList();
            var allScreenFeatures = db.ScreenFeatures.ToList();
            RoleFeaturePermission rfp = new RoleFeaturePermission();
            rfp.RoleId = roleId;
            try
            {
                foreach (var item in allScreenFeatures)
                {
                    var feature = existingFeatures.Where(f => f.Id == item.Id).FirstOrDefault();
                    if (featureIds.Where(s => s == item.Id.ToString()).FirstOrDefault() != null)
                    {
                        if (feature == null)
                        {
                            rfp.FeatureId = item.Id;
                            if (item.Roles == null)
                                item.Roles = new List<RoleFeaturePermission>();
                            item.Roles.Add(rfp);
                            db.SaveChanges();
                        }
                    }
                    else if (feature != null)
                    {
                        rfp.FeatureId = feature.Id;
                        item.Roles.Remove(rfp);
                        db.SaveChanges();
                    }
                    rfp = new RoleFeaturePermission();
                    rfp.RoleId = roleId;
                }
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        // GET: Permissions/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Permissions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Permissions/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Permissions/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Permissions/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Permissions/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Permissions/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
