using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CashReceipts.Models;

namespace CashReceipts.Controllers
{
    [Authorize]
    public class ReceiptBodiesController : BaseController
    {
        // GET: ReceiptBodies/Create
        public ActionResult Create()
        {
            ViewBag.ReceiptHeaderID = new SelectList(_db.ReceiptHeaders, "ReceiptHeaderID", "ReceiptHeaderID");
            ViewBag.TemplateID = new SelectList(_db.Templates, "TemplateID", "Description");
            return View();
        }

        // POST: ReceiptBodies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReceiptBodyID,LineTotal,ReceiptHeaderID,TemplateID")] ReceiptBody receiptBody)
        {
            if (ModelState.IsValid)
            {
                _db.ReceiptBodies.Add(receiptBody);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ReceiptHeaderID = new SelectList(_db.ReceiptHeaders, "ReceiptHeaderID", "ReceiptHeaderID", receiptBody.ReceiptHeaderID);
            ViewBag.TemplateID = new SelectList(_db.Templates, "TemplateID", "Description", receiptBody.TemplateID);
            return View(receiptBody);
        }

        // GET: ReceiptBodies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiptBody receiptBody = _db.ReceiptBodies.FirstOrDefault(x=>x.ReceiptBodyID == id);
            if (receiptBody == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReceiptHeaderID = new SelectList(_db.ReceiptHeaders, "ReceiptHeaderID", "ReceiptHeaderID", receiptBody.ReceiptHeaderID);
            ViewBag.TemplateID = new SelectList(_db.Templates, "TemplateID", "Description", receiptBody.TemplateID);
            return View(receiptBody);
        }

        // POST: ReceiptBodies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReceiptBodyID,LineTotal,ReceiptHeaderID,TemplateID")] ReceiptBody receiptBody)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(receiptBody).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ReceiptHeaderID = new SelectList(_db.ReceiptHeaders, "ReceiptHeaderID", "ReceiptHeaderID", receiptBody.ReceiptHeaderID);
            ViewBag.TemplateID = new SelectList(_db.Templates, "TemplateID", "Description", receiptBody.TemplateID);
            return View(receiptBody);
        }

        // GET: ReceiptBodies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiptBody receiptBody = _db.ReceiptBodies.Find(id);
            if (receiptBody == null)
            {
                return HttpNotFound();
            }
            return View(receiptBody);
        }

        // POST: ReceiptBodies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReceiptBody receiptBody = _db.ReceiptBodies.Find(id);
            _db.ReceiptBodies.Remove(receiptBody);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void PopulateTemplatesDropDownList(object selectedTemplate = null)
        {
            var templatesQuery = from d in _db.Templates
                                   orderby d.Description
                                   select d;
            ViewBag.TemplateID = new SelectList(templatesQuery, "TemplateID", "Description", selectedTemplate);
        }

        //private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        //{
        //    var departmentsQuery = from d in db.Department
        //                           orderby d.Name
        //                           select d;
        //    ViewBag.DepartmentID = new SelectList(departmentsQuery, "DepartmentID", "Name", selectedDepartment);
        //}
        
    }
}
