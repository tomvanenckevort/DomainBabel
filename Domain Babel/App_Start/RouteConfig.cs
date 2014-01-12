using System.Web.Mvc;
using System.Web.Routing;

namespace DomainBabel
{
    /// <summary>
    /// Route configuration class.
    /// </summary>
    public static class RouteConfig
    {
        /// <summary>
        /// Adds routes to global route collection.
        /// </summary>
        /// <param name="routes">Route collection to add to</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}