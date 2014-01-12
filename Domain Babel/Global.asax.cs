using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SimpleInjector;

namespace DomainBabel
{
    /// <summary>
    /// Class to initialize view engine and register filters.
    /// </summary>
    public class Global : HttpApplication
    {
        /// <summary>
        /// Called when application gets started in IIS.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Won't execute when static.")]
        protected void Application_Start()
        {
            var container = new Container();

            container.Register<ICloudTable, TranslationsTable>();
            container.Register<ITableStorage, AzureTableStorage>();
            container.Register<ITranslationContainer, MicrosoftTranslatorContainer>();
            container.Register<ITranslation, MicrosoftTranslation>();

            #if DEBUG
            container.Verify();
            #endif

            DependencyResolver.SetResolver(container);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            GlobalFilters.Filters.Add(new HtmlMinifyFilterAttribute());

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        /// <summary>
        /// Called just before the response headers are being sent to client.
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Won't execute when static.")]
        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Response.Headers.Remove("X-Powered-By");
                HttpContext.Current.Response.Headers.Remove("X-AspNet-Version");
                HttpContext.Current.Response.Headers.Remove("X-AspNetMvc-Version");
                HttpContext.Current.Response.Headers.Remove("Server");
                HttpContext.Current.Response.Headers.Remove("ETag");
                HttpContext.Current.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
            }
        }
    }
}