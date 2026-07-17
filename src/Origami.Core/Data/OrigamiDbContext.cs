using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;
using Origami.Core.Models.Events;

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
        /// Blogs
        /// </summary>
        public DbSet<OrigamiBlog> Blogs { get; set; }

        /// <summary>
        /// Categories
        /// </summary>
        public DbSet<OrigamiCategory> Categories { get; set; }

        /// <summary>
        /// Content Categories
        /// </summary>
        public DbSet<OrigamiContentCategory> ContentCategories { get; set; }

        /// <summary>
        /// Content Comment Ratings
        /// </summary>
        public DbSet<OrigamiContentCommentReaction> ContentCommentReactions { get; set; }

        /// <summary>
        /// Content Comments
        /// </summary>
        public DbSet<OrigamiContentComment> ContentComments { get; set; }

        /// <summary>
        /// Content Histories
        /// </summary>
        public DbSet<OrigamiContentHistory> ContentHistories { get; set; }

        /// <summary>
        /// Content Ratings
        /// </summary>
        public DbSet<OrigamiContentRating> ContentRatings { get; set; }

        /// <summary>
        /// Content Reactions
        /// </summary>
        public DbSet<OrigamiContentReaction> ContentReactions { get; set; }

        /// <summary>
        /// Contents
        /// </summary>
        public DbSet<OrigamiContent> Contents { get; set; }

        /// <summary>
        /// Content Tags
        /// </summary>
        public DbSet<OrigamiContentTag> ContentTags { get; set; }

        /// <summary>
        /// Events
        /// </summary>
        public DbSet<OrigamiEvent> Events { get; set; }

        /// <summary>
        /// Pages
        /// </summary>
        public DbSet<OrigamiPage> Pages { get; set; }

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
        /// Subscribers
        /// </summary>
        public DbSet<OrigamiSubscriber> Subscribers { get; set; }

        /// <summary>
        /// User Activities
        /// </summary>
        public DbSet<OrigamiUserActivity> UserActivities { get; set; }

        /// <summary>
        /// User blogs
        /// </summary>
        public DbSet<OrigamiUserBlog> UserBlogs { get; set; }

        public DbSet<SocialProfileDeletesCommentEvent> SocialProfileDeletesCommentEvents { get; set; }

        public DbSet<SocialProfileEditsCommentEvent> SocialProfileEditsCommentEvents { get; set; }

        public DbSet<SocialProfileLogsIntoWebsiteEvent> SocialProfileLogsIntoWebsiteEvents { get; set; }

        /// <summary>
        /// User password resets
        /// </summary>
        public DbSet<OrigamiUserPasswordReset> UserPasswordResets { get; set; }

        public DbSet<SocialProfileReactsToCommentEvent> SocialProfileReactsToCommentEvents { get; set; }

        public DbSet<SocialProfileReactsToContentEvent> SocialProfileReactsToContentEvents { get; set; }

        public DbSet<SocialProfileRepliesToCommentEvent> SocialProfileRepliesToCommentEvents { get; set; }

        public DbSet<SocialProfileRepliesToContentEvent> SocialProfileRepliesToContentEvents { get; set; }

        /// <summary>
        /// User Roles
        /// </summary>
        public DbSet<OrigamiUserRole> UserRoles { get; set; }

        /// <summary>
        /// Users
        /// </summary>
        public DbSet<OrigamiUser> Users { get; set; }

        public DbSet<SocialProfileSubscribesToWebsiteEvent> SocialProfileSubscribesToWebsiteEvents { get; set; }

        /// <summary>
        /// User Trashes
        /// </summary>
        public DbSet<OrigamiUserTrash> UserTrashes { get; set; }

        public DbSet<SocialProfileUnsubscribesFromWebsiteEvent> SocialProfileUnsubscribesFromWebsiteEvents { get; set; }

        /// <summary>
        /// Videos
        /// </summary>
        public DbSet<OrigamiVideo> Videos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Owned<Location>();
            modelBuilder.Owned<OrigamiFile>();

            modelBuilder.Entity<OrigamiContent>()
                .HasDiscriminator(x => x.Type)
                .HasValue<OrigamiPage>(nameof(OrigamiPage))
                .HasValue<OrigamiPost>(nameof(OrigamiPost))
                .HasValue<OrigamiSpecialPage>(nameof(OrigamiSpecialPage))
                .HasValue<OrigamiQuickNote>(nameof(OrigamiQuickNote))
                .HasValue<OrigamiVideo>(nameof(OrigamiVideo))
                ;

            //social-network as string
            modelBuilder
                .Entity<OrigamiSocialProfile>()
                .Property(e => e.SocialNetwork)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<SocialNetworks>(v));

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

            modelBuilder.Entity<OrigamiCategory>().HasOne<OrigamiCategory>().WithMany().HasForeignKey(x => x.ParentId);
            modelBuilder.Entity<OrigamiCategory>().HasOne<OrigamiBlog>().WithMany().HasForeignKey(x => x.BlogId);
            modelBuilder.Entity<OrigamiPhysicalPageView>().HasOne<OrigamiPhysicalPage>().WithMany().HasForeignKey(x => x.PhysicalPageId);
            modelBuilder.Entity<OrigamiPhysicalPageReaction>().HasOne<OrigamiPhysicalPage>().WithMany().HasForeignKey(x => x.PhysicalPageId);
            modelBuilder.Entity<OrigamiRightRole>().HasOne<OrigamiRole>().WithMany().HasForeignKey(x => x.RoleId);
            modelBuilder.Entity<OrigamiRightRole>().HasOne<OrigamiRight>().WithMany().HasForeignKey(x => x.RightId);

            modelBuilder.Entity<OrigamiUserPasswordReset>().HasOne<OrigamiUser>().WithMany().HasForeignKey(x => x.AuthorId);
            modelBuilder.Entity<OrigamiUserPasswordReset>().HasOne<OrigamiUser>().WithMany().HasForeignKey(x => x.UserId);

            modelBuilder.Entity<OrigamiUserRole>().HasOne<OrigamiUser>().WithMany().HasForeignKey(x => x.UserId);
            modelBuilder.Entity<OrigamiUserRole>().HasOne<OrigamiRole>().WithMany().HasForeignKey(x => x.RoleId);

            modelBuilder.Entity<OrigamiContentCommentReaction>().Property(c => c.Reaction).UseCollation("Latin1_General_BIN2");
            modelBuilder.Entity<OrigamiContentReaction>().Property(c => c.Reaction).UseCollation("Latin1_General_BIN2");
            modelBuilder.Entity<OrigamiPhysicalPageReaction>().Property(c => c.Reaction).UseCollation("Latin1_General_BIN2");

            // Map the entity to the view
            modelBuilder.Entity<OrigamiUserActivity>().ToView("oi_vw_UserActivities");
            modelBuilder.Entity<OrigamiUserActivity>().Metadata.SetIsTableExcludedFromMigrations(true);

            // Map the entity to the view
            modelBuilder.Entity<OrigamiUserView>().ToView("oi_vw_UserViews");
            modelBuilder.Entity<OrigamiUserView>().Metadata.SetIsTableExcludedFromMigrations(true);

            // Map the entity to the view
            modelBuilder.Entity<OrigamiUserTrash>().ToView("oi_vw_UserTrashes");
            modelBuilder.Entity<OrigamiUserTrash>().Metadata.SetIsTableExcludedFromMigrations(true);

            modelBuilder.Entity<ProcessedUserView>().HasNoKey();
            modelBuilder.Entity<ProcessedUserViewForHistory>().HasNoKey();

            /*backup and restore mapping*/
            modelBuilder.Entity<OrigamiBackup>()
                .ToTable("oi_BackupRestores")
                .HasDiscriminator<bool>("Backup")
                .HasValue<OrigamiBackup>(true)
                .HasValue<OrigamiBackupRestore>(false);

            modelBuilder.Entity<OrigamiUserBlog>().HasOne<OrigamiBlog>().WithMany().HasForeignKey(x => x.BlogId);
            modelBuilder.Entity<OrigamiUserBlog>().HasOne<OrigamiUser>().WithMany().HasForeignKey(x => x.UserId);
            modelBuilder.Entity<OrigamiQuickNote>().HasOne<OrigamiBlog>().WithMany().HasForeignKey(x => x.BlogId);
            modelBuilder.Entity<OrigamiQuickNote>().HasOne<OrigamiUser>().WithMany().HasForeignKey(x => x.AuthorId);

            /*backup and restore mapping*/
            modelBuilder.Entity<OrigamiEvent>()
                .ToTable("oi_Events")
                .HasDiscriminator<string>("Type")
                .HasValue<SocialProfileDeletesCommentEvent>(nameof(SocialProfileDeletesCommentEvent))
                .HasValue<SocialProfileEditsCommentEvent>(nameof(SocialProfileEditsCommentEvent))
                .HasValue<SocialProfileLogsIntoWebsiteEvent>(nameof(SocialProfileLogsIntoWebsiteEvent))
                .HasValue<SocialProfileReactsToCommentEvent>(nameof(SocialProfileReactsToCommentEvent))
                .HasValue<SocialProfileReactsToContentEvent>(nameof(SocialProfileReactsToContentEvent))
                .HasValue<SocialProfileRepliesToCommentEvent>(nameof(SocialProfileRepliesToCommentEvent))
                .HasValue<SocialProfileRepliesToContentEvent>(nameof(SocialProfileRepliesToContentEvent))
                .HasValue<SocialProfileSubscribesToWebsiteEvent>(nameof(SocialProfileSubscribesToWebsiteEvent))
                .HasValue<SocialProfileUnsubscribesFromWebsiteEvent>(nameof(SocialProfileUnsubscribesFromWebsiteEvent))
                ;

            modelBuilder.Entity<OrigamiEvent>().HasOne<OrigamiUser>().WithMany().HasForeignKey(x => x.UserId);
            modelBuilder.Entity<OrigamiEvent>().HasOne<OrigamiSocialProfile>().WithMany().HasForeignKey(x => x.SocialProfileId);

            modelBuilder.Entity<SocialProfileDeletesCommentEvent>().HasOne<OrigamiContentComment>().WithMany().HasForeignKey(x => x.CommentId);
            modelBuilder.Entity<SocialProfileEditsCommentEvent>().HasOne<OrigamiContentComment>().WithMany().HasForeignKey(x => x.CommentId);
            modelBuilder.Entity<SocialProfileReactsToCommentEvent>().HasOne<OrigamiContentCommentReaction>().WithMany().HasForeignKey(x => x.ReactionId);
            modelBuilder.Entity<SocialProfileReactsToContentEvent>().HasOne<OrigamiContentReaction>().WithMany().HasForeignKey(x => x.ReactionId);
            modelBuilder.Entity<SocialProfileRepliesToCommentEvent>().HasOne<OrigamiContentComment>().WithMany().HasForeignKey(x => x.CommentId);
            modelBuilder.Entity<SocialProfileRepliesToContentEvent>().HasOne<OrigamiContent>().WithMany().HasForeignKey(x => x.ContentId);
        }
    }
}
