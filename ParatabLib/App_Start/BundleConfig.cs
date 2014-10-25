using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
namespace ParatabLib
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Bundles").Include(
                "~/Scripts/jquery*").Include("~/Scripts/LayoutScript.js"));
            bundles.Add(new ScriptBundle("~/ApiBundles").Include(
                "~/Scripts/ApiScript.js"));
            bundles.Add(new StyleBundle("~/StyleBundles").Include(
                "~/Content/*.css"));
            BundleTable.EnableOptimizations = true;
        }
    }
}