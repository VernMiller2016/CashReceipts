using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CashReceipts.Models;
using CashReceipts.Filters;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using CashReceipts.Helpers;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CashReceipts.Controllers
{
    [Authorize]
    public class ReceiptHeadersController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly LookupHelper _lookupHelper;
        /// <summary>
        /// User manager - attached to application DB context
        /// </summary>
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public ReceiptHeadersController()
        {
            _lookupHelper = new LookupHelper(_db);
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            //ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext()
            //    .GetUserManager<ApplicationUserManager>().FindById(
            //        System.Web.HttpContext.Current.User.Identity.GetUserId());
        }

        // GET: ReceiptHeaders
        public ActionResult Index()
        {
            return View(_lookupHelper.LastReceiptId);
        }

        // GET: ReceiptHeaders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiptHeader receiptHeader = _db.ReceiptHeaders.Include(x => x.Tenders).Include(x => x.ReceiptBodyRecords).FirstOrDefault(x => x.ReceiptHeaderID == id);
            if (receiptHeader == null)
            {
                return HttpNotFound();
            }
            return View(receiptHeader);
        }

        // GET: ReceiptHeaders/Create
        public ActionResult Create()
        {
            ViewBag.ClerkID = new SelectList(_db.Clerks, "ClerkID", "FirstName");
            return View();
        }

        // POST: ReceiptHeaders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReceiptHeaderID,ReceiptDate,ReceiptTotal,ClerkID")] ReceiptHeader receiptHeader)
        {
            if (ModelState.IsValid)
            {
                _db.ReceiptHeaders.Add(receiptHeader);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClerkID = new SelectList(_db.Clerks, "ClerkID", "FirstName", receiptHeader.ClerkID);
            return View(receiptHeader);
        }

        // GET: ReceiptHeaders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiptHeader receiptHeader = _db.ReceiptHeaders.Include(x => x.Tenders).Include(x => x.ReceiptBodyRecords).FirstOrDefault(x => x.ReceiptHeaderID == id);
            if (receiptHeader == null)
            {
                return HttpNotFound();
            }
            TempData["HeaderId"] = id;
            ViewBag.ClerkID = new SelectList(_db.Clerks, "ClerkID", "FirstName", receiptHeader.ClerkID);
            return View(receiptHeader);
        }

        // POST: ReceiptHeaders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReceiptHeaderID,ReceiptDate,ReceiptTotal,ClerkID")] ReceiptHeader receiptHeader)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(receiptHeader).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClerkID = new SelectList(_db.Clerks, "ClerkID", "FirstName", receiptHeader.ClerkID);
            return View(receiptHeader);
        }

        // GET: ReceiptHeaders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiptHeader receiptHeader = _db.ReceiptHeaders.Find(id);
            if (receiptHeader == null)
            {
                return HttpNotFound();
            }
            return View(receiptHeader);
        }

        // POST: ReceiptHeaders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReceiptHeader receiptHeader = _db.ReceiptHeaders.Find(id);
            _db.ReceiptHeaders.Remove(receiptHeader);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AddReceiptBody(int id)
        {
            ReceiptBody receiptbody = new ReceiptBody();
            receiptbody.ReceiptHeaderID = id;
            ViewBag.DepartmentId = new SelectList(_db.Departments, "DepartmentID", "Name");
            ViewBag.TemplateID = new SelectList(_db.Templates, "TemplateID", "Description");
            return View(receiptbody);
        }

        [HttpPost, ActionName("AddReceiptBody")]
        public ActionResult AddReceiptBody(ReceiptBody receiptbody)
        {
            if (ModelState.IsValid)
            {
                _db.ReceiptBodies.Add(receiptbody);
                _db.SaveChanges();
                return RedirectToAction("Edit", new { id = receiptbody.ReceiptHeaderID });
            }
            ViewBag.DepartmentId = new SelectList(_db.Departments, "DepartmentID", "Name");
            ViewBag.TemplateID = new SelectList(_db.Templates, "TemplateID", "Description", receiptbody.TemplateID);
            return View(receiptbody);
        }

        public ActionResult AddTenders(int id)
        {
            Tender tenders = new Tender();
            tenders.ReceiptHeaderID = id;
            return View(tenders);
        }

        [HttpPost, ActionName("AddTenders")]
        public ActionResult AddTenders(Tender tenders)
        {
            if (ModelState.IsValid)
            {
                _db.Tenders.Add(tenders);
                _db.SaveChanges();
                return RedirectToAction("Edit", new { id = tenders.ReceiptHeaderID });
            }
            return View(tenders);
        }

        [NoCache]
        public ActionResult GetDepartmentTemplates(int id)
        {
            var templates = _db.Templates.Where(x => x.DepartmentID == id).Select(x => new { TemplateId = x.TemplateID, TemplateName = x.Description }).ToList();
            return Json(templates, JsonRequestBehavior.AllowGet);
        }

        #region Receipt Header Grid Actions
        [NoCache]
        public ActionResult GetDepartmentsList()
        {
            var departmentsList = _db.Departments.ToList()
                .Select(x => new { value = x.DepartmentID, text = x.Name }).ToList();
            return Json(departmentsList, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult GetClerksList()
        {
            var clerksList = _db.Clerks
                .Select(x => new { value = x.ClerkID, text = x.LastName + ", " + x.FirstName }).ToList();
            return Json(clerksList, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult ReceiptHeaders_Read([DataSourceRequest] DataSourceRequest request, int? receiptHeaderId=null)
        {
            var receiptHeaders = _db.ReceiptHeaders
                .Where(x => !x.IsDeleted && (!receiptHeaderId.HasValue || x.ReceiptHeaderID == receiptHeaderId.Value))
                .Select(x => new
                {
                    x.ReceiptHeaderID,
                    x.ClerkID,
                    ReceiptDate = DbFunctions.TruncateTime(x.ReceiptDate),
                    x.ReceiptTotal,
                    x.ReceiptNumber,
                    x.DepartmentID,
                    x.Comments,
                    x.ReceivedFor
                }).ToList();
            return Json(receiptHeaders.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ReceiptHeaders_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<ReceiptHeader> receiptHeaders)
        {
            var lastReciptId = _lookupHelper.LastReceiptId;
            var receiptHeadersList = receiptHeaders as List<ReceiptHeader> ?? receiptHeaders.ToList();
            if (receiptHeadersList.Any() && ModelState.IsValid)
            {
                foreach (var receiptHeader in receiptHeadersList)
                {
                    if (_db.ReceiptHeaders.Any(x => x.ReceiptNumber == lastReciptId + 1))
                    {
                        ModelState.AddModelError("_addKey", "A receipt with same number is found in database!");
                        break;
                    }
                    receiptHeader.ReceiptNumber = lastReciptId + 1;
                    _lookupHelper.LastReceiptId = receiptHeader.ReceiptNumber;
                    var templatesList = _db.Templates.Where(x => x.DepartmentID == receiptHeader.DepartmentID).ToList();
                    foreach (var template in templatesList)
                    {
                        receiptHeader.ReceiptBodyRecords.Add(new ReceiptBody
                        {
                            LineTotal = 0,
                            TemplateID = template.TemplateID,
                            AccountDescription = template.Description
                        });
                    }
                    _db.ReceiptHeaders.Add(receiptHeader);
                    try
                    {
                        if (_db.SaveChanges() <= 0)
                        {
                            //todo supports localization
                            ModelState.AddModelError("_addKey", "Can't add this receipt header to database");
                        }
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        ModelState.AddModelError("_addKey", "A receipt with same number is found in database!");
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("_addKey", "Can't add this receipt header to database");
                    }
                }
            }

            return Json(receiptHeadersList.Select(
                x => new
                {
                    x.ReceiptHeaderID,
                    x.ClerkID,
                    ReceiptDate = x.ReceiptDate.Date,
                    x.ReceiptTotal,
                    x.ReceiptNumber,
                    x.DepartmentID,
                    x.Comments,
                    x.ReceivedFor
                }).ToList().ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult ReceiptHeaders_Update([DataSourceRequest] DataSourceRequest request, IEnumerable<ReceiptHeader> receiptHeaders)
        {
            var receiptHeadersList = receiptHeaders as List<ReceiptHeader> ?? receiptHeaders.ToList();
            if (receiptHeadersList.Any() && ModelState.IsValid)
            {
                foreach (var receiptHeader in receiptHeadersList)
                {
                    _db.Entry(receiptHeader).State = EntityState.Modified;
                    try
                    {
                        if (_db.SaveChanges() <= 0)
                        {
                            ModelState.AddModelError("_updateKey", "Can't update this receipt header to database");
                        }
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("_updateKey", "Can't update this receipt header to database");
                    }
                }
            }
            return Json(receiptHeadersList.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult ReceiptHeaders_Destroy([DataSourceRequest] DataSourceRequest request, IEnumerable<ReceiptHeader> receiptHeaders)
        {
            var receiptHeadersList = receiptHeaders as List<ReceiptHeader> ?? receiptHeaders.ToList();
            if (receiptHeadersList.Any() && ModelState.IsValid)
            {
                foreach (var receiptHeader in receiptHeadersList)
                {
                    var receiptHeaderInDb = _db.ReceiptHeaders.SingleOrDefault(x => x.ReceiptHeaderID == receiptHeader.ReceiptHeaderID);
                    if (receiptHeaderInDb != null)
                    {
                        var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                        receiptHeaderInDb.IsDeleted = true;
                        _db.Audits.Add(new Audit
                        {
                            EntityId = receiptHeaderInDb.ReceiptHeaderID.ToString(),
                            EntityType = SysEntityType.ReceiptHeader,
                            OperationType = OperationType.Delete,
                            UserId = userId,
                            ActionDate = DateTime.Now
                        });
                        try
                        {
                            if (_db.SaveChanges() <= 0)
                            {
                                ModelState.AddModelError("_deleteKey", "Can't remove this receipt header from database");
                            }
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("_deleteKey", "Can't remove this receipt header from database");
                        }
                    }
                }
            }
            return Json(receiptHeadersList.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Receipt Body Grid Actions
        [NoCache]
        public ActionResult Departments_Read([DataSourceRequest] DataSourceRequest request)
        {
            var departmentsList = _db.Departments.ToList()
                .Select(x => new { value = x.DepartmentID, text = x.Name }).ToList();
            return Json(departmentsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult Templates_Read([DataSourceRequest] DataSourceRequest request, int? departmentId)
        {
            var templatesList = _db.Templates.Where(x => !departmentId.HasValue || x.DepartmentID == departmentId).ToList()
                .Select(x => new { x.TemplateID, Description = GetTemplateText(x) }).ToList();
            return Json(templatesList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult GetTemplatesList(bool? includeAccounts = true)
        {
            var templatesList = _db.Templates.Include(x => x.Department).ToList()
                .Select(x => new
                {
                    value = x.TemplateID,
                    text = includeAccounts.HasValue && includeAccounts.Value ? GetTemplateText(x) : x.Description,
                    DepartmentId = x.DepartmentID,
                    DepartmentName = x.Department?.Name,
                    AccountNumber = GetAccountNumber(x)
                }).ToList();
            return Json(templatesList, JsonRequestBehavior.AllowGet);
        }

        private string GetTemplateText(Template template)
        {
            return
                $"{template.Fund}.{template.Dept}.{template.Program}.{template.Project}.{template.BaseElementObjectDetail} | {template.Description}";
        }

        private string GetAccountNumber(Template template)
        {
            return
                $"{template.Fund}.{template.Dept}.{template.Program}.{template.Project}.{template.BaseElementObjectDetail}";
        }

        private int GetTemplateOrder(int templateID)
        {
            var template = _db.Templates.SingleOrDefault(x => x.TemplateID == templateID);
            return template?.Order ?? 0;
        }
        private string GetTemplateAccountNumber(int templateID)
        {
            var template = _db.Templates.SingleOrDefault(x => x.TemplateID == templateID);
            if (template != null)
                return GetTemplateAccountNumber(template);
            return string.Empty;
        }

        private string GetTemplateAccountNumber(Template template)
        {
            return $"{template.Fund}.{template.Dept}.{template.Program}.{template.Project}.{template.BaseElementObjectDetail}";
        }

        private string GetTemplateFundDept(Template template)
        {
            return $"{template.Fund}.{template.Dept}.{template.Program}.{template.Project}";
        }

        [NoCache]
        public ActionResult ReceiptsBody_Read([DataSourceRequest] DataSourceRequest request, int? receiptHeaderId=null)
        {
            var receiptBodies = _db.ReceiptBodies.Include(x => x.Template)
                .Where(x=> !receiptHeaderId.HasValue || x.ReceiptHeaderID == receiptHeaderId.Value)
                .ToList()
                .Select(x => new
                {
                    x.ReceiptHeaderID,
                    x.ReceiptBodyID,
                    x.LineTotal,
                    x.TemplateID,
                    AccountDescription = x.AccountDescription,
                    AccountNumber = GetTemplateAccountNumber(x.Template),
                    x.Template.DepartmentID,
                    TemplateOrder = x.Template.Order,
                    AccountDataSource = AccountDataSource.Local
                }).ToList();
            return Json(receiptBodies.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ReceiptsBody_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<ReceiptBody> receiptBodies, int receiptHeaderId)
        {
            var receiptBodiesList = receiptBodies as List<ReceiptBody> ?? receiptBodies.ToList();
            if (receiptBodiesList.Any() && ModelState.IsValid)
            {
                var receipt = _db.ReceiptHeaders.Include(x => x.Department).Single(x => x.ReceiptHeaderID == receiptHeaderId);
                foreach (var receiptBody in receiptBodiesList)
                {
                    receiptBody.ReceiptHeaderID = receiptHeaderId;
                    var template = _db.GetRemoteAccount(receiptBody.TemplateID, receiptBody.AccountDataSource);
                    if (template != null)
                    {
                        var localTemplate = receipt.Department.Templates.FirstOrDefault(x => x.Fund == template.Fund &&
                                                                               x.BaseElementObjectDetail ==
                                                                               template.BaseElementObjectDetail &&
                                                                               x.Dept == template.Dept
                                                                               && x.Program == template.Program &&
                                                                               x.Project == template.Project);
                        if (localTemplate != null)
                        {
                            receiptBody.TemplateID = localTemplate.TemplateID;
                            //localTemplate.Description = receiptBody.AccountDescription;
                        }
                        else
                        {
                            var newTemplate = new Template
                            {
                                BaseElementObjectDetail = template.BaseElementObjectDetail,
                                Dept = template.Dept,
                                Description = receiptBody.AccountDescription,
                                Fund = template.Fund,
                                Program = template.Program,
                                Project = template.Project,
                                Order = 0,
                                DataSource = template.DataSource
                            };
                            //receipt.Department.Templates.Add(newTemplate);
                            receiptBody.TemplateID = newTemplate.TemplateID;
                            receiptBody.Template = newTemplate;
                        }
                        _db.ReceiptBodies.Add(receiptBody);
                        try
                        {
                            if (_db.SaveChanges() <= 0)
                            {
                                //todo supports localization
                                ModelState.AddModelError("_addKey", "Can't add this receipt body to database");
                            }
                        }
                        catch (Exception e)
                        {
                            ModelState.AddModelError("_addKey", "Can't add this receipt body to database");
                        }
                    }
                }
            }

            return Json(receiptBodiesList.Select(
                    x => new
                    {
                        x.ReceiptHeaderID,
                        x.ReceiptBodyID,
                        x.LineTotal,
                        x.TemplateID,
                        TemplateOrder = GetTemplateOrder(x.TemplateID),
                        AccountDescription = x.AccountDescription,
                        AccountNumber = GetTemplateAccountNumber(x.Template),
                        x.Template.DepartmentID,
                        AccountDataSource = AccountDataSource.Local,
                        IsRemote = false
                    }).ToList().ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult ReceiptsBody_Update([DataSourceRequest] DataSourceRequest request,
            IEnumerable<ReceiptBody> receiptBodies)
        {
            var receiptBodiesList = receiptBodies as List<ReceiptBody> ?? receiptBodies.ToList();
            if (receiptBodiesList.Any() && ModelState.IsValid)
            {
                foreach (var receiptBody in receiptBodiesList)
                {
                    var receipt =
                        _db.ReceiptHeaders.Include(x => x.Department)
                            .Single(x => x.ReceiptHeaderID == receiptBody.ReceiptHeaderID);

                    if (receiptBody.IsRemote)
                    {
                        var template = _db.GetRemoteAccount(receiptBody.TemplateID, receiptBody.AccountDataSource);
                        if (template != null)
                        {
                            var localTemplate =
                                receipt.Department.Templates.FirstOrDefault(x => x.Fund == template.Fund &&
                                                                                 x.BaseElementObjectDetail ==
                                                                                 template
                                                                                     .BaseElementObjectDetail &&
                                                                                 x.Dept == template.Dept
                                                                                 &&
                                                                                 x.Program ==
                                                                                 template.Program &&
                                                                                 x.Project ==
                                                                                 template.Project);
                            if (localTemplate != null)
                            {
                                receiptBody.TemplateID = localTemplate.TemplateID;
                                //localTemplate.Description = receiptBody.AccountDescription;
                            }
                            else
                            {
                                var newTemplate = new Template
                                {
                                    BaseElementObjectDetail = template.BaseElementObjectDetail,
                                    Dept = template.Dept,
                                    Description = receiptBody.AccountDescription,
                                    Fund = template.Fund,
                                    Program = template.Program,
                                    Project = template.Project,
                                    Order = 0,
                                    DataSource = template.DataSource
                                };
                                //receipt.Department.Templates.Add(newTemplate);
                                receiptBody.TemplateID = newTemplate.TemplateID;
                                receiptBody.Template = newTemplate;
                            }
                        }
                    }
                    //else
                    //{
                    //    var localTemplate = _db.Templates.Find(receiptBody.TemplateID);
                    //    if (localTemplate != null)
                    //    {
                    //        localTemplate.Description = receiptBody.AccountDescription;
                    //    }
                    //}
                    _db.Entry(receiptBody).State = EntityState.Modified;
                    try
                    {
                        if (_db.SaveChanges() <= 0)
                            ModelState.AddModelError("_updateKey", "Can't update this receipt body to database");
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("_updateKey", "Can't update this receipt body to database");
                    }
                }
            }
            return Json(receiptBodiesList.Select(
                x => new
                {
                    x.ReceiptHeaderID,
                    x.ReceiptBodyID,
                    x.LineTotal,
                    x.TemplateID,
                    TemplateOrder = GetTemplateOrder(x.TemplateID),
                    AccountDescription = x.AccountDescription,
                    AccountNumber = GetTemplateAccountNumber(x.Template),
                    x.Template.DepartmentID,
                    AccountDataSource = AccountDataSource.Local,
                    IsRemote = false
                }).ToList().ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult ReceiptsBody_Destroy([DataSourceRequest] DataSourceRequest request, IEnumerable<ReceiptBody> receiptBodies)
        {
            var receiptBodiesList = receiptBodies as List<ReceiptBody> ?? receiptBodies.ToList();
            if (receiptBodiesList.Any() && ModelState.IsValid)
            {
                foreach (var receiptBody in receiptBodiesList)
                {
                    var receiptBodyInDb = _db.ReceiptBodies.SingleOrDefault(x => x.ReceiptBodyID == receiptBody.ReceiptBodyID);
                    if (receiptBodyInDb != null)
                    {
                        _db.ReceiptBodies.Remove(receiptBodyInDb);
                        try
                        {
                            if (_db.SaveChanges() <= 0)
                                ModelState.AddModelError("_deleteKey", "Can't remove this receipt body from database");
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("_deleteKey", "Can't remove this receipt body from database");
                        }
                    }
                }
            }
            return Json(receiptBodiesList.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Receipt Tenders Grid Actions
        [NoCache]
        public ActionResult GetPaymentMethods()
        {
            var paymentMethods = _db.TenderPaymentMethods
                .Select(x => new { value = x.Id, text = x.Name }).ToList();
            return Json(paymentMethods, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult ReceiptsTenders_Read([DataSourceRequest] DataSourceRequest request, int? receiptHeaderId = null)
        {
            var receiptTenders = _db.Tenders
                .Where(x => !receiptHeaderId.HasValue || x.ReceiptHeaderID == receiptHeaderId.Value)
                .Select(x => new { x.ReceiptHeaderID, x.Amount, x.Description, x.TenderID, x.PaymentMethodId }).ToList();
            return Json(receiptTenders.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ReceiptsTenders_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<Tender> receiptTenders, int receiptHeaderId)
        {
            var receiptTendersList = receiptTenders as List<Tender> ?? receiptTenders.ToList();
            if (receiptTendersList.Any() && ModelState.IsValid)
            {
                foreach (var receiptTender in receiptTendersList)
                {
                    receiptTender.ReceiptHeaderID = receiptHeaderId;
                    _db.Tenders.Add(receiptTender);
                    try
                    {
                        if (_db.SaveChanges() <= 0)
                        {
                            //todo supports localization
                            ModelState.AddModelError("_addKey", "Can't add this tender to database");
                        }
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("_addKey", "Can't add this tender to database");
                    }
                }
            }

            return Json(receiptTendersList.Select(
                    x => new { x.ReceiptHeaderID, x.Amount, x.Description, x.TenderID, x.PaymentMethodId }).ToList().ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult ReceiptsTenders_Update([DataSourceRequest] DataSourceRequest request, IEnumerable<Tender> receiptTenders)
        {
            var receiptTendersList = receiptTenders as List<Tender> ?? receiptTenders.ToList();
            if (receiptTendersList.Any() && ModelState.IsValid)
            {
                foreach (var receiptTender in receiptTendersList)
                {
                    _db.Entry(receiptTender).State = EntityState.Modified;
                    try
                    {
                        if (_db.SaveChanges() <= 0)
                            ModelState.AddModelError("_updateKey", "Can't update this tender to database");
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("_updateKey", "Can't update this tender to database");
                    }
                }
            }
            return Json(receiptTendersList.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult ReceiptsTenders_Destroy([DataSourceRequest] DataSourceRequest request, IEnumerable<Tender> receiptTenders)
        {
            var receiptTendersList = receiptTenders as List<Tender> ?? receiptTenders.ToList();
            if (receiptTendersList.Any() && ModelState.IsValid)
            {
                foreach (var receiptTender in receiptTendersList)
                {
                    var receiptTenderInDb = _db.Tenders.SingleOrDefault(x => x.TenderID == receiptTender.TenderID);
                    if (receiptTenderInDb != null)
                    {
                        _db.Tenders.Remove(receiptTenderInDb);
                        try
                        {
                            if (_db.SaveChanges() <= 0)
                                ModelState.AddModelError("_deleteKey", "Can't remove this tender from database");
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("_deleteKey", "Can't remove this tender from database");
                        }
                    }
                }
            }
            return Json(receiptTendersList.ToDataSourceResult(request, ModelState));
        }

        #endregion

        [HttpPost]
        public ActionResult DownloadReceipt(int receiptHeaderId)
        {
            var receipt = _db.ReceiptHeaders
                .Include(x => x.Clerk)
                .Include(x => x.Department)
                .Include(x => x.ReceiptBodyRecords)
                .Include(x => x.ReceiptBodyRecords.Select(y => y.Template))
                .Include(x => x.Tenders).Include(x => x.Tenders.Select(y => y.PaymentMethod))
                .SingleOrDefault(x => x.ReceiptHeaderID == receiptHeaderId);
            if (receipt == null)
                return Content("");

            int paddingTop = 3, paddingBottom = 3, paddingLeft = 0, paddingRight = 0;

            //Start PDF Process
            var document = new Document(PageSize.A4, 5, 5, 5, 5);
            var filestream = new MemoryStream();
            var writer = PdfWriter.GetInstance(document, filestream);
            writer.CloseStream = false;
            document.Open();

            string fontpath = Server.MapPath("~/fonts/OpenSans-Regular.ttf");
            BaseFont customfont = BaseFont.CreateFont(fontpath, BaseFont.CP1252, BaseFont.EMBEDDED);
            var font = new Font(customfont, 10);
            var boldFont = new Font(customfont, 12);

            const int numOfColumns = 7;
            var dataTable = new PdfPTable(numOfColumns);
            dataTable.DefaultCell.PaddingTop = paddingTop;
            dataTable.DefaultCell.PaddingBottom = paddingBottom;
            dataTable.DefaultCell.PaddingLeft = paddingLeft;
            dataTable.DefaultCell.PaddingRight = paddingRight;
            dataTable.DefaultCell.Border = 0;
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            dataTable.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;

            // Adding headers
            //string imagepath = Server.MapPath("~/Images");
            //Image header = Image.GetInstance(imagepath + "/PASlogo.png");
            //header.ScalePercent(75f);
            //header.Alignment = 1;
            //document.Add(header);

            dataTable.SpacingBefore = 10f;
            var cell = new PdfPCell(new Phrase(receipt.Department.Name, font))
            {
                Colspan = numOfColumns,
                HorizontalAlignment = 1,
                VerticalAlignment = 1,
                //BackgroundColor = new BaseColor(24, 145, 238),
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            };
            dataTable.AddCell(cell);

            dataTable.AddCell(new PdfPCell(new Phrase(" ", font))
            {
                Colspan = numOfColumns,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });
            dataTable.AddCell(new PdfPCell(new Phrase(" ", font))
            {
                Colspan = numOfColumns,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

            cell = new PdfPCell(new Phrase("Received From", font))
            {
                Colspan = 3,
                HorizontalAlignment = 1,
                VerticalAlignment = 1,
                //BackgroundColor = BaseColor.LIGHT_GRAY,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            };
            dataTable.AddCell(cell);
            dataTable.AddCell(new PdfPCell(new Phrase("", font))
            {
                Colspan = 4,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });

            cell = new PdfPCell(new Phrase(receipt.Comments, font))
            {
                Colspan = 4,
                Rowspan = 4,
                HorizontalAlignment = 1,
                VerticalAlignment = 3,
                //BackgroundColor = BaseColor.LIGHT_GRAY,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            };
            dataTable.AddCell(cell);
            dataTable.AddCell(new PdfPCell(new Phrase("**ORIGINAL**", font))
            {
                Colspan = 2,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });
            dataTable.AddCell(new Paragraph(" ", font));

            dataTable.AddCell(new PdfPCell(new Phrase("Receipt Number", font))
            {
                Colspan = 2,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });
            dataTable.AddCell(new Paragraph(receipt.ReceiptNumber.ToString(), font));
            dataTable.AddCell(new PdfPCell(new Phrase("Receipt Date", font))
            {
                Colspan = 2,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });
            dataTable.AddCell(new Paragraph(receipt.ReceiptDate.ToString("MM/dd/yyyy"), font));
            dataTable.AddCell(new PdfPCell(new Phrase("Clerk", font))
            {
                Colspan = 2,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });
            dataTable.AddCell(new Paragraph($"{receipt.Clerk.LastName}, {receipt.Clerk.FirstName}", font));

            dataTable.AddCell(new PdfPCell(new Phrase(" ", font))
            {
                Colspan = numOfColumns,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });

            dataTable.AddCell(new PdfPCell(new Phrase("Fund/Dept", boldFont))
            {
                Colspan = 2,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });

            dataTable.AddCell(new PdfPCell(new Phrase("Revenue", boldFont))
            {
                Colspan = 1,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });

            dataTable.AddCell(new PdfPCell(new Phrase("Description", boldFont))
            {
                Colspan = 3,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });

            dataTable.AddCell(new PdfPCell(new Phrase("Paid", boldFont))
            {
                Colspan = 1,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight,
                HorizontalAlignment = Element.ALIGN_RIGHT
            });

            foreach (var receiptBody in receipt.ReceiptBodyRecords.Where(x => x.LineTotal != 0).ToList())
            {
                dataTable.AddCell(new PdfPCell(new Phrase(GetTemplateFundDept(receiptBody.Template), font))
                {
                    Colspan = 2,
                    Border = 1,
                    PaddingTop = paddingTop,
                    PaddingBottom = paddingBottom,
                    PaddingLeft = paddingLeft,
                    PaddingRight = paddingRight
                });

                dataTable.AddCell(new PdfPCell(new Phrase(receiptBody.Template.BaseElementObjectDetail, font))
                {
                    Colspan = 1,
                    Border = 1,
                    PaddingTop = paddingTop,
                    PaddingBottom = paddingBottom,
                    PaddingLeft = paddingLeft,
                    PaddingRight = paddingRight
                });

                dataTable.AddCell(new PdfPCell(new Phrase(receiptBody.Template.Description, font))
                {
                    Colspan = 3,
                    Border = 1,
                    PaddingTop = paddingTop,
                    PaddingBottom = paddingBottom,
                    PaddingLeft = paddingLeft,
                    PaddingRight = paddingRight
                });


                dataTable.AddCell(new PdfPCell(new Phrase($"{receiptBody.LineTotal:0.00}", font))
                {
                    Colspan = 1,
                    Border = 1,
                    PaddingTop = paddingTop,
                    PaddingBottom = paddingBottom,
                    PaddingLeft = paddingLeft,
                    PaddingRight = paddingRight,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                });
            }

            dataTable.AddCell(new PdfPCell(new Phrase(" ", font))
            {
                Border = 1,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });
            dataTable.AddCell(new PdfPCell(new Phrase("Total", font))
            {
                Colspan = 5,
                Border = 1,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });
            dataTable.AddCell(new PdfPCell(new Phrase($"{receipt.ReceiptBodyRecords.Sum(x => x.LineTotal):0.00}", font))
            {
                Border = 1,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight,
                HorizontalAlignment = Element.ALIGN_RIGHT
            });

            dataTable.AddCell(new PdfPCell(new Phrase(" ", font))
            {
                Colspan = numOfColumns,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });

            dataTable.AddCell(new PdfPCell(new Phrase(" ", font))
            {
                Colspan = 3,
                Rowspan = receipt.Tenders.Count + 1,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });
            dataTable.AddCell(new PdfPCell(new Phrase("Tender:", font))
            {
                Rowspan = receipt.Tenders.Count + 1,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });

            foreach (var tender in receipt.Tenders.ToList())
            {
                var phrase = !string.IsNullOrEmpty(tender.Description)
                    ? $"{tender.PaymentMethod.Name} - {tender.Description}"
                    : tender.PaymentMethod.Name;

                dataTable.AddCell(new PdfPCell(new Phrase(phrase, font))
                {
                    Colspan = 2,
                    Border = 0,
                    PaddingTop = paddingTop,
                    PaddingBottom = paddingBottom,
                    PaddingLeft = paddingLeft,
                    PaddingRight = paddingRight
                });
                dataTable.AddCell(new PdfPCell(new Phrase($"{tender.Amount:0.00}", font))
                {
                    Border = 0,
                    PaddingTop = paddingTop,
                    PaddingBottom = paddingBottom,
                    PaddingLeft = paddingLeft,
                    PaddingRight = paddingRight,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                });
            }


            dataTable.AddCell(new PdfPCell(new Phrase("Total", font))
            {
                Colspan = 2,
                Border = 1,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });

            var totalTendersAmount = receipt.Tenders.Sum(x => x.Amount);
            dataTable.AddCell(new PdfPCell(new Phrase($"{totalTendersAmount:0.00}", font))
            {
                Border = 1,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight,
                HorizontalAlignment = Element.ALIGN_RIGHT
            });

            dataTable.AddCell(new PdfPCell(new Phrase(" ", font))
            {
                Colspan = numOfColumns,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });

            dataTable.AddCell(new PdfPCell(new Phrase(" ", font))
            {
                Colspan = numOfColumns,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });

            dataTable.AddCell(new PdfPCell(new Phrase("© 2016 - Cash Receipting", font))
            {
                Colspan = 4,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });

            dataTable.AddCell(new PdfPCell(new Phrase("Print Date: " + DateTime.Now.ToString("MM/dd/yyyy H:mm:ss"), font))
            {
                Colspan = 3,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });
            //Image bottom = Image.GetInstance(imagepath + "/PASbottom.jpg");
            //bottom.ScalePercent(75f);
            //bottom.Alignment = 1;

            dataTable.SpacingAfter = 10f;
            document.Add(dataTable);
            //var cImage = new Chunk(bottom, 30, -20, true);
            //cImage.SetAnchor(GlobalSettings.SystemUrl);
            //var anchor = new Anchor(cImage) { Reference = GlobalSettings.SystemUrl };
            //document.Add(anchor);

            //Finalizing PDF
            document.Close();
            filestream.Close();
            return File(filestream.ToArray(), "pdf", $"Receipt_{receipt.ReceiptNumber}.pdf");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        public ActionResult CheckReciptHeaderTotals(int receiptHeaderId)
        {
            bool result = false;
            string msg = "";
            var receipt = _db.ReceiptHeaders
                .Include(x=>x.Tenders)
                .Include(x=>x.ReceiptBodyRecords).SingleOrDefault(x => x.ReceiptHeaderID == receiptHeaderId);
            if (receipt != null)
            {
                var bodyTotal = receipt.ReceiptBodyRecords.Sum(x => x.LineTotal);
                result = bodyTotal == receipt.Tenders.Sum(x => x.Amount) && bodyTotal == receipt.ReceiptTotal;
                msg = result ? "Values Are Equal!" : "Values Are Not Equal!";
            }
            return Json(new { Result = result, Message = msg });
        }

        public ActionResult Search()
        {
            return View();
        }

        [NoCache]
        public ActionResult LineItems_Read([DataSourceRequest] DataSourceRequest request, DateTime? fromDate=null, DateTime? toDate=null, string acctNum="")
        {
            var receiptBodies = _db.ReceiptBodies
                .Include(x => x.ReceiptHeader)
                .Include(x => x.ReceiptHeader.Department)
                .Include(x => x.Template)
                .Where(x=>x.LineTotal != 0)
                .Where(x=> !fromDate.HasValue || SqlFunctions.DateDiff("DAY", x.ReceiptHeader.ReceiptDate, fromDate) <= 0)
                .Where(x=> !toDate.HasValue || SqlFunctions.DateDiff("DAY", x.ReceiptHeader.ReceiptDate, toDate) >= 0)
                .Where(x=> string.IsNullOrEmpty(acctNum) || 
                (x.Template.Fund + x.Template.Dept + x.Template.Program + x.Template.Project + x.Template.BaseElementObjectDetail)
                .StartsWith(acctNum)).ToList()
                .Select(x => new
                {
                    x.ReceiptHeaderID,
                    ReceiptHeaderNumber = x.ReceiptHeader.ReceiptNumber,
                    ReceiptDepartment = x.ReceiptHeader.Department.Name,
                    x.ReceiptBodyID,
                    x.LineTotal,
                    x.TemplateID,
                    AccountDescription = x.Template.Description,
                    AccountNumber = GetTemplateAccountNumber(x.Template),
                    x.Template.DepartmentID,
                    TemplateOrder = x.Template.Order,
                    AccountDataSource = AccountDataSource.Local
                }).ToList();
            return Json(receiptBodies.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}
