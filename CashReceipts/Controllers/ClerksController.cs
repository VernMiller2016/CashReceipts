using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CashReceipts.Filters;
using CashReceipts.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using CashReceipts.Helpers;

namespace CashReceipts.Controllers
{
    [Authorize]
    public class ClerksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public AccessHelper access;
        public ClerksController()
        {
            access = new AccessHelper();
        }

        // GET: Clerks
        [CanAccess((int)FeaturePermissions.ClerksIndex)]
        public ActionResult Index()
        {
            ViewBag.isCreate = access.UserFeatures.Where(f => f.FeatureId == (int)FeaturePermissions.EditClerks).FirstOrDefault() == null ? false : true;
            ViewBag.isEdit = access.UserFeatures.Where(f => f.FeatureId == (int)FeaturePermissions.CreateClerks).FirstOrDefault() == null ? false : true;
            ViewBag.isDetails = access.UserFeatures.Where(f => f.FeatureId == (int)FeaturePermissions.ViewClerks).FirstOrDefault() == null ? false : true;
            ViewBag.isDelete = access.UserFeatures.Where(f => f.FeatureId == (int)FeaturePermissions.DeleteClerks).FirstOrDefault() == null ? false : true;
            return View(db.Clerks.Include(x => x.User).ToList());
        }

        // GET: Clerks/Details/5
        [CanAccess((int)FeaturePermissions.ViewClerks)]
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
        [CanAccess((int)FeaturePermissions.CreateClerks)]
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

                if (db.Clerks.Where(c => c.UserId == UserID).FirstOrDefault() != null)
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
            string roleId = "";
            if (clerk.User != null && clerk.User.Roles.Count > 0)
            {
                roleId = clerk.User.Roles.FirstOrDefault().RoleId;
            }
            ViewBag.Users = new SelectList(Users, "Id", "UserName", clerk.UserId);
            ViewBag.Roles = new SelectList(Roles, "Id", "Name", roleId);
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
                var user = db.Users.Where(u => u.Id == clerk.UserId).FirstOrDefault();

                if (clerk.UserId != OldUserId)
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
        [CanAccess((int)FeaturePermissions.DeleteClerks)]
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

    public enum FeaturePermissions
    {
        UsersIndex = 1,
        EditUserRole = 2,
        ClerksIndex = 3,
        CreateClerks = 4,
        EditClerks = 5,
        ViewClerks = 6,
        DeleteClerks = 7,
        EntitiesIndex = 8,
        CreateEntity = 9,
        EditEntity = 10,
        ViewEntity = 11,
        DeleteEntity = 12,
        DepartmentIndex = 13,
        CreateDepartment = 14,
        EditDepartment = 15,
        ViewDepartment = 16,
        DeleteDepartment = 17,
        SystemAccountIndex = 18,
        EditSystemAccount = 19,
        ViewSystemAccount = 20,
        DeleteSystemAccount = 21,
        ManageReceiptsIndex = 22,
        AddNewReceipt = 23,
        DownloadReceipt = 24,
        PostReceipt = 25,
        AddReceiptItem = 26,
        EditReceiptItem = 27,
        DeleteReceiptItem = 28,
        AddTenderItem = 44,
        EditTenderItem = 45,
        DeleteTenderItem = 46,
        AddReceiptBody = 48,
        EditReceiptBody = 49,
        DeleteReceiptBody = 50,
        SearchLineItemIndex = 29,
        ExportLineItem = 30,
        ShowReceipt = 31,
        ReceiptsExportIndex = 32,
        ReceiptsExport = 33,
        LineItemsExport = 34,
        TendersExport = 35,
        ReceiptsDetailsExport = 36,
        DaySummaryReportIndex = 37,
        ExportAndPrintSummary = 38,
        AuditsIndex = 39,
        GrantCountyAccountIndex = 40,
        DistrictsAccountIndex = 41,
        PrintReceipt = 42,
        LockReceipt = 51,
        LockReceipts = 52
    }
}
