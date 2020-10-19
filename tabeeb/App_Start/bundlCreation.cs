using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
namespace tabeeb.App_Start
{
    public class bundlCreation
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/validationLinks").Include(
            "~/Scripts/jquery.validate.js",
            "~/Scripts/jquery.validate.unobtrusive.js"
            ));
            bundles.Add(new ScriptBundle("~/bootstrapSelectJs").Include(
            "~/Scripts/bootstrap-select.min.js"
            ));
            bundles.Add(new StyleBundle("~/bootstrapSelectCss").Include("~/css/bootstrap-select.min.css"));
            BundleTable.EnableOptimizations = true;//to hide link fron inspector
        }
    }
}