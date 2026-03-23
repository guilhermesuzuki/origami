using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    /// <summary>
    /// Database Context for the Origami application
    /// </summary>
    public class OrigamiDbContext : DbContext
    {
        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="ctxOptions"></param>
        public OrigamiDbContext(DbContextOptions<OrigamiDbContext> ctxOptions) : base(ctxOptions)
        {

        }

        /// <summary>
        /// Backups and Restores
        /// </summary>
        public DbSet<OrigamiBackup> Backups { get; set; }

        /// <summary>
        /// Blog Rolls
        /// </summary>
        public DbSet<OrigamiBlogRoll> BlogRolls { get; set; }

        /// <summary>
        /// Blogs
        /// </summary>
        public DbSet<OrigamiBlog> Blogs { get; set; }

        /// <summary>
        /// Categories
        /// </summary>
        public DbSet<OrigamiCategory> Categories { get; set; }

        /// <summary>
        /// Post Categories
        /// </summary>
        public DbSet<OrigamiContentCategory> ContentCategories { get; set; }

        /// <summary>
        /// Post Comment Ratings
        /// </summary>
        public DbSet<OrigamiContentCommentReaction> ContentCommentReactions { get; set; }

        /// <summary>
        /// Post Comments
        /// </summary>
        public DbSet<OrigamiContentComment> ContentComments { get; set; }

        /// <summary>
        /// Post Ratings
        /// </summary>
        public DbSet<OrigamiContentRating> ContentRatings { get; set; }

        /// <summary>
        /// Contents
        /// </summary>
        public DbSet<OrigamiContent> Contents { get; set; }

        /// <summary>
        /// Post Tags
        /// </summary>
        public DbSet<OrigamiContentTag> ContentTags { get; set; }

        /// <summary>
        /// Custom Fields
        /// </summary>
        public DbSet<OrigamiCustomField> CustomFields { get; set; }

        /// <summary>
        /// Data Store Settings
        /// </summary>
        public DbSet<OrigamiDataStoreSetting> DataStoreSettings { get; set; }

        /// <summary>
        /// File Store Directories
        /// </summary>
        public DbSet<OrigamiFileStoreDirectory> FileStoreDirectories { get; set; }

        /// <summary>
        /// File Store Files
        /// </summary>
        public DbSet<OrigamiFileStoreFile> FileStoreFiles { get; set; }

        /// <summary>
        /// File Store File Thumbs
        /// </summary>
        public DbSet<OrigamiFileStoreFileThumb> FileStoreFileThumbs { get; set; }
        /// <summary>
        /// Package Files
        /// </summary>
        public DbSet<OrigamiPackageFile> PackageFiles { get; set; }

        /// <summary>
        /// Packages
        /// </summary>
        public DbSet<OrigamiPackage> Packages { get; set; }

        /// <summary>
        /// Pages
        /// </summary>
        public DbSet<OrigamiPage> Pages { get; set; }

        /// <summary>
        /// Ping Services
        /// </summary>
        public DbSet<OrigamiPingService> PingServices { get; set; }

        /// <summary>
        /// Physical Page Reactions
        /// </summary>
        public DbSet<OrigamiPhysicalPageReaction> PhysicalPageReactions { get; set; }

        /// <summary>
        /// Physical Pages
        /// </summary>
        public DbSet<OrigamiPhysicalPage> PhysicalPages { get; set; }

        /// <summary>
        /// Physical Page Views
        /// </summary>
        public DbSet<OrigamiPhysicalPageView> PhysicalPageViews { get; set; }
        /// <summary>
        /// Posts
        /// </summary>
        public DbSet<OrigamiPost> Posts { get; set; }
        /// <summary>
        /// Processed User Views for Histories
        /// </summary>
        public DbSet<ProcessedUserViewForHistory> ProcessedUserViewForHistories { get; set; }

        /// <summary>
        /// Processed User Views
        /// </summary>
        public DbSet<ProcessedUserView> ProcessedUserViews { get; set; }

        /// <summary>
        /// Quick Notes
        /// </summary>
        public DbSet<OrigamiQuickNote> QuickNotes { get; set; }

        /// <summary>
        /// Quick Settings
        /// </summary>
        public DbSet<OrigamiQuickSetting> QuickSettings { get; set; }

        /// <summary>
        /// Referrers
        /// </summary>
        public DbSet<OrigamiReferrer> Referrers { get; set; }

        /// <summary>
        /// Right Roles
        /// </summary>
        public DbSet<OrigamiRightRole> RightRoles { get; set; }

        /// <summary>
        /// Rights
        /// </summary>
        public DbSet<OrigamiRight> Rights { get; set; }

        /// <summary>
        /// Roles
        /// </summary>
        public DbSet<OrigamiRole> Roles { get; set; }

        /// <summary>
        /// Settings
        /// </summary>
        public DbSet<OrigamiSetting> Settings { get; set; }

        /// <summary>
        /// Social Profiles
        /// </summary>
        public DbSet<OrigamiSocialProfile> SocialProfiles { get; set; }

        /// <summary>
        /// Social Profiles for Deletion
        /// </summary>
        public DbSet<OrigamiSocialProfileDelete> SocialProfilesForDeletion { get; set; }

        /// <summary>
        /// Special messages
        /// </summary>
        public DbSet<OrigamiSpecialMessage> SpecialMessages { get; set; }

        /// <summary>
        /// Special Pages
        /// </summary>
        public DbSet<OrigamiSpecialPage> SpecialPages { get; set; }

        /// <summary>
        /// Stop Words
        /// </summary>
        public DbSet<OrigamiStopWord> StopWords { get; set; }

        /// <summary>
        /// Subscribers
        /// </summary>
        public DbSet<OrigamiSubscriber> Subscribers { get; set; }

        /// <summary>
        /// Tags
        /// </summary>
        public DbSet<OrigamiTag> Tags { get; set; }

        /// <summary>
        /// Trashes
        /// </summary>
        public DbSet<OrigamiTrash> Trashes { get; set; }

        /// <summary>
        /// User Activities
        /// </summary>
        public DbSet<OrigamiUserActivity> UserActivities { get; set; }

        /// <summary>
        /// User blogs
        /// </summary>
        public DbSet<OrigamiUserBlog> UserBlogs { get; set; }

        /// <summary>
        /// User password resets
        /// </summary>
        public DbSet<OrigamiUserPasswordReset> UserPasswordResets { get; set; }

        /// <summary>
        /// User Roles
        /// </summary>
        public DbSet<OrigamiUserRole> UserRoles { get; set; }

        /// <summary>
        /// Users
        /// </summary>
        public DbSet<OrigamiUser> Users { get; set; }
        /// <summary>
        /// User Trashes
        /// </summary>
        public DbSet<OrigamiUserTrash> UserTrashes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Owned<Location>();
            modelBuilder.Owned<OrigamiFile>();
            modelBuilder.Owned<Content>();

            modelBuilder.Entity<OrigamiContent>()
                .HasDiscriminator(x => x.Type)
                .HasValue<OrigamiPage>(nameof(OrigamiPage))
                .HasValue<OrigamiPost>(nameof(OrigamiPost))
                .HasValue<OrigamiSpecialPage>(nameof(OrigamiSpecialPage))
                .HasValue<OrigamiVideo>(nameof(OrigamiVideo))
                ;

            modelBuilder.Entity<OrigamiContent>().HasOne<OrigamiContent>().WithMany().HasForeignKey(x => x.ParentId);
            modelBuilder.Entity<OrigamiContent>().HasOne<OrigamiBlog>().WithMany().HasForeignKey(x => x.BlogId);
            modelBuilder.Entity<OrigamiContent>().HasOne<OrigamiUser>().WithMany().HasForeignKey(x => x.AuthorId);

            modelBuilder.Entity<OrigamiContentCategory>().HasOne<OrigamiContent>().WithMany().HasForeignKey(x => x.ContentId);
            modelBuilder.Entity<OrigamiContentCategory>().HasOne<OrigamiContentCategory>().WithMany().HasForeignKey(x => x.CategoryId);

            modelBuilder.Entity<OrigamiContentComment>().HasOne<OrigamiContent>().WithMany().HasForeignKey(x => x.ContentId);
            modelBuilder.Entity<OrigamiContentComment>().HasOne<OrigamiContentComment>().WithMany().HasForeignKey(x => x.ParentId);
            modelBuilder.Entity<OrigamiContentComment>().HasOne<OrigamiSocialProfile>().WithMany().HasForeignKey(x => x.ModeratedById);
            modelBuilder.Entity<OrigamiContentComment>().HasOne<OrigamiSocialProfile>().WithMany().HasForeignKey(x => x.PinnedById);
            modelBuilder.Entity<OrigamiContentComment>().HasOne<OrigamiSocialProfile>().WithMany().HasForeignKey(x => x.SocialProfileId);

            modelBuilder.Entity<OrigamiCategory>()
                .HasOne<OrigamiCategory>()
                .WithMany()
                .HasForeignKey(x => x.ParentId);

            modelBuilder.Entity<OrigamiCategory>()
                .HasOne<OrigamiBlog>()
                .WithMany()
                .HasForeignKey(x => x.BlogId);

            modelBuilder.Entity<OrigamiQuickSetting>()
                .HasOne<OrigamiBlog>()
                .WithMany()
                .HasForeignKey(x => x.BlogId);

            modelBuilder.Entity<OrigamiPhysicalPageView>()
                .HasOne<OrigamiPhysicalPage>()
                .WithMany()
                .HasForeignKey(x => x.PhysicalPageId);

            modelBuilder.Entity<OrigamiPhysicalPageReaction>()
                .HasOne<OrigamiPhysicalPage>()
                .WithMany()
                .HasForeignKey(x => x.PhysicalPageId);

            //social-network as string
            modelBuilder
                .Entity<OrigamiSocialProfile>()
                .Property(e => e.SocialNetwork)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<SocialNetworks>(v));

            //role FK for right-role
            modelBuilder.Entity<OrigamiRightRole>()
                .HasOne<OrigamiRole>()
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            //right FK for right-role
            modelBuilder.Entity<OrigamiRightRole>()
                .HasOne<OrigamiRight>()
                .WithMany()
                .HasForeignKey(x => x.RightId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrigamiPage>()
                .HasOne<OrigamiUser>()
                .WithMany()
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrigamiUserPasswordReset>()
                .HasOne<OrigamiUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrigamiCustomField>()
                .HasOne<OrigamiBlog>()
                .WithMany()
                .HasForeignKey(x => x.BlogId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrigamiUserPasswordReset>()
                .HasOne<OrigamiUser>()
                .WithMany()
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrigamiUserRole>()
                .HasOne<OrigamiUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrigamiUserRole>()
                .HasOne<OrigamiRole>()
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrigamiContentCommentReaction>().Property(c => c.Reaction).UseCollation("Latin1_General_BIN2");
            modelBuilder.Entity<OrigamiPhysicalPageReaction>().Property(c => c.Reaction).UseCollation("Latin1_General_BIN2");

            // Map the entity to the view
            modelBuilder.Entity<OrigamiUserActivity>().ToView("oi_vw_UserActivities");
            modelBuilder.Entity<OrigamiUserActivity>().Metadata.SetIsTableExcludedFromMigrations(true);

            // Map the entity to the view
            modelBuilder.Entity<OrigamiUserView>().ToView("oi_vw_UserViews");
            modelBuilder.Entity<OrigamiUserView>().Metadata.SetIsTableExcludedFromMigrations(true);

            // Map the entity to the view
            modelBuilder.Entity<OrigamiUserContent>().ToView("oi_vw_UserContents");
            modelBuilder.Entity<OrigamiUserContent>().Metadata.SetIsTableExcludedFromMigrations(true);

            // Map the entity to the view
            modelBuilder.Entity<OrigamiUserTrash>().ToView("oi_vw_UserTrashes");
            modelBuilder.Entity<OrigamiUserTrash>().Metadata.SetIsTableExcludedFromMigrations(true);

            // Map the entity to the view
            modelBuilder.Entity<OrigamiTag>().ToView("oi_vw_Tags");
            modelBuilder.Entity<OrigamiTag>().Metadata.SetIsTableExcludedFromMigrations(true);

            // Map the entity to the view
            modelBuilder.Entity<OrigamiTrash>().ToView("oi_vw_Trashes");
            modelBuilder.Entity<OrigamiTrash>().Metadata.SetIsTableExcludedFromMigrations(true);

            modelBuilder.Entity<ProcessedUserView>().HasNoKey();
            modelBuilder.Entity<ProcessedUserViewForHistory>().HasNoKey();

            /*backup and restore mapping*/
            modelBuilder.Entity<OrigamiBackup>()
                .ToTable("oi_BackupRestores")
                .HasDiscriminator<bool>("Backup")
                .HasValue<OrigamiBackup>(true)
                .HasValue<OrigamiBackupRestore>(false);

            modelBuilder.Entity<OrigamiUserBlog>()
                .HasOne<OrigamiBlog>()
                .WithMany()
                .HasForeignKey(x => x.BlogId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrigamiUserBlog>()
                .HasOne<OrigamiUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrigamiTag>()
                .HasOne<OrigamiBlog>()
                .WithMany()
                .HasForeignKey(x => x.BlogId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrigamiQuickNote>()
                .HasOne<OrigamiBlog>()
                .WithMany()
                .HasForeignKey(x => x.BlogId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrigamiQuickNote>()
                .HasOne<OrigamiUser>()
                .WithMany()
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
