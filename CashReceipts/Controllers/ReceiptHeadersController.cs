using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CashReceipts.Models;
using CashReceipts.Filters;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using CashReceipts.Helpers;
using System.Data.Entity.Infrastructure;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace CashReceipts.Controllers
{
    [Authorize]
    public class ReceiptHeadersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly LookupHelper _lookupHelper;

        public ReceiptHeadersController()
        {
            _lookupHelper = new LookupHelper(db);
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
            ReceiptHeader receiptHeader = db.ReceiptHeaders.Include(x => x.Tenders).Include(x => x.ReceiptBodyRecords).FirstOrDefault(x => x.ReceiptHeaderID == id);
            if (receiptHeader == null)
            {
                return HttpNotFound();
            }
            return View(receiptHeader);
        }

        // GET: ReceiptHeaders/Create
        public ActionResult Create()
        {
            ViewBag.ClerkID = new SelectList(db.Clerks, "ClerkID", "FirstName");
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
                db.ReceiptHeaders.Add(receiptHeader);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClerkID = new SelectList(db.Clerks, "ClerkID", "FirstName", receiptHeader.ClerkID);
            return View(receiptHeader);
        }

        // GET: ReceiptHeaders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiptHeader receiptHeader = db.ReceiptHeaders.Include(x => x.Tenders).Include(x => x.ReceiptBodyRecords).FirstOrDefault(x => x.ReceiptHeaderID == id);
            if (receiptHeader == null)
            {
                return HttpNotFound();
            }
            TempData["HeaderId"] = id;
            ViewBag.ClerkID = new SelectList(db.Clerks, "ClerkID", "FirstName", receiptHeader.ClerkID);
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
                db.Entry(receiptHeader).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClerkID = new SelectList(db.Clerks, "ClerkID", "FirstName", receiptHeader.ClerkID);
            return View(receiptHeader);
        }

        // GET: ReceiptHeaders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiptHeader receiptHeader = db.ReceiptHeaders.Find(id);
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
            ReceiptHeader receiptHeader = db.ReceiptHeaders.Find(id);
            db.ReceiptHeaders.Remove(receiptHeader);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AddReceiptBody(int id)
        {
            ReceiptBody receiptbody = new ReceiptBody();
            receiptbody.ReceiptHeaderID = id;
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentID", "Name");
            ViewBag.TemplateID = new SelectList(db.Templates, "TemplateID", "Description");
            return View(receiptbody);
        }

        [HttpPost, ActionName("AddReceiptBody")]
        public ActionResult AddReceiptBody(ReceiptBody receiptbody)
        {
            if (ModelState.IsValid)
            {
                db.ReceiptBodies.Add(receiptbody);
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = receiptbody.ReceiptHeaderID });
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentID", "Name");
            ViewBag.TemplateID = new SelectList(db.Templates, "TemplateID", "Description", receiptbody.TemplateID);
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
                db.Tenders.Add(tenders);
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = tenders.ReceiptHeaderID });
            }
            return View(tenders);
        }

        [NoCache]
        public ActionResult GetDepartmentTemplates(int id)
        {
            var templates = db.Templates.Where(x => x.DepartmentID == id).Select(x => new { TemplateId = x.TemplateID, TemplateName = x.Description }).ToList();
            return Json(templates, JsonRequestBehavior.AllowGet);
        }

        #region Receipt Header Grid Actions
        [NoCache]
        public ActionResult GetDepartmentsList()
        {
            var departmentsList = db.Departments.ToList()
                .Select(x => new { value = x.DepartmentID, text = x.Name }).ToList();
            return Json(departmentsList, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult GetClerksList()
        {
            var clerksList = db.Clerks
                .Select(x => new { value = x.ClerkID, text = x.LastName + ", " + x.FirstName }).ToList();
            return Json(clerksList, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult ReceiptHeaders_Read([DataSourceRequest] DataSourceRequest request)
        {
            var receiptHeaders = db.ReceiptHeaders
                .Select(x => new { x.ReceiptHeaderID, x.ClerkID, ReceiptDate = DbFunctions.TruncateTime(x.ReceiptDate),
                    x.ReceiptTotal, x.ReceiptNumber, x.DepartmentID, x.Comments }).ToList();
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
                    if (db.ReceiptHeaders.Any(x => x.ReceiptNumber == lastReciptId + 1))
                    {
                        ModelState.AddModelError("_addKey", "A receipt with same number is found in database!");
                        break;
                    }
                    receiptHeader.ReceiptNumber = lastReciptId + 1;
                    _lookupHelper.LastReceiptId = receiptHeader.ReceiptNumber;
                    var templatesList = db.Templates.Where(x => x.DepartmentID == receiptHeader.DepartmentID).ToList();
                    foreach (var template in templatesList)
                    {
                        receiptHeader.ReceiptBodyRecords.Add(new ReceiptBody
                        {
                            LineTotal = 0,
                            TemplateID = template.TemplateID
                        });
                    }
                    db.ReceiptHeaders.Add(receiptHeader);
                    try
                    {
                        if (db.SaveChanges() <= 0)
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
                    x.Comments
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
                    db.Entry(receiptHeader).State = EntityState.Modified;
                    try
                    {
                        if (db.SaveChanges() <= 0)
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
                    var receiptHeaderInDb = db.ReceiptHeaders.SingleOrDefault(x => x.ReceiptHeaderID == receiptHeader.ReceiptHeaderID);
                    if (receiptHeaderInDb != null)
                    {
                        db.ReceiptHeaders.Remove(receiptHeaderInDb);
                        try
                        {
                            if (db.SaveChanges() <= 0)
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
            var departmentsList = db.Departments.ToList()
                .Select(x => new { x.DepartmentID, Description = x.Name }).ToList();
            return Json(departmentsList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult Templates_Read([DataSourceRequest] DataSourceRequest request, int? departmentId)
        {
            var templatesList = db.Templates.Where(x => !departmentId.HasValue || x.DepartmentID == departmentId).ToList()
                .Select(x => new { x.TemplateID, Description = GetTemplateText(x) }).ToList();
            return Json(templatesList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult GetTemplatesList(bool? includeAccounts = true)
        {
            var templatesList = db.Templates.Include(x => x.Department).ToList()
                .Select(x => new
                {
                    value = x.TemplateID,
                    text = includeAccounts.HasValue && includeAccounts.Value? GetTemplateText(x) : x.Description,
                    DepartmentId = x.DepartmentID,
                    DepartmentName = x.Department.Name,
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
            var template = db.Templates.SingleOrDefault(x => x.TemplateID == templateID);
            return template?.Order ?? 0;
        }
        private string GetTemplateAccountNumber(int templateID)
        {
            var template = db.Templates.SingleOrDefault(x => x.TemplateID == templateID);
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
        public ActionResult ReceiptsBody_Read([DataSourceRequest] DataSourceRequest request)
        {
            var receiptBodies = db.ReceiptBodies.Include(x => x.Template).ToList()
                .Select(x => new
                {
                    x.ReceiptHeaderID,
                    x.ReceiptBodyID,
                    x.LineTotal,
                    x.TemplateID,
                    x.Template.DepartmentID,
                    AccountNumber = x.TemplateID,
                    TemplateOrder = x.Template.Order
                }).ToList();
            return Json(receiptBodies.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ReceiptsBody_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<ReceiptBody> receiptBodies, int receiptHeaderId)
        {
            var receiptBodiesList = receiptBodies as List<ReceiptBody> ?? receiptBodies.ToList();
            if (receiptBodiesList.Any() && ModelState.IsValid)
            {
                foreach (var receiptBody in receiptBodiesList)
                {
                    receiptBody.ReceiptHeaderID = receiptHeaderId;
                    db.ReceiptBodies.Add(receiptBody);
                    try
                    {
                        if (db.SaveChanges() <= 0)
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

            return Json(receiptBodiesList.Select(
                    x => new
                    {
                        x.ReceiptHeaderID,
                        x.ReceiptBodyID,
                        x.LineTotal,
                        x.TemplateID,
                        AccountNumber = x.TemplateID,
                        TemplateOrder = GetTemplateOrder(x.TemplateID)
                    }).ToList().ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult ReceiptsBody_Update([DataSourceRequest] DataSourceRequest request, IEnumerable<ReceiptBody> receiptBodies)
        {
            var receiptBodiesList = receiptBodies as List<ReceiptBody> ?? receiptBodies.ToList();
            if (receiptBodiesList.Any() && ModelState.IsValid)
            {
                foreach (var receiptBody in receiptBodiesList)
                {
                    db.Entry(receiptBody).State = EntityState.Modified;
                    try
                    {
                        if (db.SaveChanges() <= 0)
                            ModelState.AddModelError("_updateKey", "Can't update this receipt body to database");
                    }
                    catch (Exception)
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
                        AccountNumber = x.TemplateID,
                        TemplateOrder = GetTemplateOrder(x.TemplateID)
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
                    var receiptBodyInDb = db.ReceiptBodies.SingleOrDefault(x => x.ReceiptBodyID == receiptBody.ReceiptBodyID);
                    if (receiptBodyInDb != null)
                    {
                        db.ReceiptBodies.Remove(receiptBodyInDb);
                        try
                        {
                            if (db.SaveChanges() <= 0)
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
            var paymentMethods = db.TenderPaymentMethods
                .Select(x => new { value = x.Id, text = x.Name }).ToList();
            return Json(paymentMethods, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult ReceiptsTenders_Read([DataSourceRequest] DataSourceRequest request)
        {
            var receiptTenders = db.Tenders
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
                    db.Tenders.Add(receiptTender);
                    try
                    {
                        if (db.SaveChanges() <= 0)
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
                    db.Entry(receiptTender).State = EntityState.Modified;
                    try
                    {
                        if (db.SaveChanges() <= 0)
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
                    var receiptTenderInDb = db.Tenders.SingleOrDefault(x => x.TenderID == receiptTender.TenderID);
                    if (receiptTenderInDb != null)
                    {
                        db.Tenders.Remove(receiptTenderInDb);
                        try
                        {
                            if (db.SaveChanges() <= 0)
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
            var receipt = db.ReceiptHeaders.Include(x => x.ReceiptBodyRecords)
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
                Colspan = 7,
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
                Colspan = 7,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });
            dataTable.AddCell(new PdfPCell(new Phrase(" ", font))
            {
                Colspan = 7,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

            cell = new PdfPCell(new Phrase("Received From", font))
            {
                Colspan = 4,
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
                Colspan = 3,
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
                Colspan = 7,
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
            dataTable.AddCell(new PdfPCell(new Phrase("Description", boldFont))
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
                Colspan = 2,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });

            dataTable.AddCell(new PdfPCell(new Phrase("Paid", boldFont))
            {
                Colspan = 2,
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
                    Border = 0,
                    PaddingTop = paddingTop,
                    PaddingBottom = paddingBottom,
                    PaddingLeft = paddingLeft,
                    PaddingRight = paddingRight
                });
                dataTable.AddCell(new PdfPCell(new Phrase(receiptBody.Template.Description, font))
                {
                    Colspan = 2,
                    Border = 0,
                    PaddingTop = paddingTop,
                    PaddingBottom = paddingBottom,
                    PaddingLeft = paddingLeft,
                    PaddingRight = paddingRight
                });
                dataTable.AddCell(new PdfPCell(new Phrase(receiptBody.Template.BaseElementObjectDetail, font))
                {
                    Colspan = 2,
                    Border = 0,
                    PaddingTop = paddingTop,
                    PaddingBottom = paddingBottom,
                    PaddingLeft = paddingLeft,
                    PaddingRight = paddingRight
                });

                dataTable.AddCell(new PdfPCell(new Phrase($"{receiptBody.LineTotal:0.00}", font))
                {
                    Colspan = 2,
                    Border = 0,
                    PaddingTop = paddingTop,
                    PaddingBottom = paddingBottom,
                    PaddingLeft = paddingLeft,
                    PaddingRight = paddingRight,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                });
            }
            dataTable.AddCell(new Paragraph(" ", font));
            dataTable.AddCell(new PdfPCell(new Phrase("Total", font))
            {
                Colspan = 5,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });
            dataTable.AddCell(new PdfPCell(new Phrase($"{receipt.ReceiptBodyRecords.Sum(x => x.LineTotal):0.00}", font))
            {
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight,
                HorizontalAlignment = Element.ALIGN_RIGHT
            });
            dataTable.AddCell(new PdfPCell(new Phrase(" ", font))
            {
                Colspan = 7,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });
            dataTable.AddCell(new PdfPCell(new Phrase(" ", font))
            {
                Colspan = 3,
                Rowspan = receipt.Tenders.Count,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });
            dataTable.AddCell(new PdfPCell(new Phrase("Tender:", font))
            {
                Rowspan = receipt.Tenders.Count,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });

            foreach (var tender in receipt.Tenders.ToList())
            {
                dataTable.AddCell(new PdfPCell(new Phrase($"{tender.Description}({tender.PaymentMethod.Name})", font))
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

            dataTable.AddCell(new PdfPCell(new Phrase(" ", font))
            {
                Colspan = 7,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });
            dataTable.AddCell(new PdfPCell(new Phrase(" ", font))
            {
                Colspan = 7,
                Border = 0,
                PaddingTop = paddingTop,
                PaddingBottom = paddingBottom,
                PaddingLeft = paddingLeft,
                PaddingRight = paddingRight
            });
            dataTable.AddCell(new PdfPCell(new Phrase("© 2016 - Cash Receipting", font))
            {
                Colspan = 7,
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
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
