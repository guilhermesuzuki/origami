namespace Origami.Core.Data
{
    public interface IRssRepository
    {
        /// <summary>
        /// Gets the RSS feed for a blog
        /// </summary>
        /// <param name="slug">Blog slug</param>
        /// <param name="oi">the origami base URL</param>
        /// <returns></returns>
        string GetRss(string slug, string oi);
    }
}
