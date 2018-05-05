using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CashReceipts.Helpers;
using CashReceipts.Models;
using CashReceipts.ViewModels;

namespace CashReceipts.Controllers
{
    public class BaseController : Controller
    {
        public AccessHelper Access;

        protected readonly ApplicationDbContext _db = new ApplicationDbContext();

        public BaseController()
        {
            Access = new AccessHelper(_db);
        }

        protected new JsonResult Json(object data)
        {
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue
            };
        }

        protected bool HasAccess(FeaturePermissions feature)
        {
            return Access.UserFeatures.FirstOrDefault(f => f.FeatureId == (int)feature) != null;
        }

        protected override void Dispose(bool disposing)
        {
            _db?.Dispose();
            base.Dispose(disposing);
        }

    }
}