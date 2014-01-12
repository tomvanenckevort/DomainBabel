using System.Web.Mvc;

namespace DomainBabel
{
    /// <summary>
    /// Filter configuration class.
    /// </summary>
    public static class FilterConfig
    {
        /// <summary>
        /// Registers global filters.
        /// </summary>
        /// <param name="filters">List of global filters to add to</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}