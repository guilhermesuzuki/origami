using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class WhatToSeeNextRepository : IWhatToSeeNextRepository
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPageRepository _pageRepository;
        private readonly IPostRepository _postRepository;
        private readonly IVideoRepository _videoRepository;
        private readonly IPostCategoryRepository _postCategoryRepository;
        private readonly IVideoCategoryRepository _videoCategoryRepository;
        private readonly IPostTagRepository _postTagRepository;
        private readonly IVideoTagRepository _videoTagRepository;

        public WhatToSeeNextRepository(
            ICategoryRepository categoryRepository,
            IPageRepository pageRepository,
            IPostRepository postRepository,
            IVideoRepository videoRepository,
            IPostCategoryRepository postCategoryRepository,
            IVideoCategoryRepository videoCategoryRepository,
            IPostTagRepository postTagRepository,
            IVideoTagRepository videoTagRepository)
            : base()
        {
            _categoryRepository = categoryRepository;
            _pageRepository = pageRepository;
            _postRepository = postRepository;
            _videoRepository = videoRepository;
            _postCategoryRepository = postCategoryRepository;
            _videoCategoryRepository = videoCategoryRepository;
            _postTagRepository = postTagRepository;
            _videoTagRepository = videoTagRepository;
        }

        public IEnumerable<BaseContent> GetWhatToSeeNext<T>(T entity) where T : ITitle, IContent, IId, new()
        {
            if (entity is OrigamiPost post)
            {
                var content = new List<BaseContent>();

                var categories = _postCategoryRepository.ReadFromCache().Where(x => x.PostId == post.Id).Select(x => x.CategoryId);
                var tags = _postTagRepository.ReadFromCache().Where(x => x.PostId == post.Id).Select(x => x.Tag);

                var ps = from pc in _postCategoryRepository.ReadFromCache()
                         join pt in _postRepository.ReadFromCache().NonDeleted().Published() on pc.PostId equals pt.Id
                         where pc.PostId == post.Id
                         select pt;

                var vs = from vc in _videoCategoryRepository.ReadFromCache()
                         join vt in _videoRepository.ReadFromCache().NonDeleted().Published() on vc.VideoId equals vt.Id
                         join ct in categories on vc.CategoryId equals ct
                         select vt;

                var t1 = from pt in _postTagRepository.ReadFromCache()
                         join po in _postRepository.ReadFromCache().NonDeleted().Published() on pt.PostId equals po.Id
                         where tags.Contains(pt.Tag)
                         select po;

                var t2 = from vt in _videoTagRepository.ReadFromCache()
                         join vd in _videoRepository.ReadFromCache().NonDeleted().Published() on vt.VideoId equals vd.Id
                         where tags.Contains(vt.Tag)
                         select vd;

                content.AddRange(ps.Cast<BaseContent>());
                content.AddRange(vs.Cast<BaseContent>());
                content.AddRange(t1.Cast<BaseContent>());
                content.AddRange(t2.Cast<BaseContent>());
                content.RemoveAll(x => x.Id == post.Id);

                return content.GroupBy(x => x).OrderByDescending(x => x.Count()).Select(x => x.Key);
            }

            if (entity is OrigamiVideo video)
            {
                var content = new List<BaseContent>();

                var categories = _videoCategoryRepository.ReadFromCache().Where(x => x.VideoId == video.Id).Select(x => x.CategoryId);
                var tags = _videoTagRepository.ReadFromCache().Where(x => x.VideoId == video.Id).Select(x => x.Tag);

                var ps = from pc in _videoCategoryRepository.ReadFromCache()
                         join pt in _videoRepository.ReadFromCache().NonDeleted().Published() on pc.VideoId equals pt.Id
                         where pc.VideoId == video.Id
                         select pt;

                var vs = from vc in _postCategoryRepository.ReadFromCache()
                         join vt in _postRepository.ReadFromCache().NonDeleted().Published() on vc.PostId equals vt.Id
                         join ct in categories on vc.CategoryId equals ct
                         select vt;

                var t1 = from pt in _videoTagRepository.ReadFromCache()
                         join po in _videoRepository.ReadFromCache().NonDeleted().Published() on pt.VideoId equals po.Id
                         where tags.Contains(pt.Tag)
                         select po;

                var t2 = from vt in _postTagRepository.ReadFromCache()
                         join vd in _postRepository.ReadFromCache().NonDeleted().Published() on vt.PostId equals vd.Id
                         where tags.Contains(vt.Tag)
                         select vd;

                content.AddRange(ps.Cast<BaseContent>());
                content.AddRange(vs.Cast<BaseContent>());
                content.AddRange(t1.Cast<BaseContent>());
                content.AddRange(t2.Cast<BaseContent>());
                content.RemoveAll(x => x.Id == video.Id);

                return content.GroupBy(x => x).OrderByDescending(x => x.Count()).Select(x => x.Key);
            }

            throw new NotImplementedException();
        }

        public IEnumerable<BaseContent> GetWhatToSeeNextTitle<T>(T entity) where T : ITitle, IContent, IId, new()
        {
            var content = new List<FuzzyContent>();

            var ps1 = from p in _postRepository.ReadFromCache().NonDeleted().Published()
                      select new FuzzyContent { Content = p, Fuzzy = FuzzySharp.Fuzz.WeightedRatio(entity.Title, p.Title) };

            var ps2 = from p in _postRepository.ReadFromCache().NonDeleted().Published()
                      select new FuzzyContent { Content = p, Fuzzy = FuzzySharp.Fuzz.Ratio(entity.Title, p.Title) };

            var vs1 = from v in _videoRepository.ReadFromCache().NonDeleted().Published()
                      select new FuzzyContent { Content = v, Fuzzy = FuzzySharp.Fuzz.WeightedRatio(entity.Title, v.Title) };

            var vs2 = from v in _videoRepository.ReadFromCache().NonDeleted().Published()
                      select new FuzzyContent { Content = v, Fuzzy = FuzzySharp.Fuzz.Ratio(entity.Title, v.Title) };

            content.AddRange(ps1);
            content.AddRange(ps2);
            content.AddRange(vs1);
            content.AddRange(vs2);

            return content.Where(x => x.Fuzzy >= 70 && x.Fuzzy < 100).OrderByDescending(x => x.Fuzzy).DistinctBy(x => x.Content).Select(x => x.Content);
        }

        private class FuzzyContent
        {
            public BaseContent Content { get; set; } = new OrigamiPost();
            public int Fuzzy { get; set; } = 0;
        }
    }
}
