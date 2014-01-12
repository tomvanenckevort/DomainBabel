using System.Web.Mvc;

namespace DomainBabel
{
    /// <summary>
    /// Controller containing actions for home views.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Index action.
        /// </summary>
        /// <returns>View for the Index action</returns>
        public ActionResult Index()
        {
            var model = new HomeModel();

            return this.View("Index", model);
        }
    }
}
