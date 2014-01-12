using System;
using System.Collections.ObjectModel;
using Microsoft.WindowsAzure.Storage.Table;  

namespace DomainBabel
{
    /// <summary>
    /// Translation entity for storing translations in Azure Table storage.
    /// </summary>
    public class TranslationEntity : TableEntity
    {
        /// <summary>
        /// Separator string used to store multiple translations in a single string.
        /// </summary>
        private const string TranslationSeparator = "||";

        /// <summary>
        /// Gets or sets the language of the translation (stored in the PartitionKey).
        /// </summary>
        [IgnoreProperty]
        public string Language 
        {
            get
            {
                return this.PartitionKey;
            }

            set
            {
                this.PartitionKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the text part of the translation (stored in the RowKey).
        /// </summary>
        [IgnoreProperty]
        public string Part
        {
            get
            {
                return this.RowKey;
            }

            set
            {
                this.RowKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the found translation texts.
        /// </summary>
        public string Translations { get; set; }

        /// <summary>
        /// Gets the found translation texts by splitting the string.
        /// </summary>
        [IgnoreProperty]
        public ReadOnlyCollection<string> Translation 
        {
            get
            {
                if (this.Translations == null)
                {
                    return null;
                }
                else
                {
                    return new ReadOnlyCollection<string>(this.Translations.Split(new string[] { TranslationSeparator }, StringSplitOptions.RemoveEmptyEntries));
                }
            }
        }

        /// <summary>
        /// Sets the found translation texts by joining the array.
        /// </summary>
        /// <param name="translations">List of translations</param>
        public void SetTranslations(string[] translations)
        {
            if (translations == null)
            {
                this.Translations = null;
            }
            else
            {
                this.Translations = string.Join(TranslationSeparator, translations);
            }
        }
    }
}