using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Origami.Core.Data
{
    public class OrigamiIdentityDbContext : IdentityDbContext
    {
        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="ctxOptions"></param>
        public OrigamiIdentityDbContext(DbContextOptions<OrigamiIdentityDbContext> ctxOptions) : base(ctxOptions)
        {

        }
    }
}
