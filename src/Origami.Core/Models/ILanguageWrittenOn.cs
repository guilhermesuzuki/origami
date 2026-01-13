namespace Origami.Core.Models
{
    /// <summary>
    /// interface for which language the content was written on
    /// </summary>
    public interface ILanguageWrittenOn
    {
        /// <summary>
        /// Language it was written on
        /// </summary>
        string LanguageWrittenOn { get; set; }
    }
}
