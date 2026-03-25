using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IPageTitleRepository
    {
        /// <summary>
        /// Gets the current page title.
        /// </summary>
        /// <returns></returns>
        string GetTitle();
        void SetTitle(string? page);
        void SetTitle(string? category, string? page);
        void SetTitle(ITitle? page);
        void SetTitle(IName? category, ITitle? page);
        void SetTitle(ITag? tag, ITitle? page);
        void SetTitle(ITag? tag, IName? category, ITitle? page);
        void SetTitle(params string?[] parts);
    }
}
