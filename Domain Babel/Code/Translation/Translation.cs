using System.Data.Services.Common;

namespace DomainBabel
{
    /// <summary>
    /// Translation container class.
    /// </summary>
    [DataServiceEntity]
    public class Translation
    {
        /// <summary>
        /// Gets or sets the translation text.
        /// </summary>
        public string Text { get; set; }
    }
}