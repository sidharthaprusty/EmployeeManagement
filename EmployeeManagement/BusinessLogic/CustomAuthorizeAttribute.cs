using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeManagement.BusinessLogic
{
    public class CustomAuthorizeAttribute: AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            IsAuthorized(filterContext);
            
        }

        void IsAuthorized(AuthorizationContext filterContext)
        {
            //User is authorized
            if(filterContext.Result == null)
            {
                return;
            }

            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult(System.Web.Security.FormsAuthentication.LoginUrl + "?returnUrl=" + filterContext.HttpContext.Request.Url);
                return;
            }
        }
    }
}