using System.ComponentModel.DataAnnotations;

namespace DomainBabel
{
    /// <summary>
    /// View model for the Home view.
    /// </summary>
    public class HomeModel
    {
        /// <summary>
        /// Gets or sets the search text (domain name) used for finding translations.
        /// </summary>
        [Required(ErrorMessage = "Please enter a domain name")]
        [RegularExpression(@"^[a-zA-Z0-9][a-zA-Z0-9-]{1,65}[a-zA-Z0-9]\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid domain name (minimum length of 3 characters)")]
        public string SearchText { get; set; }
    }
}