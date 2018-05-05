using System.Collections.Generic;
using System.Linq;
using System.Web;
using CashReceipts.Models;
using CashReceipts.ViewModels;

namespace CashReceipts.Helpers
{
    public class AccessHelper
    {
        private readonly ApplicationDbContext _db;
        public List<RoleFeaturePermission> UserFeatures { get; set; }

        public AccessHelper(ApplicationDbContext db)
        {
            _db = db;
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (HttpContext.Current.Session["AccessFeatures"] == null)
                {
                    var user = GetCurrentUser();
                    UserFeatures = _db.RolesPermissions.Where(r => r.RoleId == user.RoleId).ToList();
                    HttpContext.Current.Session["AccessFeatures"] = UserFeatures;
                }
                else
                    UserFeatures = (List<RoleFeaturePermission>)HttpContext.Current.Session["AccessFeatures"];
            }
        }

        private ApplicationUser GetCurrentUser()
        {
            var userName = HttpContext.Current.User.Identity.Name;
            var user = _db.Users.FirstOrDefault(u => u.UserName == userName);
            return user;
        }

        public bool IsAdminUser()
        {
            var user = GetCurrentUser();
            return user?.RoleId == AdminRoleId;
        }

        public bool HasUsersIndexAccess
        {
            get { return UserFeatures.Any(x => x.FeatureId == (int)FeaturePermissions.UsersIndex); }
        }

        public bool HasSystemAccountIndexAccess
        {
            get { return UserFeatures.Any(x => x.FeatureId == (int)FeaturePermissions.SystemAccountIndex); }
        }

        public bool HasGrantCountyAccountIndexAccess
        {
            get { return UserFeatures.Any(x => x.FeatureId == (int)FeaturePermissions.GrantCountyAccountIndex); }
        }

        public bool HasDistrictsAccountIndexAccess
        {
            get { return UserFeatures.Any(x => x.FeatureId == (int)FeaturePermissions.DistrictsAccountIndex); }
        }

        public bool HasManageReceiptsIndexAccess
        {
            get { return UserFeatures.Any(x => x.FeatureId == (int)FeaturePermissions.ManageReceiptsIndex); }
        }

        public bool HasSearchLineItemIndexAccess
        {
            get { return UserFeatures.Any(x => x.FeatureId == (int)FeaturePermissions.SearchLineItemIndex); }
        }

        public bool HasReceiptsExportIndexAccess
        {
            get { return UserFeatures.Any(x => x.FeatureId == (int)FeaturePermissions.ReceiptsExportIndex); }
        }

        public bool HasDaySummaryReportIndexAccess
        {
            get { return UserFeatures.Any(x => x.FeatureId == (int)FeaturePermissions.DaySummaryReportIndex); }
        }

        public bool HasAuditsIndexAccess
        {
            get { return UserFeatures.Any(x => x.FeatureId == (int)FeaturePermissions.AuditsIndex); }
        }

        public const string AdminRoleId = "6eae8487-db3e-46af-af20-1a63307ae86c";
    }
}