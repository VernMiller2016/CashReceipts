using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using CashReceipts.Filters;
using CashReceipts.Models;
using CashReceipts.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CashReceipts.Controllers
{
    public class AuditsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public AuditsController()
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
        }

        public ActionResult Index()
        {
            return View();
        }

        [NoCache]
        public ActionResult Audits_Read([DataSourceRequest] DataSourceRequest request)
        {
            var deletedReceiptsInAudit =
                _db.Audits.Where(
                    x => x.EntityType == SysEntityType.ReceiptHeader && x.OperationType == OperationType.Delete)
                    .Select(x=>x.EntityId)
                    .ToList()
                    .Select(int.Parse);

            var deletedReceipts = _db.ReceiptHeaders
                .Where(x => deletedReceiptsInAudit.Contains(x.ReceiptHeaderID))
                .ToList();

            var receiptHeaders = _db.Audits
                .Select(x => new AuditVm{
                    Id = x.Id,
                    ActionDate = x.ActionDate,
                    UserId = x.UserId,
                    EntityType = x.EntityType,
                    OperationType = x.OperationType,
                    EntityId = x.EntityId
                }).ToList();
            foreach (var receiptHeader in receiptHeaders)
            {
                receiptHeader.ReceiptNumber =
                    deletedReceipts.FirstOrDefault(y => y.ReceiptHeaderID.ToString() == receiptHeader.EntityId)?
                        .ReceiptNumber;
            }

            return Json(receiptHeaders.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public ActionResult GetSysUsersList()
        {
            var users = UserManager.Users.Select(x => new { text = x.UserName, value = x.Id }).ToList();
            return Json(users, JsonRequestBehavior.AllowGet);
        }
    }
}