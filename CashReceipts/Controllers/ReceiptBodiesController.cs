﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CashReceipts.Models;

namespace CashReceipts.Controllers
{
    public class ReceiptBodiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //// GET: ReceiptBodies
        //public ActionResult Index()
        //{
        //    var receiptBody = db.ReceiptBody.Include(r => r.ReceiptHeaders).Include(r => r.Templates);
        //    return View(receiptBody.ToList());
        //}

        // GET: ReceiptBodies
        public ActionResult Index(int? SelectedTemplate)
        {
            var templates = db.Template.OrderBy(q => q.Description).ToList();
            ViewBag.SelectedTemplate = new SelectList(templates, "TemplateID", "Description", SelectedTemplate);
            int templateID = SelectedTemplate.GetValueOrDefault();

            IQueryable<ReceiptBody> receiptbodies = db.ReceiptBody
                .Where(c => !SelectedTemplate.HasValue || c.TemplateID == templateID)
                .OrderBy(d => d.ReceiptBodyID)
                .Include(d => d.Templates);
            var sql = templates.ToString();
            return View(templates.ToList());
        }

        // GET: ReceiptBodies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiptBody receiptBody = db.ReceiptBody.Find(id);
            if (receiptBody == null)
            {
                return HttpNotFound();
            }
            return View(receiptBody);
        }

        // GET: ReceiptBodies/Create
        public ActionResult Create()
        {
            ViewBag.ReceiptHeaderID = new SelectList(db.ReceiptHeader, "ReceiptHeaderID", "ReceiptHeaderID");
            ViewBag.TemplateID = new SelectList(db.Template, "TemplateID", "Description");
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
                db.ReceiptBody.Add(receiptBody);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ReceiptHeaderID = new SelectList(db.ReceiptHeader, "ReceiptHeaderID", "ReceiptHeaderID", receiptBody.ReceiptHeaderID);
            ViewBag.TemplateID = new SelectList(db.Template, "TemplateID", "Description", receiptBody.TemplateID);
            return View(receiptBody);
        }

        // GET: ReceiptBodies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiptBody receiptBody = db.ReceiptBody.Include(x=>x.ReceiptDetails).FirstOrDefault(x=>x.ReceiptBodyID == id);
            if (receiptBody == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReceiptHeaderID = new SelectList(db.ReceiptHeader, "ReceiptHeaderID", "ReceiptHeaderID", receiptBody.ReceiptHeaderID);
            ViewBag.TemplateID = new SelectList(db.Template, "TemplateID", "Description", receiptBody.TemplateID);
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
                db.Entry(receiptBody).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ReceiptHeaderID = new SelectList(db.ReceiptHeader, "ReceiptHeaderID", "ReceiptHeaderID", receiptBody.ReceiptHeaderID);
            ViewBag.TemplateID = new SelectList(db.Template, "TemplateID", "Description", receiptBody.TemplateID);
            return View(receiptBody);
        }

        // GET: ReceiptBodies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiptBody receiptBody = db.ReceiptBody.Find(id);
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
            ReceiptBody receiptBody = db.ReceiptBody.Find(id);
            db.ReceiptBody.Remove(receiptBody);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void PopulateTemplatesDropDownList(object selectedTemplate = null)
        {
            var templatesQuery = from d in db.Template
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
