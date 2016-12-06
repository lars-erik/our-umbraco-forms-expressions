using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using Newtonsoft.Json.Serialization;

namespace Our.Umbraco.Forms.Expressions.UI
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(SetupWebApiRoutes);
        }

        private static void SetupWebApiRoutes(HttpConfiguration c)
        {
            c.MapHttpAttributeRoutes();
            //c.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new Pa();
        }

    }
}