using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CashReceipts.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CashReceipts.Controllers
{
    [Authorize]
    public class ClerksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Clerks
        public ActionResult Index()
        {
            return View(db.Clerks.Include(x=>x.User).ToList());
        }

        // GET: Clerks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clerk clerk = db.Clerks.Find(id);
            if (clerk == null)
            {
                return HttpNotFound();
            }
            return View(clerk);
        }

        // GET: Clerks/Create
        public ActionResult Create()
        {
            var Users = db.Users.ToList();
            var Roles = db.Roles.ToList();
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
                var user = db.Users.Where(u => u.Id == UserID).FirstOrDefault();

                if (db.Clerks.Where(c=>c.UserId==UserID).FirstOrDefault() !=null)
                {
                    ModelState.AddModelError("", "The user " + user.UserName + " is already used with another clerk");
                    var Users = db.Users.ToList();
                    var Roles = db.Roles.ToList();
                    ViewBag.Users = new SelectList(Users, "Id", "UserName");
                    ViewBag.Roles = new SelectList(Roles, "Id", "Name");
                    return View(clerk);
                }
               
                clerk.UserId = UserID;
                db.Clerks.Add(clerk);
                db.SaveChanges();
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
            Clerk clerk = db.Clerks.Find(id);
            if (clerk == null)
            {
                return HttpNotFound();
            }
            var Users = db.Users.ToList();
            var Roles = db.Roles.ToList();
            string roleId="";
            if (clerk.User != null && clerk.User.Roles.Count > 0)
            {
                roleId = clerk.User.Roles.FirstOrDefault().RoleId;
            }
            ViewBag.Users = new SelectList(Users, "Id", "UserName",clerk.UserId);
            ViewBag.Roles = new SelectList(Roles, "Id", "Name",roleId);
            return View(clerk);
        }

        // POST: Clerks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClerkID,FirstName,LastName,UserId")] Clerk clerk, string OldUserId)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Where(u => u.Id == clerk.UserId).FirstOrDefault();

                if(clerk.UserId!= OldUserId)
                {
                    if (db.Clerks.Where(c => c.UserId == clerk.UserId).FirstOrDefault() != null)
                    {
                        ModelState.AddModelError("", "The user " + user.UserName + " is already used with another clerk");
                        var Users = db.Users.ToList();
                        var Roles = db.Roles.ToList();
                        ViewBag.Users = new SelectList(Users, "Id", "UserName");
                        ViewBag.Roles = new SelectList(Roles, "Id", "Name");
                        return View(clerk);
                    }
                }

                db.Entry(clerk).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(clerk);
        }

        // GET: Clerks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clerk clerk = db.Clerks.Find(id);
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
            Clerk clerk = db.Clerks.Find(id);
            db.Clerks.Remove(clerk);
            db.SaveChanges();
            return RedirectToAction("Index");
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
