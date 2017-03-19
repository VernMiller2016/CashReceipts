using CashReceipts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace CashReceipts.Controllers
{
    public class CanAccessAttribute : ActionFilterAttribute
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private int featureId { get; set; }
        public CanAccessAttribute(int id )
        {
            featureId = id;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var userName = filterContext.HttpContext.User.Identity.Name;
            var user = db.Users.Where(u => u.UserName == userName).FirstOrDefault();
            var userFeatures = db.RolesPermissions.Where(r => r.RoleId == user.RoleId).ToList();
            if (userFeatures == null || userFeatures.Where(f => f.FeatureId == featureId).FirstOrDefault() == null)//filterContext.HttpContext.User.Identity.Name
                filterContext.Result = new RedirectResult("~/Error/NotAuthorized");
            base.OnActionExecuting(filterContext);
        }
        //public override void OnAuthorization(AuthorizationContext filterContext)
        //{
        //    var userName = filterContext.HttpContext.User.Identity.Name;
        //    var user = db.Users.Where(u => u.UserName == userName).FirstOrDefault();
        //    var userFeatures = db.RolesPermissions.Where(r => r.RoleId == user.RoleId).ToList();
        //    if (userFeatures==null || userFeatures.Where(f=>f.Id==featureId).FirstOrDefault()==null)//filterContext.HttpContext.User.Identity.Name
        //        filterContext.Result = new RedirectResult("~/Error/UsernotFound");
        //    base.OnAuthorization(filterContext);

        //}
    }
}
