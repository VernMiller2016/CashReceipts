using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CashReceipts.Filters;
using CashReceipts.Models;
using CashReceipts.Helpers;

namespace CashReceipts.Controllers
{
    [Authorize]
    public class EntitiesController : Controller
    {
        private ApplicationDbContext db;
        public AccessHelper access;
        public EntitiesController()
        {
            db = new ApplicationDbContext();
            access = new AccessHelper();
        }

        protected override void Dispose(bool disposing)
        {
            if (db != null)
                db.Dispose();
            db = null;
            base.Dispose(disposing);
        }

        // GET: Entities
        [CanAccess((int)FeaturePermissions.EntitiesIndex)]
        public ActionResult Index()
        {
            ViewBag.isCreate = access.UserFeatures.Where(f => f.FeatureId == (int)FeaturePermissions.CreateEntity).FirstOrDefault() == null ? false : true;
            ViewBag.isEdit = access.UserFeatures.Where(f => f.FeatureId == (int)FeaturePermissions.EditEntity).FirstOrDefault() == null ? false : true;
            ViewBag.isDetails = access.UserFeatures.Where(f => f.FeatureId == (int)FeaturePermissions.ViewEntity).FirstOrDefault() == null ? false : true;
            ViewBag.isDelete = access.UserFeatures.Where(f => f.FeatureId == (int)FeaturePermissions.DeleteEntity).FirstOrDefault() == null ? false : true;
            return View(db.Entities.ToList());
        }

        // GET: Entities/Details/5
        [CanAccess((int)FeaturePermissions.ViewEntity)]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entity entity = db.Entities.Find(id);
            if (entity == null)
            {
                return HttpNotFound();
            }
            return View(entity);
        }

        // GET: Entities/Create
        [CanAccess((int)FeaturePermissions.CreateEntity)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Entities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EntityID,Name,Address1,Address2,City,State,ZipCode,Telephone")] Entity entity)
        {
            if (ModelState.IsValid)
            {
                db.Entities.Add(entity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(entity);
        }

        // GET: Entities/Edit/5
        [CanAccess((int)FeaturePermissions.EditEntity)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entity entity = db.Entities.Find(id);
            if (entity == null)
            {
                return HttpNotFound();
            }
            return View(entity);
        }

        // POST: Entities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EntityID,Name,Address1,Address2,City,State,ZipCode,Telephone")] Entity entity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(entity);
        }

        // GET: Entities/Delete/5
        [CanAccess((int)FeaturePermissions.DeleteEntity)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entity entity = db.Entities.Find(id);
            if (entity == null)
            {
                return HttpNotFound();
            }
            return View(entity);
        }

        // POST: Entities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Entity entity = db.Entities.Find(id);
            db.Entities.Remove(entity);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
