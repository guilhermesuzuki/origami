using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class WhatToSeeNextRepository : IWhatToSeeNextRepository
    {
        protected readonly IContentRepository _contentRepository;
        protected readonly IContentCategoryRepository _contentCategoryRepository;
        protected readonly IContentTagRepository _contentTagRepository;

        public WhatToSeeNextRepository(
            IContentRepository contentRepository,
            IContentCategoryRepository postCategoryRepository,
            IContentTagRepository postTagRepository)
            : base()
        {
            _contentRepository = contentRepository;
            _contentCategoryRepository = postCategoryRepository;
            _contentTagRepository = postTagRepository;
        }

        public IEnumerable<OrigamiContent> GetWhatToSeeNext<T>(T entity) where T : ITitle, IContent, IId
        {
            var content = new List<OrigamiContent>();

            var categories = _contentCategoryRepository.ReadFromCache().Where(x => x.ContentId == entity.Id).Select(x => x.CategoryId);
            var tags = _contentTagRepository.ReadFromCache().Where(x => x.ContentId == entity.Id).Select(x => x.Tag);

            var ps = from pc in _contentCategoryRepository.ReadFromCache()
                     join pt in _contentRepository.ReadFromCache().NonDeleted().Published() on pc.ContentId equals pt.Id
                     where pc.ContentId == entity.Id
                     select pt;

            var t1 = from pt in _contentTagRepository.ReadFromCache()
                     join po in _contentRepository.ReadFromCache().NonDeleted().Published() on pt.ContentId equals po.Id
                     where tags.Contains(pt.Tag)
                     select po;

            content.AddRange(ps);
            content.AddRange(t1);
            content.RemoveAll(x => x.Id == entity.Id);

            return content.GroupBy(x => x).OrderByDescending(x => x.Count()).Select(x => x.Key);
        }

        public IEnumerable<OrigamiContent> GetWhatToSeeNextTitle<T>(T entity) where T : ITitle, IContent, IId
        {
            var content = new List<FuzzyContent>();

            var ps1 = from p in _contentRepository.ReadFromCache().NonDeleted().Published()
                      select new FuzzyContent { Content = p, Fuzzy = FuzzySharp.Fuzz.WeightedRatio(entity.Title, p.Title) };

            var ps2 = from p in _contentRepository.ReadFromCache().NonDeleted().Published()
                      select new FuzzyContent { Content = p, Fuzzy = FuzzySharp.Fuzz.Ratio(entity.Title, p.Title) };

            var vs1 = from v in _contentRepository.ReadFromCache().NonDeleted().Published()
                      select new FuzzyContent { Content = v, Fuzzy = FuzzySharp.Fuzz.WeightedRatio(entity.Title, v.Title) };

            var vs2 = from v in _contentRepository.ReadFromCache().NonDeleted().Published()
                      select new FuzzyContent { Content = v, Fuzzy = FuzzySharp.Fuzz.Ratio(entity.Title, v.Title) };

            content.AddRange(ps1);
            content.AddRange(ps2);
            content.AddRange(vs1);
            content.AddRange(vs2);

            return content.Where(x => x.Fuzzy >= 70 && x.Fuzzy < 100).OrderByDescending(x => x.Fuzzy).DistinctBy(x => x.Content).Select(x => x.Content);
        }

        private class FuzzyContent
        {
            public OrigamiContent Content { get; set; } = null!;
            public int Fuzzy { get; set; } = 0;
        }
    }
}
