using System.Linq;
using System.Web.Mvc;
using CashReceipts.Models;

namespace CashReceipts.Filters
{
    public class IsAdminAttribute : ActionFilterAttribute
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var userName = filterContext.HttpContext.User.Identity.Name;
            var user = _db.Users.FirstOrDefault(u => u.UserName == userName);
            if (user?.RoleId != "6eae8487-db3e-46af-af20-1a63307ae86c")
                filterContext.Result = new RedirectResult("~/Error/NotAuthorized");
            base.OnActionExecuting(filterContext);
        }
    }
}