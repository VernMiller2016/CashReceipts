using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using CashReceipts.Models;
using System.Data.Entity;
using CashReceipts.Filters;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace CashReceipts.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private ApplicationDbContext db;
        public AccountsController() : this(new ApplicationDbContext())
        {

        }
        public AccountsController(ApplicationDbContext context)
        {
            db = context;
        }

        // GET: Templates
        public ActionResult Index(int? SelectedDepartment)
        {
            var departments = db.Departments.OrderBy(q => q.Name).ToList();
            ViewBag.SelectedDepartment = new SelectList(departments, "DepartmentID", "Name", SelectedDepartment);
            int departmentID = SelectedDepartment.GetValueOrDefault();

            IQueryable<Template> templates = db.Templates
                .Where(c => !SelectedDepartment.HasValue || c.DepartmentID == departmentID)
                .OrderBy(d => d.TemplateID)
                .Include(d => d.Department);
            var sql = templates.ToString();
            return View(templates.ToList());
        }

        // GET: Templates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Template template = db.Templates.Include(x => x.Department).Single(m => m.TemplateID == id);

            if (template == null)
            {
                return HttpNotFound();
            }
            return View(template);
        }

        // GET: Templates/Create
        public ActionResult Create()
        {
            PopulateDepartmentsDropDownList();
            return View();
        }

        // POST: Templates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Template template)
        {
            if (ModelState.IsValid)
            {
                db.Templates.Add(template);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateDepartmentsDropDownList();
            return View(template);
        }

        // GET: Templates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Template template = db.Templates.Single(m => m.TemplateID == id);
            if (template == null)
            {
                return HttpNotFound();
            }
            PopulateDepartmentsDropDownList(template.DepartmentID);
            return View(template);
        }

        // POST: Templates/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Template template)
        {
            if (ModelState.IsValid)
            {
                db.Entry(template).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateDepartmentsDropDownList();
            return View(template);
        }

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in db.Departments
                                   orderby d.Name
                                   select d;
            ViewBag.DepartmentID = new SelectList(departmentsQuery, "DepartmentID", "Name", selectedDepartment);
        }

        // GET: Templates/Delete/5
        [ActionName("Delete")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Template template = db.Templates.Single(m => m.TemplateID == id);
            if (template == null)
            {
                return HttpNotFound();
            }

            return View(template);
        }

        // POST: Templates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Template template = db.Templates.Single(m => m.TemplateID == id);
            db.Templates.Remove(template);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult GlAccounts()
        {
            return View();
        }

        [NoCache]
        public ActionResult GlAccounts_Read([DataSourceRequest] DataSourceRequest request, int? skip, int? take,
            string value, string @operator, string field)
        {
            var isAzure = bool.Parse(ConfigurationManager.AppSettings["IsAzure"] ?? "false");
            var dbName = isAzure ? "" : "GC.";
            var rowsTotal = db.Database.SqlQuery<int>($"Select count(*) from {dbName}dbo.GL00100 where Active = 1");
            var colIndex = ColumnOrders.All;
            if (!string.IsNullOrEmpty(@operator) && !string.IsNullOrEmpty(value))
            {
                switch (field)
                {
                    case "Fund":
                        colIndex = ColumnOrders.Fund;
                        break;
                    case "Dept":
                        colIndex = ColumnOrders.Dept;
                        break;
                    case "Program":
                        colIndex = ColumnOrders.Program;
                        break;
                    case "Project":
                        colIndex = ColumnOrders.Project;
                        break;
                    case "BaseElementObjectDetail":
                        colIndex = ColumnOrders.BaseElementObjectDetail;
                        break;
                    case "Description":
                        colIndex = ColumnOrders.Description;
                        break;
                }
            }
            var templates =
           db.GetGCAccounts(colIndex, value??string.Empty, take??20, skip??0)
           .ToList();

            return Json(new {Data= templates, Total=rowsTotal}, JsonRequestBehavior.AllowGet);
        }
        
    }
}
