using CashReceipts.Helpers;
using CashReceipts.Models;
using CashReceipts.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CashReceipts.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public AccessHelper access;
        public UsersController()
        {
            access = new AccessHelper();
        }
        // GET: Users
        [CanAccess((int)FeaturePermissions.UsersIndex)]
        public ActionResult Index()
        {
            List<UsersVM> usersList = new List<UsersVM>();
            var users = db.Users.Include(x=>x.Role).ToList();
            foreach (var item in users)
            {
                UsersVM userItem = new UsersVM();
                userItem.Id = item.Id;
                userItem.Email = item.Email;
                if (item.Role != null)
                    userItem.RoleName = item.Role.Name;
                else
                    userItem.RoleName = "";

                usersList.Add(userItem);
            }
            ViewBag.isEdit = access.UserFeatures.Where(f => f.FeatureId == (int)FeaturePermissions.EditUserRole).FirstOrDefault()==null? false:true;
            return View(usersList);
        }

        // GET: Users/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
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

        // GET: Users/Edit/5
        [CanAccess((int)FeaturePermissions.EditUserRole)]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Where(u=>u.Id==id).FirstOrDefault();

            if (user == null)
            {
                return HttpNotFound();
            }
            var Roles = db.Roles.ToList();
            string roleId = "";
            if (user.Role != null)
            {
                roleId = user.Role.Id;
            }
            ViewBag.Roles = new SelectList(Roles, "Id", "Name", roleId);
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,RoleName")] UsersVM user,string RoleId)
        {
            if (ModelState.IsValid)
            {
                var userItem = db.Users.Where(u => u.Id == user.Id).FirstOrDefault();
                userItem.RoleId = RoleId;
                db.Entry(userItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Users/Delete/5
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
