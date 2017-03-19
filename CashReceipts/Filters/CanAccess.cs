using System.Linq;
using System.Web.Mvc;
using CashReceipts.Models;

namespace CashReceipts.Filters
{
    public class CanAccessAttribute : ActionFilterAttribute
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private int FeatureId { get; set; }

        public CanAccessAttribute(int id )
        {
            FeatureId = id;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var userName = filterContext.HttpContext.User.Identity.Name;
            var user = _db.Users.FirstOrDefault(u => u.UserName == userName);
            var userFeatures = _db.RolesPermissions.Where(r => r.RoleId == user.RoleId).ToList();
            if (userFeatures?.FirstOrDefault(f => f.FeatureId == FeatureId) == null)
                filterContext.Result = new RedirectResult("~/Error/NotAuthorized");
            base.OnActionExecuting(filterContext);
        }
    }
}
