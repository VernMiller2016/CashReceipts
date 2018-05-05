using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CashReceipts.Filters;
using CashReceipts.Models;
using CashReceipts.ViewModels;

namespace CashReceipts.Controllers
{
    [Authorize]
    public class ClerksController : BaseController
    {
        // GET: Clerks
        [CanAccess((int)FeaturePermissions.ClerksIndex)]
        public ActionResult Index()
        {
            ViewBag.isCreate = HasAccess(FeaturePermissions.EditClerks);
            ViewBag.isEdit = HasAccess(FeaturePermissions.CreateClerks);
            ViewBag.isDetails = HasAccess(FeaturePermissions.ViewClerks);
            ViewBag.isDelete = HasAccess(FeaturePermissions.DeleteClerks);
            return View(_db.Clerks.Include(x => x.User).ToList());
        }

        // GET: Clerks/Details/5
        [CanAccess((int)FeaturePermissions.ViewClerks)]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clerk clerk = _db.Clerks.Find(id);
            if (clerk == null)
            {
                return HttpNotFound();
            }
            return View(clerk);
        }

        // GET: Clerks/Create
        [CanAccess((int)FeaturePermissions.CreateClerks)]
        public ActionResult Create()
        {
            var Users = _db.Users.ToList();
            var Roles = _db.Roles.ToList();
            ViewBag.Users = new SelectList(Users, "Id", "UserName");
            ViewBag.Roles = new SelectList(Roles, "Id", "Name");
            return View();
        }

        // POST: Clerks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClerkID,FirstName,LastName")] Clerk clerk, string UserID)
        {
            if (ModelState.IsValid)
            {
                var user = _db.Users.FirstOrDefault(u => u.Id == UserID);

                if (_db.Clerks.FirstOrDefault(c => c.UserId == UserID) != null)
                {
                    ModelState.AddModelError("", "The user " + user.UserName + " is already used with another clerk");
                    var users = _db.Users.ToList();
                    var roles = _db.Roles.ToList();
                    ViewBag.Users = new SelectList(users, "Id", "UserName");
                    ViewBag.Roles = new SelectList(roles, "Id", "Name");
                    return View(clerk);
                }

                clerk.UserId = UserID;
                _db.Clerks.Add(clerk);
                _db.SaveChanges();
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
            Clerk clerk = _db.Clerks.Find(id);
            if (clerk == null)
            {
                return HttpNotFound();
            }
            var users = _db.Users.ToList();
            var roles = _db.Roles.ToList();
            string roleId = "";
            if (clerk.User != null && clerk.User.Roles.Count > 0)
            {
                roleId = clerk.User.Roles.FirstOrDefault().RoleId;
            }
            ViewBag.Users = new SelectList(users, "Id", "UserName", clerk.UserId);
            ViewBag.Roles = new SelectList(roles, "Id", "Name", roleId);
            return View(clerk);
        }

        // POST: Clerks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CanAccess((int)FeaturePermissions.EditClerks)]
        public ActionResult Edit([Bind(Include = "ClerkID,FirstName,LastName,UserId")] Clerk clerk, string OldUserId)
        {
            if (ModelState.IsValid)
            {
                var user = _db.Users.FirstOrDefault(u => u.Id == clerk.UserId);

                if (clerk.UserId != OldUserId)
                {
                    if (_db.Clerks.FirstOrDefault(c => c.UserId == clerk.UserId) != null)
                    {
                        ModelState.AddModelError("", "The user " + user.UserName + " is already used with another clerk");
                        var users = _db.Users.ToList();
                        var roles = _db.Roles.ToList();
                        ViewBag.Users = new SelectList(users, "Id", "UserName");
                        ViewBag.Roles = new SelectList(roles, "Id", "Name");
                        return View(clerk);
                    }
                }

                _db.Entry(clerk).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(clerk);
        }

        // GET: Clerks/Delete/5
        [CanAccess((int)FeaturePermissions.DeleteClerks)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clerk clerk = _db.Clerks.Find(id);
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
            Clerk clerk = _db.Clerks.Find(id);
            if (clerk != null) _db.Clerks.Remove(clerk);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}