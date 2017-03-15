using CashReceipts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace CashReceipts.Controllers
{
    public class PermissionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Permissions
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetScreens()
        {
            var screens = db.Screens.Include(s=>s.Features).ToList();
            TreeViewModel tv = new TreeViewModel();
            List<TreeViewModel> tvList = new List<TreeViewModel>();

            foreach (var item in screens)
            {
                tv.Id = item.Id;
                tv.text = item.Name;
                tv.spriteCssClass = "rootfolder";
                tv.expanded = true;
                if (item.Features != null)
                {
                    tv.Items = new List<TreeModel>();

                    foreach (var feature in item.Features)
                    {
                        TreeModel tv2 = new TreeModel();
                        tv2.Id = feature.Id;
                        tv2.text = feature.Name;
                        tv2.spriteCssClass = "folder";
                        tv2.expanded = true;
                        tv.Items.Add( tv2);
                    }
                }
                tvList.Add(tv);
            }
            return Json(tvList);
            
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
