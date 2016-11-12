using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Our.Umbraco.Forms.Expressions.WebApi;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.UI.JavaScript;

namespace Our.Umbraco.Forms.Expressions.Startup
{
    public class RouteSetup : ApplicationEventHandler
    {
        protected override void ApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ServerVariablesParser.Parsing += AppendApiRoutes;
        }

        private void AppendApiRoutes(object sender, Dictionary<string, object> dictionary)
        {
            var helper = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));
            var urls = (Dictionary<string, object>) dictionary["umbracoUrls"];
            urls.Add("ufx-program-evaluator", helper.GetUmbracoApiServiceBaseUrl<BackofficeProgramController>(c => c.Run(null)));
        }
    }
}
