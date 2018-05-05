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
    public class EntitiesController : BaseController
    {
        // GET: Entities
        [CanAccess((int)FeaturePermissions.EntitiesIndex)]
        public ActionResult Index()
        {
            ViewBag.isCreate = HasAccess(FeaturePermissions.CreateEntity);
            ViewBag.isEdit = HasAccess(FeaturePermissions.EditEntity);
            ViewBag.isDetails = HasAccess(FeaturePermissions.ViewEntity);
            ViewBag.isDelete = HasAccess(FeaturePermissions.DeleteEntity);
            return View(_db.Entities.ToList());
        }

        // GET: Entities/Details/5
        [CanAccess((int)FeaturePermissions.ViewEntity)]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entity entity = _db.Entities.Find(id);
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
                _db.Entities.Add(entity);
                _db.SaveChanges();
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
            Entity entity = _db.Entities.Find(id);
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
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
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
            Entity entity = _db.Entities.Find(id);
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
            Entity entity = _db.Entities.Find(id);
            _db.Entities.Remove(entity);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
