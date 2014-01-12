using System.Diagnostics.CodeAnalysis;
using Owin;

[assembly: Microsoft.Owin.OwinStartup(typeof(DomainBabel.StartUp))]

namespace DomainBabel
{
    /// <summary>
    /// OWIN startup class.
    /// </summary>
    public class StartUp
    {
        /// <summary>
        /// Configures OWIN for application startup.
        /// </summary>
        /// <param name="app">OWN App Builder</param>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Won't execute when static.")]
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
