using Origami.Core.Models;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Origami.Core.Data
{
    public class RssRepository : IRssRepository
    {
        protected readonly IBlogRepository _blogRepository;
        protected readonly IFileRepository _fileRepository;
        protected readonly IPostRepository _postRepository;
        protected readonly IVideoRepository _videoRepository;
        protected readonly Text _text;

        public RssRepository(
            IBlogRepository blogRepository,
            IPostRepository postRepository,
            IVideoRepository videoRepository,
            IFileRepository fileRepository,
            Text text) : base()
        {
            _blogRepository = blogRepository;
            _fileRepository = fileRepository;
            _postRepository = postRepository;
            _videoRepository = videoRepository;
            _text = text;
        }

        public string GetRss(string slug, string oi)
        {
            oi = oi.TrimEnd('/') + '/';
            var blog = this._blogRepository.ReadFromCache().NonDeleted().Active().Slug(slug);
            if (blog != null)
            {
                var posts = this._postRepository.ReadFromCache().Blog(blog.Id).Published().OrderByDescending(x => x.DatePublished).Take(10);
                var videos = this._videoRepository.ReadFromCache().Blog(blog.Id).Published().OrderByDescending(x => x.DatePublished).Take(10);

                IList<BaseContent> allItems = [.. posts, .. videos];

                var rss = new XDocument(
                    new XElement("rss",
                        new XAttribute("version", "2.0"),
                        new XElement("channel",
                            new XElement("title", blog.Name),
                            new XElement("link", oi + blog.Hyperlink.TrimStart('/')),
                            new XElement("description", _text.Original("RSS feed")),
                            new XElement("lastBuildDate", DateTime.UtcNow.ToString("R")),
                            allItems.Select(item =>
                                new XElement("item",
                                    new XElement("title", item.Title),
                                    new XElement("link", oi + item.Hyperlink.TrimStart('/')),
                                    new XElement("description", item.Description),
                                    new XElement("pubDate", item.DatePublished.GetValueOrDefault().ToString("R")),
                                    new XElement("guid", item.Id),
                                    new XElement("category", this.GetCategory(item)),
                                    this.GetEnclosure(item, oi)
                                )
                            )
                        )
                    )
                );

                var sb = new StringBuilder();
                using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8, OmitXmlDeclaration = true }))
                {
                    rss.Save(writer);
                    writer.Flush();
                }
                return sb.ToString();
            }

            return string.Empty;
        }

        protected string GetCategory(BaseContent item)
        {
            return item switch
            {
                OrigamiPage => "Page",
                OrigamiPost => "Post",
                OrigamiVideo => "Video",
                _ => "Unknown"
            };
        }

        protected XElement? GetEnclosure(BaseContent item, string oi)
        {
            if (item is IHeaderImage header)
            {
                var file = this._fileRepository.GetFile(header.HeaderImage);
                if (file != null)
                {
                    return new XElement("enclosure",
                                        new XAttribute("url", oi + header.HeaderImage.TrimStart('/')),
                                        new XAttribute("type", file.ContentType),
                                        new XAttribute("length", file.FileSize));
                }
            }
            return null;
        }
    }
}
