using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CashReceipts.Filters
{
    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            context.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        }
    }
}