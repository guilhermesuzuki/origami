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
        RepositoryLayer3SmartData<T>,
        ISearch<T>
        where T : class, IId
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

            if (entity is IContent content && content.Content.Has() == true) doc.Add(new TextField("content", content.Content, Field.Store.YES));
            if (entity is IDescription description && description.Description.Has() == true) doc.Add(new TextField("description", description.Description, Field.Store.YES));
            if (entity is IDescriptionNull descriptionNull && descriptionNull.Description.Has() == true) doc.Add(new TextField("description", descriptionNull.Description, Field.Store.YES));
            if (entity is IName name && name.Name.Has() == true) doc.Add(new TextField("name", name.Name, Field.Store.YES));
            if (entity is ISlug slug && slug.Slug.Has() == true) doc.Add(new TextField("slug", slug.Slug, Field.Store.YES));
            if (entity is ITag tag && tag.Tag.Has() == true) doc.Add(new TextField("tag", tag.Tag, Field.Store.YES));
            if (entity is ITitle title && title.Title.Has() == true) doc.Add(new TextField("title", title.Title, Field.Store.YES));
            if (entity is INanoId nano && nano.NanoId.Has() == true) doc.Add(new StringField("nano-id", nano.NanoId, Field.Store.YES));

            if (entity is OrigamiSocialProfile socialProfile)
            {
                doc.Add(new TextField("socialProfileSocialNetwork", socialProfile.SocialNetwork.ToString(), Field.Store.YES));
                if (socialProfile.FirstName.Has() == true) doc.Add(new TextField("socialProfileFirstName", socialProfile.FirstName, Field.Store.YES));
                if (socialProfile.LastName.Has() == true) doc.Add(new TextField("socialProfileLastName", socialProfile.LastName, Field.Store.YES));
            }

            if (entity is OrigamiUser user)
            {
                if (user.Username.Has() == true) doc.Add(new TextField("userName", user.Username, Field.Store.YES));
                if (user.DisplayName.Has() == true) doc.Add(new TextField("displayName", user.DisplayName, Field.Store.YES));
                if (user.FirstName.Has() == true) doc.Add(new TextField("firstName", user.FirstName, Field.Store.YES));
                if (user.LastName.Has() == true) doc.Add(new TextField("lastName", user.LastName, Field.Store.YES));
            }

            if (entity is OrigamiContentComment pcomment)
            {
                var post = this.ReadFromCache<OrigamiContent>().Id(pcomment.ContentId);
                var pcSocialProfile = this.ReadFromCache<OrigamiSocialProfile>().Id(pcomment.SocialProfileId);
                if (pcSocialProfile?.FirstName.Has() == true) doc.Add(new TextField("comment_socialProfileFirstName", pcSocialProfile.FirstName, Field.Store.YES));
                if (pcSocialProfile?.LastName.Has() == true) doc.Add(new TextField("comment_socialProfileLastName", pcSocialProfile.LastName, Field.Store.YES));
                if (pcSocialProfile?.Name.Has() == true) doc.Add(new TextField("comment_socialProfileName", pcSocialProfile.Name, Field.Store.YES));
                if (pcSocialProfile != null) doc.Add(new TextField("comment_socialProfileSocialNetwork", pcSocialProfile.SocialNetwork.ToString(), Field.Store.YES));
                if (post?.Title.Has() == true) doc.Add(new TextField("comment_postTitle", post.Title, Field.Store.YES));
            }

            return doc;
        }

        protected virtual IEnumerable<WildcardQuery> GetWildcardQueries(string searchTerm)
        {
            searchTerm = $"{QueryParser.Escape(searchTerm)}";

            var type = typeof(T);
            var queries = new List<WildcardQuery>();

            if (type.Implements<IId>() == true) queries.Add(new(new("id", searchTerm)));
            if (type.Implements<IContent>() == true) queries.Add(new(new("content", searchTerm)));
            if (type.Implements<IDescription>() == true) queries.Add(new(new("description", searchTerm)));
            if (type.Implements<IDescriptionNull>() == true) queries.Add(new(new("description", searchTerm)));
            if (type.Implements<IName>() == true) queries.Add(new(new("name", searchTerm)));
            if (type.Implements<ISlug>() == true) queries.Add(new(new("slug", searchTerm)));
            if (type.Implements<ITag>() == true) queries.Add(new(new("tag", searchTerm)));
            if (type.Implements<ITitle>() == true) queries.Add(new(new("title", searchTerm)));
            if (type.Implements<INanoId>() == true) queries.Add(new(new("nano-id", searchTerm)));

            if (type.IsAssignableFrom(typeof(OrigamiSocialProfile)) == true)
            {
                queries.Add(new(new("socialProfileSocialNetwork", searchTerm)));
                queries.Add(new(new("socialProfileFirstName", searchTerm)));
                queries.Add(new(new("socialProfileLastName", searchTerm)));
            }

            if (type.IsAssignableFrom(typeof(OrigamiUser)) == true)
            {
                queries.Add(new(new("userName", searchTerm)));
                queries.Add(new(new("displayName", searchTerm)));
                queries.Add(new(new("firstName", searchTerm)));
                queries.Add(new(new("lastName", searchTerm)));
            }

            if (type.IsAssignableFrom(typeof(OrigamiContentComment)) == true)
            {
                queries.Add(new(new("comment_socialProfileFirstName", searchTerm)));
                queries.Add(new(new("comment_socialProfileLastName", searchTerm)));
                queries.Add(new(new("comment_socialProfileName", searchTerm)));
                queries.Add(new(new("comment_socialProfileSocialNetwork", searchTerm)));
            }

            return queries;
        }
    }
}
