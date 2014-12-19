using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlinTuriCab.Models;

namespace AlinTuriCab.Helpers
{
    public class ValidateRequestAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            new CabDataContext().ValidateCookie();
            if (LoginHelper.Client == null)
            {
                var result = new RedirectResult("/logout");
                result.ExecuteResult(filterContext.Controller.ControllerContext);
            }
            base.OnActionExecuting(filterContext);
        }
    }
}