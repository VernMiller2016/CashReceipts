using CashReceipts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace CashReceipts.Filters
{
    public class AccessAttribute : ActionFilterAttribute
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            

        }
    }
}