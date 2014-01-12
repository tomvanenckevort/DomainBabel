using System.Data.Services.Common;

namespace DomainBabel
{
    /// <summary>
    /// Language container class.
    /// </summary>
    [DataServiceEntity]
    public class Language
    {
        /// <summary>
        /// Gets or sets the language ISO code.
        /// </summary>
        public string Code { get; set; }
    }
}