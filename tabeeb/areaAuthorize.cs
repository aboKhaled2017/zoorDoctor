using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using tabeeb.Controllers;
namespace tabeeb
{
    public class areaAuthorize : AuthorizeAttribute
{
    private readonly string area;

    public areaAuthorize(string area)
    {
        this.area = area;
    }

    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
        string loginUrl = "";

        if (area == "admin")
        {
            loginUrl = "/admin/account/logIn";
        }
        else if (area == "doctors")
        {
            loginUrl = "/"+globalController.defaultPathForDoctorsArea+"/register";
        }
        else if (area == "users")
        {
            loginUrl = "/"+globalController.defaultPathForUsersArea;
        }
        filterContext.Result = new RedirectResult(loginUrl);
    }
    }
}