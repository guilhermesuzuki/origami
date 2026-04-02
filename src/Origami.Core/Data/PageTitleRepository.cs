using Origami.Core.Models;
using System.Text;

namespace Origami.Core.Data
{
    public class PageTitleRepository : IPageTitleRepository
    {
        private readonly ISettingsRepository _blogSettingsRepository;
        private List<string?> _parts = [];

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="blogSettingsRepository"></param>
        public PageTitleRepository(ISettingsRepository blogSettingsRepository) : base()
        {
            _blogSettingsRepository = blogSettingsRepository;
        }

        public string GetTitle()
        {
            var result = new StringBuilder(_blogSettingsRepository.GetSettings().Name);

            foreach (var part in _parts)
            {
                if (part.Has() == false) continue;
                result.Append(" • " + part);
            }

            return result.ToString().ToLower();
        }

        public void SetTitle(string? page)
        {
            _parts.Clear();
            _parts.Add(page);
        }

        public void SetTitle(string? category, string? page)
        {
            _parts.Clear();
            _parts.Add(category);
            _parts.Add(page);
        }

        public void SetTitle(params string?[] parts)
        {
            _parts.Clear();
            _parts.AddRange(parts);
        }

        public void SetTitle(ITitle? page)
        {
            _parts.Clear();
            _parts.AddRange(page?.Title);
        }

        public void SetTitle(IName? category, ITitle? page)
        {
            _parts.Clear();
            _parts.AddRange(category?.Name);
            _parts.AddRange(page?.Title);
        }

        public void SetTitle(ITag? category, ITitle? page)
        {
            _parts.Clear();
            _parts.AddRange(category?.Tag);
            _parts.AddRange(page?.Title);
        }

        public void SetTitle(ITag? tag, IName? category, ITitle? page)
        {
            _parts.Clear();
            _parts.AddRange(tag?.Tag);
            _parts.AddRange(category?.Name);
            _parts.AddRange(page?.Title);
        }
    }
}
