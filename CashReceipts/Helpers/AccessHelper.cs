using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CashReceipts.Models;

namespace CashReceipts.Helpers
{
    public class AccessHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public List<RoleFeaturePermission> UserFeatures;
        public AccessHelper()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                HttpContext.Current.Session["AccessFeatures"] = null;
                if (HttpContext.Current.Session["AccessFeatures"] == null)
                {
                    var userName = HttpContext.Current.User.Identity.Name;
                    var user = db.Users.Where(u => u.UserName == userName).FirstOrDefault();
                    UserFeatures = db.RolesPermissions.Where(r => r.RoleId == user.RoleId).ToList();
                    HttpContext.Current.Session["AccessFeatures"] = UserFeatures;
                }
                else
                    UserFeatures = (List<RoleFeaturePermission>)HttpContext.Current.Session["AccessFeatures"];
            }
        }
    }
}