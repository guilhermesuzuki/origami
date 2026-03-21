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
        public DbSet<OrigamiPingService> PageServices { get; set; }

        /// <summary>
        /// Page Views
        /// </summary>
        public DbSet<OrigamiPageView> PageViews { get; set; }

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
        /// Post Categories
        /// </summary>
        public DbSet<OrigamiPostCategory> PostCategories { get; set; }

        /// <summary>
        /// Post Comment Ratings
        /// </summary>
        public DbSet<OrigamiPostCommentReaction> PostCommentReactions { get; set; }

        /// <summary>
        /// Post Comments
        /// </summary>
        public DbSet<OrigamiPostComment> PostComments { get; set; }

        /// <summary>
        /// Post Notifications
        /// </summary>
        public DbSet<OrigamiPostNotification> PostNotifications { get; set; }

        /// <summary>
        /// Post Ratings
        /// </summary>
        public DbSet<OrigamiPostRating> PostRatings { get; set; }

        /// <summary>
        /// Posts
        /// </summary>
        public DbSet<OrigamiPost> Posts { get; set; }

        /// <summary>
        /// Post Tags
        /// </summary>
        public DbSet<OrigamiPostTag> PostTags { get; set; }

        /// <summary>
        /// Post Views
        /// </summary>
        public DbSet<OrigamiPostView> PostViews { get; set; }

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

        /// <summary>
        /// Video Categories
        /// </summary>
        public DbSet<OrigamiVideoCategory> VideoCategories { get; set; }

        /// <summary>
        /// Video Comment Ratings
        /// </summary>
        public DbSet<OrigamiVideoCommentReaction> VideoCommentReactions { get; set; }

        /// <summary>
        /// Video Comments
        /// </summary>
        public DbSet<OrigamiVideoComment> VideoComments { get; set; }

        /// <summary>
        /// Video Notifications
        /// </summary>
        public DbSet<OrigamiVideoNotification> VideoNotifications { get; set; }

        /// <summary>
        /// Video Ratings
        /// </summary>
        public DbSet<OrigamiVideoRating> VideoRatings { get; set; }

        /// <summary>
        /// Videos
        /// </summary>
        public DbSet<OrigamiVideo> Videos { get; set; }

        /// <summary>
        /// Video Tags
        /// </summary>
        public DbSet<OrigamiVideoTag> VideoTags { get; set; }

        /// <summary>
        /// Video Views
        /// </summary>
        public DbSet<OrigamiVideoView> VideoViews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Owned<Location>();
            modelBuilder.Owned<OrigamiFile>();
            modelBuilder.Owned<Content>();

            modelBuilder.Entity<OrigamiCategory>()
                .HasOne<OrigamiCategory>()
                .WithMany()
                .HasForeignKey(x => x.ParentId);

            modelBuilder.Entity<OrigamiPage>()
                .HasOne<OrigamiPage>()
                .WithMany()
                .HasForeignKey(x => x.ParentId);

            modelBuilder.Entity<OrigamiPostComment>()
                .HasOne<OrigamiPostComment>()
                .WithMany()
                .HasForeignKey(x => x.ParentId);

            modelBuilder.Entity<OrigamiVideoComment>()
                .HasOne<OrigamiVideoComment>()
                .WithMany()
                .HasForeignKey(x => x.ParentId);

            modelBuilder.Entity<OrigamiCategory>()
                .HasOne<OrigamiBlog>()
                .WithMany()
                .HasForeignKey(x => x.BlogId);

            modelBuilder.Entity<OrigamiPage>()
                .HasOne<OrigamiBlog>()
                .WithMany()
                .HasForeignKey(x => x.BlogId);

            modelBuilder.Entity<OrigamiPost>()
                .HasOne<OrigamiBlog>()
                .WithMany()
                .HasForeignKey(x => x.BlogId);

            modelBuilder.Entity<OrigamiVideo>()
                .HasOne<OrigamiBlog>()
                .WithMany()
                .HasForeignKey(x => x.BlogId);

            modelBuilder.Entity<OrigamiQuickSetting>()
                .HasOne<OrigamiBlog>()
                .WithMany()
                .HasForeignKey(x => x.BlogId);

            modelBuilder.Entity<OrigamiPageView>()
                .HasOne<OrigamiPage>()
                .WithMany()
                .HasForeignKey(x => x.PageId);

            modelBuilder.Entity<OrigamiPhysicalPageView>()
                .HasOne<OrigamiPhysicalPage>()
                .WithMany()
                .HasForeignKey(x => x.PhysicalPageId);

            modelBuilder.Entity<OrigamiPostView>()
                .HasOne<OrigamiPost>()
                .WithMany()
                .HasForeignKey(x => x.PostId);

            modelBuilder.Entity<OrigamiVideoView>()
                .HasOne<OrigamiVideo>()
                .WithMany()
                .HasForeignKey(x => x.VideoId);

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

            modelBuilder.Entity<OrigamiPost>()
                .HasOne<OrigamiUser>()
                .WithMany()
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrigamiPostCategory>()
                .HasOne<OrigamiCategory>()
                .WithMany()
                .HasForeignKey(x => x.CategoryId);

            modelBuilder.Entity<OrigamiPostCategory>()
                .HasOne(x => x.Post)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrigamiVideo>()
                .HasOne<OrigamiUser>()
                .WithMany()
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrigamiVideoCategory>()
                .HasOne<OrigamiCategory>()
                .WithMany()
                .HasForeignKey(x => x.CategoryId);

            modelBuilder.Entity<OrigamiVideoCategory>()
                .HasOne(x => x.Video)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

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

            modelBuilder.Entity<OrigamiSpecialPageView>()
                .HasOne<OrigamiSpecialPage>()
                .WithMany()
                .HasForeignKey(x => x.SpecialPageId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrigamiPostComment>()
                .HasOne<OrigamiPost>()
                .WithMany()
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrigamiPostComment>()
                .HasOne<OrigamiSocialProfile>()
                .WithMany()
                .HasForeignKey(x => x.SocialProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrigamiPostComment>()
                .HasOne<OrigamiSocialProfile>()
                .WithMany()
                .HasForeignKey(x => x.PinnedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrigamiPostComment>()
                .HasOne<OrigamiSocialProfile>()
                .WithMany()
                .HasForeignKey(x => x.ModeratedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrigamiVideoComment>()
                .HasOne<OrigamiVideo>()
                .WithMany()
                .HasForeignKey(x => x.VideoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrigamiVideoComment>()
                .HasOne<OrigamiSocialProfile>()
                .WithMany()
                .HasForeignKey(x => x.SocialProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrigamiVideoComment>()
                .HasOne<OrigamiSocialProfile>()
                .WithMany()
                .HasForeignKey(x => x.PinnedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrigamiVideoComment>()
                .HasOne<OrigamiSocialProfile>()
                .WithMany()
                .HasForeignKey(x => x.ModeratedById)
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

            modelBuilder.Entity<OrigamiPostCommentReaction>().Property(c => c.Reaction).UseCollation("Latin1_General_BIN2");
            modelBuilder.Entity<OrigamiVideoCommentReaction>().Property(c => c.Reaction).UseCollation("Latin1_General_BIN2");
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
