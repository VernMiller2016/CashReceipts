using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CashReceipts.Models;

namespace CashReceipts.Helpers
{
    public class AccessHelper
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public List<RoleFeaturePermission> UserFeatures { get; set; }

        public AccessHelper()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (HttpContext.Current.Session["AccessFeatures"] == null)
                {
                    var userName = HttpContext.Current.User.Identity.Name;
                    var user = _db.Users.FirstOrDefault(u => u.UserName == userName);
                    UserFeatures = _db.RolesPermissions.Where(r => r.RoleId == user.RoleId).ToList();
                    HttpContext.Current.Session["AccessFeatures"] = UserFeatures;
                }
                else
                    UserFeatures = (List<RoleFeaturePermission>)HttpContext.Current.Session["AccessFeatures"];
            }
        }

        public const string AdminRoleId = "6eae8487-db3e-46af-af20-1a63307ae86c";
    }
}