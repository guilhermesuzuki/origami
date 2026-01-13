using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public abstract class RepositoryLayer4Search<T> :
        RepositoryLayer3Data<T>,
        ISearch<T>
        where T : class, IId, new()
    {
        protected RepositoryLayer4Search(
            Text text,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IWebRootPath webRootPath)
            : base(text, dbContextFactory, memoryCache, webRootPath)
        {

        }

        public virtual bool CreateSearchIndex()
        {
            // Specify the compatibility version we want
            const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;

            //Open the Directory using a Lucene Directory class
            var key = $"lucene_{typeof(T).GetPlural().ToLower()}";
            using RAMDirectory? oldIndex = MemoryCache.Get<RAMDirectory>(key);
            var index = new RAMDirectory();

            //Create an analyzer to process the text 
            using Analyzer standardAnalyzer = new StandardAnalyzer(luceneVersion);

            //Create an index writer
            IndexWriterConfig indexConfig = new(luceneVersion, standardAnalyzer);
            indexConfig.OpenMode = OpenMode.CREATE;
            using IndexWriter writer = new(index, indexConfig);

            foreach (var entity in ReadFromCache())
            {
                writer.AddDocument(this.GetLuceneDocument(entity));
            }

            //Flush and commit the index data to the directory
            writer.Commit();

            MemoryCache.Set(key, index);

            return true;
        }

        public virtual IEnumerable<T> Search(string searchTerm)
        {
            //Open the Directory using a Lucene Directory class
            var index = typeof(T).GetPlural().ToLower();
            var directory = MemoryCache.Get<RAMDirectory>($"lucene_{index}");
            if (directory == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Lucene index for {index} not found. Please create it first.");
                return [];
            }

            using var reader = DirectoryReader.Open(directory);
            using var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            var searcher = new IndexSearcher(reader);
            var result = new List<T>();

            foreach (var query in GetWildcardQueries(searchTerm))
            {
                var topDocs = searcher.Search(query, int.MaxValue);
                foreach (var scoreDoc in topDocs.ScoreDocs)
                {
                    var doc = searcher.Doc(scoreDoc.Doc);
                    if (Guid.TryParse(doc.Get("id"), out var id) == true)
                    {
                        var entity = ReadFromCache().Id(id);
                        if (entity != null) result.Add(entity);
                    }
                }
            }

            return result.DistinctBy(x => x.Id);
        }

        protected virtual Document GetLuceneDocument(T entity)
        {
            var doc = new Document
            {
                new StringField("id", entity.Id.ToString(), Field.Store.YES)
            };

            if (entity is IContent content) doc.Add(new TextField("content", content.Content, Field.Store.YES));
            if (entity is IDescription description) doc.Add(new TextField("description", description.Description, Field.Store.YES));
            if (entity is IDescriptionNull descriptionNull) doc.Add(new TextField("description", descriptionNull.Description, Field.Store.YES));
            if (entity is IName name) doc.Add(new TextField("name", name.Name, Field.Store.YES));
            if (entity is ISlug slug) doc.Add(new TextField("slug", slug.Slug, Field.Store.YES));
            if (entity is ITag tag) doc.Add(new TextField("tag", tag.Tag, Field.Store.YES));
            if (entity is ITitle title) doc.Add(new TextField("title", title.Title, Field.Store.YES));
            if (entity is INanoId nano) doc.Add(new StringField("nano-id", nano.NanoId, Field.Store.YES));

            if (entity is OrigamiSocialProfile socialProfile)
            {
                doc.Add(new TextField("socialProfileSocialNetwork", socialProfile.SocialNetwork.ToString(), Field.Store.YES));
                doc.Add(new TextField("socialProfileFirstName", socialProfile.FirstName, Field.Store.YES));
                doc.Add(new TextField("socialProfileLastName", socialProfile.LastName, Field.Store.YES));
            }

            if (entity is OrigamiUser user)
            {
                doc.Add(new TextField("userName", user.Username, Field.Store.YES));
                doc.Add(new TextField("displayName", user.DisplayName, Field.Store.YES));
                doc.Add(new TextField("firstName", user.FirstName, Field.Store.YES));
                doc.Add(new TextField("lastName", user.LastName, Field.Store.YES));
            }

            if (entity is OrigamiPostComment pcomment)
            {
                var post = this.ReadFromCache<OrigamiPost>().Id(pcomment.PostId);
                var pcSocialProfile = this.ReadFromCache<OrigamiSocialProfile>().Id(pcomment.SocialProfileId);
                doc.Add(new TextField("comment_socialProfileFirstName", pcSocialProfile?.FirstName, Field.Store.YES));
                doc.Add(new TextField("comment_socialProfileLastName", pcSocialProfile?.LastName, Field.Store.YES));
                doc.Add(new TextField("comment_socialProfileName", pcSocialProfile?.Name, Field.Store.YES));
                doc.Add(new TextField("comment_socialProfileSocialNetwork", pcSocialProfile?.SocialNetwork.ToString(), Field.Store.YES));
                doc.Add(new TextField("comment_postTitle", post?.Title, Field.Store.YES));
            }

            if (entity is OrigamiVideoComment vcomment)
            {
                var video = this.ReadFromCache<OrigamiVideo>().Id(vcomment.VideoId);
                var vcSocialProfile = this.ReadFromCache<OrigamiSocialProfile>().Id(vcomment.SocialProfileId);
                doc.Add(new TextField("comment_socialProfileFirstName", vcSocialProfile?.FirstName, Field.Store.YES));
                doc.Add(new TextField("comment_socialProfileLastName", vcSocialProfile?.LastName, Field.Store.YES));
                doc.Add(new TextField("comment_socialProfileName", vcSocialProfile?.Name, Field.Store.YES));
                doc.Add(new TextField("comment_socialProfileSocialNetwork", vcSocialProfile?.SocialNetwork.ToString(), Field.Store.YES));
                doc.Add(new TextField("comment_videoTitle", video?.Title, Field.Store.YES));
            }

            return doc;
        }

        protected virtual IEnumerable<WildcardQuery> GetWildcardQueries(string searchTerm)
        {
            searchTerm = $"{QueryParser.Escape(searchTerm)}*";

            var t = new T();
            var queries = new List<WildcardQuery>();

            if (t is IId id) queries.Add(new(new("id", searchTerm)));
            if (t is IContent content) queries.Add(new(new("content", searchTerm)));
            if (t is IDescription description) queries.Add(new(new("description", searchTerm)));
            if (t is IDescriptionNull descriptionNull) queries.Add(new(new("description", searchTerm)));
            if (t is IName name) queries.Add(new(new("name", searchTerm)));
            if (t is ISlug slug) queries.Add(new(new("slug", searchTerm)));
            if (t is ITag tag) queries.Add(new(new("tag", searchTerm)));
            if (t is ITitle title) queries.Add(new(new("title", searchTerm)));
            if (t is INanoId nano) queries.Add(new(new("nano-id", searchTerm)));

            if (t is OrigamiSocialProfile socialProfile)
            {
                queries.Add(new(new("socialProfileSocialNetwork", searchTerm)));
                queries.Add(new(new("socialProfileFirstName", searchTerm)));
                queries.Add(new(new("socialProfileLastName", searchTerm)));
            }

            if (t is OrigamiUser user)
            {
                queries.Add(new(new("userName", searchTerm)));
                queries.Add(new(new("displayName", searchTerm)));
                queries.Add(new(new("firstName", searchTerm)));
                queries.Add(new(new("lastName", searchTerm)));
            }

            if (t is OrigamiPostComment pcomment)
            {
                queries.Add(new(new("comment_socialProfileFirstName", searchTerm)));
                queries.Add(new(new("comment_socialProfileLastName", searchTerm)));
                queries.Add(new(new("comment_socialProfileName", searchTerm)));
                queries.Add(new(new("comment_socialProfileSocialNetwork", searchTerm)));
                queries.Add(new(new("comment_postTitle", searchTerm)));
            }

            if (t is OrigamiVideoComment vcomment)
            {
                queries.Add(new(new("comment_socialProfileFirstName", searchTerm)));
                queries.Add(new(new("comment_socialProfileLastName", searchTerm)));
                queries.Add(new(new("comment_socialProfileName", searchTerm)));
                queries.Add(new(new("comment_socialProfileSocialNetwork", searchTerm)));
                queries.Add(new(new("comment_videoTitle", searchTerm)));
            }

            return queries;
        }
    }
}
