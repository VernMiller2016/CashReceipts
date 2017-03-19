using CashReceipts.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using CashReceipts.Filters;

namespace CashReceipts.Controllers
{
    [IsAdmin]
    public class PermissionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Permissions
        public ActionResult Index()
        {
            var roles = db.Roles.ToList();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View();
        }

        [NoCache]
        public ActionResult GetScreens(string roleId)
        {
            var screens = db.Screens.Include(s => s.Features).Include(x => x.Features.Select(y => y.Roles)).ToList();

            List<TreeViewModel> tvList = new List<TreeViewModel>();
            foreach (var item in screens)
            {
                var tv = new TreeViewModel
                {
                    id = item.Id * 1000,
                    text = item.Name,
                    spriteCssClass = "folder",
                    expanded = true
                };
                if (item.Features != null)
                {
                    tv.items = new List<TreeModel>();

                    foreach (var feature in item.Features)
                    {
                        TreeModel tv2 = new TreeModel
                        {
                            id = feature.Id,
                            text = feature.Name,
                            selected = feature.Roles.Any(x => x.RoleId == roleId)
                        };
                        //tv2.spriteCssClass = "folder";
                        //tv2.expanded = true;
                        tv.items.Add(tv2);
                    }
                }
                tvList.Add(tv);
            }

            var val = string.Join(",", tvList.SelectMany(x => x.items).Where(x => x.selected).Select(x => x.id));
            return Json(new { Screens = tvList, ScreensValue = val }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SavePermissions(string nodeIds, string roleId)
        {
            var result = false;
            string[] featureIds = nodeIds.Split(',').Distinct().ToArray();
            var existingFeatures = db.ScreenFeatures.Include(r => r.Roles).Where(r => r.Roles.Any(ro => ro.RoleId == roleId)).ToList();
            var allScreenFeatures = db.ScreenFeatures.ToList();
            RoleFeaturePermission rfp = new RoleFeaturePermission();
            rfp.RoleId = roleId;
            try
            {
                foreach (var item in allScreenFeatures)
                {
                    var feature = existingFeatures.FirstOrDefault(f => f.Id == item.Id);
                    if (featureIds.FirstOrDefault(s => s == item.Id.ToString()) != null)
                    {
                        if (feature == null)
                        {
                            rfp.FeatureId = item.Id;
                            if (item.Roles == null)
                                item.Roles = new List<RoleFeaturePermission>();
                            item.Roles.Add(rfp);
                            db.SaveChanges();
                        }
                    }
                    else if (feature != null)
                    {
                        rfp = db.RolesPermissions.FirstOrDefault(rp => rp.FeatureId == feature.Id && rp.RoleId == roleId);
                        // rfp.FeatureId = feature.Id;
                        db.RolesPermissions.Remove(rfp);
                        db.SaveChanges();
                    }
                    rfp = new RoleFeaturePermission { RoleId = roleId };
                }
                result = true;
            }
            catch
            {
                result = false;
            }
            return Json(new {Success = result});
        }

    }
}
