using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;
using Origami.Core.Models.Resume;
using System.Xml.Linq;

namespace Origami.Core.Data
{
    public class ResumeRepository :
        RepositoryOuterLayer<Resume>,
        IResumeRepository
    {
        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public ResumeRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {

        }

        public override List<Resume> ReadFromCache()
        {
            List<Resume> resumes = [];

            var dir = Path.Combine(this.WebRootPath.WebRootPath, "files", "resumes");
            if (Directory.Exists(dir) == true)
            {
                var xmlFiles = Directory.GetFiles(dir);
                foreach (var xmlFile in xmlFiles)
                {
                    var xml = XDocument.Load(xmlFile);
                    var resume = xml.ToString().Deserialize<Resume>();
                    if (resume != null) resumes.Add(resume);
                }
            }

            return resumes;
        }
    }
}
