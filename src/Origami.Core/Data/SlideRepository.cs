using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models.FileSystem;

namespace Origami.Core.Data
{
    public class SlideRepository : ISlideRepository
    {
        private const string SlidesDirectory = "/files/slides";
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly Random _random = new();

        public SlideRepository(IDirectoryRepository directoryRepository, IMemoryCache memoryCache)
        {
            _directoryRepository = directoryRepository;
            _memoryCache = memoryCache;
        }

        public OrigamiSystemFile? GetSlide()
        {
            return _memoryCache.GetOrCreate("slide", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                var files = GetSlides();
                if (files.Count() > 0)
                {
                    return files.ElementAt(_random.Next(0, files.Count()));
                }
                return null;
            });
        }

        public IEnumerable<OrigamiSystemFile> GetSlides()
        {
            if (_directoryRepository.DirectoryExists(SlidesDirectory) == true)
            {
                var files = _directoryRepository.GetFiles(SlidesDirectory);
                return files.Where(x => x.IsImage);
            }
            return Enumerable.Empty<OrigamiSystemFile>();
        }
    }
}
