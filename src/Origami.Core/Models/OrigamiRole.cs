using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_Roles")]
    public class OrigamiRole :
        IChanged,
        IId,
        IName,
        IVersion,
        INew,
        IDeleted,
        IDateCreated,
        IDateModified
    {
        private DateTime _dateCreated;
        private DateTime? _dateModified;
        private Guid _id = Guid.NewGuid();
        private bool _isDeleted = false;
        private bool _isSystemRole = false;
        private string _name = string.Empty;
        private byte[] _version = [];

        public OrigamiRole() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public DateTime DateCreated
        {
            get => _dateCreated;
            set => this.Set(ref _dateCreated, value, Changed);
        }

        public DateTime? DateModified
        {
            get => _dateModified;
            set => this.Set(ref _dateModified, value, Changed);
        }

        [Key]
        public Guid Id
        {
            get { return _id; }
            set { this.Set(ref _id, value, Changed); }
        }

        public bool IsDeleted
        {
            get => _isDeleted;
            set => this.Set(ref _isDeleted, value, Changed);
        }

        /// <summary>
        /// Is this a system role?
        /// </summary>
        public bool IsSystemRole
        {
            get => _isSystemRole;
            set => this.Set(ref _isSystemRole, value, Changed);
        }

        [StringLength(255)]
        public string Name
        {
            get { return _name; }
            set { this.Set(ref _name, value, Changed); }
        }

        public bool New => Version.SequenceEqual([]);

        [Timestamp]
        public byte[] Version
        {
            get => _version;
            set => this.Set(ref _version, value, Changed);
        }

        #region "Role Permissions"

        private bool _accessAdminPages;
        private bool _activateBlogs;
        private bool _backupSystem;
        private bool _blockSocialProfiles;
        private bool _blockUserSelf;
        private bool _blockUsersOtherThanSelf;
        private bool _createNewBlogs;
        private bool _createNewCategories;
        private bool _createNewPages;
        private bool _createNewPosts;
        private bool _createNewQuickNotes;
        private bool _createNewRoles;
        private bool _createNewSpecialMessages;
        private bool _createNewSpecialPages;
        private bool _createNewUsers;
        private bool _createNewVideos;
        private bool _deactivateBlogs;
        private bool _deleteBlogs;
        private bool _deleteCategories;
        private bool _deleteOtherUsersPages;
        private bool _deleteOtherUsersPosts;
        private bool _deleteOtherUsersQuickNotes;
        private bool _deleteOtherUsersSpecialMessages;
        private bool _deleteOtherUsersSpecialPages;
        private bool _deleteOtherUsersVideos;
        private bool _deleteOwnPages;
        private bool _deleteOwnPosts;
        private bool _deleteOwnQuickNotes;
        private bool _deleteOwnSpecialMessages;
        private bool _deleteOwnSpecialPages;
        private bool _deleteOwnVideos;
        private bool _deleteRoles;
        private bool _deleteTags;
        private bool _deleteUserSelf;
        private bool _deleteUsersOtherThanSelf;
        private bool _editBlogs;
        private bool _editCategories;
        private bool _editOtherUsers;
        private bool _editOtherUsersPages;
        private bool _editOtherUsersPosts;
        private bool _editOtherUsersQuickNotes;
        private bool _editOtherUsersRoles;
        private bool _editOtherUsersSpecialMessages;
        private bool _editOtherUsersSpecialPages;
        private bool _editOtherUsersVideos;
        private bool _editOwnPages;
        private bool _editOwnPosts;
        private bool _editOwnQuickNotes;
        private bool _editOwnSpecialMessages;
        private bool _editOwnSpecialPages;
        private bool _editOwnUser;
        private bool _editOwnVideos;
        private bool _editRoles;
        private bool _editSystemRoles;
        private bool _editTags;
        private bool _enterMaintenanceMode;
        private bool _leaveMaintenanceMode;
        private bool _manageExtensions;
        private bool _manageFiles;
        private bool _managePackages;
        private bool _manageThemes;
        private bool _manageWidgets;
        private bool _markAsFrontPage;
        private bool _markBlogAsPrimary;
        private bool _moderateComments;
        private bool _none;
        private bool _publishOtherUsersPages;
        private bool _publishOtherUsersPosts;
        private bool _publishOtherUsersQuickNotes;
        private bool _publishOtherUsersSpecialMessages;
        private bool _publishOtherUsersSpecialPages;
        private bool _publishOtherUsersVideos;
        private bool _publishOwnPages;
        private bool _publishOwnPosts;
        private bool _publishOwnQuickNotes;
        private bool _publishOwnSpecialMessages;
        private bool _publishOwnSpecialPages;
        private bool _publishOwnVideos;
        private bool _purgeBlogs;
        private bool _purgeCategories;
        private bool _purgeComments;
        private bool _purgePages;
        private bool _purgePosts;
        private bool _purgeQuickNotes;
        private bool _purgeRoles;
        private bool _purgeSpecialMessages;
        private bool _purgeSpecialPages;
        private bool _purgeTags;
        private bool _purgeUsers;
        private bool _purgeVideos;
        private bool _resetOtherUsers2FA;
        private bool _resetOtherUsersPasswords;
        private bool _resetOwn2FA;
        private bool _resetOwnPassword;
        private bool _restoreBlogs;
        private bool _restoreCategories;
        private bool _restoreComments;
        private bool _restorePages;
        private bool _restorePosts;
        private bool _restoreQuickNotes;
        private bool _restoreRoles;
        private bool _restoreSpecialMessages;
        private bool _restoreSpecialPages;
        private bool _restoreSystem;
        private bool _restoreUsers;
        private bool _restoreVideos;
        private bool _revokeModeratorRolesFromSocialProfiles;
        private bool _submitRatingsOnPosts;
        private bool _submitRatingsOnVideos;
        private bool _turnSocialProfilesIntoModerators;
        private bool _unblockSocialProfiles;
        private bool _unblockUsers;
        private bool _unmarkAsFrontPage;
        private bool _unpublishOtherUsersPages;
        private bool _unpublishOtherUsersPosts;
        private bool _unpublishOtherUsersQuickNotes;
        private bool _unpublishOtherUsersSpecialMessages;
        private bool _unpublishOtherUsersSpecialPages;
        private bool _unpublishOtherUsersVideos;
        private bool _unpublishOwnPages;
        private bool _unpublishOwnPosts;
        private bool _unpublishOwnQuickNotes;
        private bool _unpublishOwnSpecialMessages;
        private bool _unpublishOwnSpecialPages;
        private bool _unpublishOwnVideos;
        private bool _unsubcribeSocialProfiles;
        private bool _viewBackupRestoreSystem;
        private bool _viewBlogs;
        private bool _viewCategories;
        private bool _viewComments;
        private bool _viewDashboard;
        private bool _viewDetailedErrorMessages;
        private bool _viewPages;
        private bool _viewPosts;
        private bool _viewQuickNotes;
        private bool _viewRatingsOnPosts;
        private bool _viewRatingsOnVideos;
        private bool _viewRoles;
        private bool _viewSettings;
        private bool _viewSocialProfiles;
        private bool _viewSpecialMessages;
        private bool _viewSpecialPages;
        private bool _viewTags;
        private bool _viewTrashes;
        private bool _viewUsers;
        private bool _viewVideos;
        private bool _wipeDataOutFromSocialProfiles;

        [NotMapped]
        public bool AccessAdminPages
        {
            get => _accessAdminPages;
            set => this.Set(ref _accessAdminPages, value, Changed);
        }

        [NotMapped]
        public bool ActivateBlogs
        {
            get => _activateBlogs;
            set => this.Set(ref _activateBlogs, value, Changed);
        }

        [NotMapped]
        public bool BackupSystem
        {
            get => _backupSystem;
            set => this.Set(ref _backupSystem, value, Changed);
        }

        [NotMapped]
        public bool BlockSocialProfiles
        {
            get => _blockSocialProfiles;
            set => this.Set(ref _blockSocialProfiles, value, Changed);
        }

        [NotMapped]
        public bool BlockUserSelf
        {
            get => _blockUserSelf;
            set => this.Set(ref _blockUserSelf, value, Changed);
        }

        [NotMapped]
        public bool BlockUsersOtherThanSelf
        {
            get => _blockUsersOtherThanSelf;
            set => this.Set(ref _blockUsersOtherThanSelf, value, Changed);
        }

        [NotMapped]
        public bool CreateNewBlogs
        {
            get => _createNewBlogs;
            set => this.Set(ref _createNewBlogs, value, Changed);
        }

        [NotMapped]
        public bool CreateNewCategories
        {
            get => _createNewCategories;
            set => this.Set(ref _createNewCategories, value, Changed);
        }

        [NotMapped]
        public bool CreateNewPages
        {
            get => _createNewPages;
            set => this.Set(ref _createNewPages, value, Changed);
        }

        [NotMapped]
        public bool CreateNewPosts
        {
            get => _createNewPosts;
            set => this.Set(ref _createNewPosts, value, Changed);
        }

        [NotMapped]
        public bool CreateNewQuickNotes
        {
            get => _createNewQuickNotes;
            set => this.Set(ref _createNewQuickNotes, value, Changed);
        }

        [NotMapped]
        public bool CreateNewRoles
        {
            get => _createNewRoles;
            set => this.Set(ref _createNewRoles, value, Changed);
        }

        [NotMapped]
        public bool CreateNewSpecialMessages
        {
            get => _createNewSpecialMessages;
            set => this.Set(ref _createNewSpecialMessages, value, Changed);
        }

        [NotMapped]
        public bool CreateNewSpecialPages
        {
            get => _createNewSpecialPages;
            set => this.Set(ref _createNewSpecialPages, value, Changed);
        }

        [NotMapped]
        public bool CreateNewUsers
        {
            get => _createNewUsers;
            set => this.Set(ref _createNewUsers, value, Changed);
        }

        [NotMapped]
        public bool CreateNewVideos
        {
            get => _createNewVideos;
            set => this.Set(ref _createNewVideos, value, Changed);
        }

        [NotMapped]
        public bool DeactivateBlogs
        {
            get => _deactivateBlogs;
            set => this.Set(ref _deactivateBlogs, value, Changed);
        }

        [NotMapped]
        public bool DeleteBlogs
        {
            get => _deleteBlogs;
            set => this.Set(ref _deleteBlogs, value, Changed);
        }

        [NotMapped]
        public bool DeleteCategories
        {
            get => _deleteCategories;
            set => this.Set(ref _deleteCategories, value, Changed);
        }

        [NotMapped]
        public bool DeleteOtherUsersPages
        {
            get => _deleteOtherUsersPages;
            set => this.Set(ref _deleteOtherUsersPages, value, Changed);
        }

        [NotMapped]
        public bool DeleteOtherUsersPosts
        {
            get => _deleteOtherUsersPosts;
            set => this.Set(ref _deleteOtherUsersPosts, value, Changed);
        }

        [NotMapped]
        public bool DeleteOtherUsersQuickNotes
        {
            get => _deleteOtherUsersQuickNotes;
            set => this.Set(ref _deleteOtherUsersQuickNotes, value, Changed);
        }

        [NotMapped]
        public bool DeleteOtherUsersSpecialMessages
        {
            get => _deleteOtherUsersSpecialMessages;
            set => this.Set(ref _deleteOtherUsersSpecialMessages, value, Changed);
        }

        [NotMapped]
        public bool DeleteOtherUsersSpecialPages
        {
            get => _deleteOtherUsersSpecialPages;
            set => this.Set(ref _deleteOtherUsersSpecialPages, value, Changed);
        }

        [NotMapped]
        public bool DeleteOtherUsersVideos
        {
            get => _deleteOtherUsersVideos;
            set => this.Set(ref _deleteOtherUsersVideos, value, Changed);
        }

        [NotMapped]
        public bool DeleteOwnPages
        {
            get => _deleteOwnPages;
            set => this.Set(ref _deleteOwnPages, value, Changed);
        }

        [NotMapped]
        public bool DeleteOwnPosts
        {
            get => _deleteOwnPosts;
            set => this.Set(ref _deleteOwnPosts, value, Changed);
        }

        [NotMapped]
        public bool DeleteOwnQuickNotes
        {
            get => _deleteOwnQuickNotes;
            set => this.Set(ref _deleteOwnQuickNotes, value, Changed);
        }

        [NotMapped]
        public bool DeleteOwnSpecialMessages
        {
            get => _deleteOwnSpecialMessages;
            set => this.Set(ref _deleteOwnSpecialMessages, value, Changed);
        }

        [NotMapped]
        public bool DeleteOwnSpecialPages
        {
            get => _deleteOwnSpecialPages;
            set => this.Set(ref _deleteOwnSpecialPages, value, Changed);
        }

        [NotMapped]
        public bool DeleteOwnVideos
        {
            get => _deleteOwnVideos;
            set => this.Set(ref _deleteOwnVideos, value, Changed);
        }

        [NotMapped]
        public bool DeleteRoles
        {
            get => _deleteRoles;
            set => this.Set(ref _deleteRoles, value, Changed);
        }

        [NotMapped]
        public bool DeleteTags
        {
            get => _deleteTags;
            set => this.Set(ref _deleteTags, value, Changed);
        }

        [NotMapped]
        public bool DeleteUserSelf
        {
            get => _deleteUserSelf;
            set => this.Set(ref _deleteUserSelf, value, Changed);
        }

        [NotMapped]
        public bool DeleteUsersOtherThanSelf
        {
            get => _deleteUsersOtherThanSelf;
            set => this.Set(ref _deleteUsersOtherThanSelf, value, Changed);
        }

        [NotMapped]
        public bool EditBlogs
        {
            get => _editBlogs;
            set => this.Set(ref _editBlogs, value, Changed);
        }

        [NotMapped]
        public bool EditCategories
        {
            get => _editCategories;
            set => this.Set(ref _editCategories, value, Changed);
        }

        [NotMapped]
        public bool EditOtherUsers
        {
            get => _editOtherUsers;
            set => this.Set(ref _editOtherUsers, value, Changed);
        }

        [NotMapped]
        public bool EditOtherUsersPages
        {
            get => _editOtherUsersPages;
            set => this.Set(ref _editOtherUsersPages, value, Changed);
        }

        [NotMapped]
        public bool EditOtherUsersPosts
        {
            get => _editOtherUsersPosts;
            set => this.Set(ref _editOtherUsersPosts, value, Changed);
        }

        [NotMapped]
        public bool EditOtherUsersQuickNotes
        {
            get => _editOtherUsersQuickNotes;
            set => this.Set(ref _editOtherUsersQuickNotes, value, Changed);
        }

        [NotMapped]
        public bool EditOtherUsersRoles
        {
            get => _editOtherUsersRoles;
            set => this.Set(ref _editOtherUsersRoles, value, Changed);
        }

        [NotMapped]
        public bool EditOtherUsersSpecialMessages
        {
            get => _editOtherUsersSpecialMessages;
            set => this.Set(ref _editOtherUsersSpecialMessages, value, Changed);
        }

        [NotMapped]
        public bool EditOtherUsersSpecialPages
        {
            get => _editOtherUsersSpecialPages;
            set => this.Set(ref _editOtherUsersSpecialPages, value, Changed);
        }

        [NotMapped]
        public bool EditOtherUsersVideos
        {
            get => _editOtherUsersVideos;
            set => this.Set(ref _editOtherUsersVideos, value, Changed);
        }

        [NotMapped]
        public bool EditOwnPages
        {
            get => _editOwnPages;
            set => this.Set(ref _editOwnPages, value, Changed);
        }

        [NotMapped]
        public bool EditOwnPosts
        {
            get => _editOwnPosts;
            set => this.Set(ref _editOwnPosts, value, Changed);
        }

        [NotMapped]
        public bool EditOwnQuickNotes
        {
            get => _editOwnQuickNotes;
            set => this.Set(ref _editOwnQuickNotes, value, Changed);
        }

        [NotMapped]
        public bool EditOwnSpecialMessages
        {
            get => _editOwnSpecialMessages;
            set => this.Set(ref _editOwnSpecialMessages, value, Changed);
        }

        [NotMapped]
        public bool EditOwnSpecialPages
        {
            get => _editOwnSpecialPages;
            set => this.Set(ref _editOwnSpecialPages, value, Changed);
        }

        [NotMapped]
        public bool EditOwnUser
        {
            get => _editOwnUser;
            set => this.Set(ref _editOwnUser, value, Changed);
        }

        [NotMapped]
        public bool EditOwnVideos
        {
            get => _editOwnVideos;
            set => this.Set(ref _editOwnVideos, value, Changed);
        }

        [NotMapped]
        public bool EditRoles
        {
            get => _editRoles;
            set => this.Set(ref _editRoles, value, Changed);
        }

        [NotMapped]
        public bool EditSystemRoles
        {
            get => _editSystemRoles;
            set => this.Set(ref _editSystemRoles, value, Changed);
        }

        [NotMapped]
        public bool EditTags
        {
            get => _editTags;
            set => this.Set(ref _editTags, value, Changed);
        }

        [NotMapped]
        public bool EnterMaintenanceMode
        {
            get => _enterMaintenanceMode;
            set => this.Set(ref _enterMaintenanceMode, value, Changed);
        }

        [NotMapped]
        public bool LeaveMaintenanceMode
        {
            get => _leaveMaintenanceMode;
            set => this.Set(ref _leaveMaintenanceMode, value, Changed);
        }

        [NotMapped]
        public bool ManageExtensions
        {
            get => _manageExtensions;
            set => this.Set(ref _manageExtensions, value, Changed);
        }

        [NotMapped]
        public bool ManageFiles
        {
            get => _manageFiles;
            set => this.Set(ref _manageFiles, value, Changed);
        }

        [NotMapped]
        public bool ManagePackages
        {
            get => _managePackages;
            set => this.Set(ref _managePackages, value, Changed);
        }

        [NotMapped]
        public bool ManageThemes
        {
            get => _manageThemes;
            set => this.Set(ref _manageThemes, value, Changed);
        }

        [NotMapped]
        public bool ManageWidgets
        {
            get => _manageWidgets;
            set => this.Set(ref _manageWidgets, value, Changed);
        }

        [NotMapped]
        public bool MarkAsFrontPage
        {
            get => _markAsFrontPage;
            set => this.Set(ref _markAsFrontPage, value, Changed);
        }

        [NotMapped]
        public bool MarkBlogAsPrimary
        {
            get => _markBlogAsPrimary;
            set => this.Set(ref _markBlogAsPrimary, value, Changed);
        }

        [NotMapped]
        public bool ModerateComments
        {
            get => _moderateComments;
            set => this.Set(ref _moderateComments, value, Changed);
        }

        [NotMapped]
        public bool None
        {
            get => _none;
            set => this.Set(ref _none, value, Changed);
        }

        [NotMapped]
        public bool PublishOtherUsersPages
        {
            get => _publishOtherUsersPages;
            set => this.Set(ref _publishOtherUsersPages, value, Changed);
        }

        [NotMapped]
        public bool PublishOtherUsersPosts
        {
            get => _publishOtherUsersPosts;
            set => this.Set(ref _publishOtherUsersPosts, value, Changed);
        }

        [NotMapped]
        public bool PublishOtherUsersQuickNotes
        {
            get => _publishOtherUsersQuickNotes;
            set => this.Set(ref _publishOtherUsersQuickNotes, value, Changed);
        }

        [NotMapped]
        public bool PublishOtherUsersSpecialMessages
        {
            get => _publishOtherUsersSpecialMessages;
            set => this.Set(ref _publishOtherUsersSpecialMessages, value, Changed);
        }

        [NotMapped]
        public bool PublishOtherUsersSpecialPages
        {
            get => _publishOtherUsersSpecialPages;
            set => this.Set(ref _publishOtherUsersSpecialPages, value, Changed);
        }

        [NotMapped]
        public bool PublishOtherUsersVideos
        {
            get => _publishOtherUsersVideos;
            set => this.Set(ref _publishOtherUsersVideos, value, Changed);
        }

        [NotMapped]
        public bool PublishOwnPages
        {
            get => _publishOwnPages;
            set => this.Set(ref _publishOwnPages, value, Changed);
        }

        [NotMapped]
        public bool PublishOwnPosts
        {
            get => _publishOwnPosts;
            set => this.Set(ref _publishOwnPosts, value, Changed);
        }

        [NotMapped]
        public bool PublishOwnQuickNotes
        {
            get => _publishOwnQuickNotes;
            set => this.Set(ref _publishOwnQuickNotes, value, Changed);
        }

        [NotMapped]
        public bool PublishOwnSpecialMessages
        {
            get => _publishOwnSpecialMessages;
            set => this.Set(ref _publishOwnSpecialMessages, value, Changed);
        }

        [NotMapped]
        public bool PublishOwnSpecialPages
        {
            get => _publishOwnSpecialPages;
            set => this.Set(ref _publishOwnSpecialPages, value, Changed);
        }

        [NotMapped]
        public bool PublishOwnVideos
        {
            get => _publishOwnVideos;
            set => this.Set(ref _publishOwnVideos, value, Changed);
        }

        [NotMapped]
        public bool PurgeBlogs
        {
            get => _purgeBlogs;
            set => this.Set(ref _purgeBlogs, value, Changed);
        }

        [NotMapped]
        public bool PurgeCategories
        {
            get => _purgeCategories;
            set => this.Set(ref _purgeCategories, value, Changed);
        }

        [NotMapped]
        public bool PurgeComments
        {
            get => _purgeComments;
            set => this.Set(ref _purgeComments, value, Changed);
        }

        [NotMapped]
        public bool PurgePages
        {
            get => _purgePages;
            set => this.Set(ref _purgePages, value, Changed);
        }

        [NotMapped]
        public bool PurgePosts
        {
            get => _purgePosts;
            set => this.Set(ref _purgePosts, value, Changed);
        }

        [NotMapped]
        public bool PurgeQuickNotes
        {
            get => _purgeQuickNotes;
            set => this.Set(ref _purgeQuickNotes, value, Changed);
        }

        [NotMapped]
        public bool PurgeRoles
        {
            get => _purgeRoles;
            set => this.Set(ref _purgeRoles, value, Changed);
        }

        [NotMapped]
        public bool PurgeSpecialMessages
        {
            get => _purgeSpecialMessages;
            set => this.Set(ref _purgeSpecialMessages, value, Changed);
        }

        [NotMapped]
        public bool PurgeSpecialPages
        {
            get => _purgeSpecialPages;
            set => this.Set(ref _purgeSpecialPages, value, Changed);
        }

        [NotMapped]
        public bool PurgeTags
        {
            get => _purgeTags;
            set => this.Set(ref _purgeTags, value, Changed);
        }

        [NotMapped]
        public bool PurgeUsers
        {
            get => _purgeUsers;
            set => this.Set(ref _purgeUsers, value, Changed);
        }

        [NotMapped]
        public bool PurgeVideos
        {
            get => _purgeVideos;
            set => this.Set(ref _purgeVideos, value, Changed);
        }

        [NotMapped]
        public bool ResetOtherUsers2FA
        {
            get => _resetOtherUsers2FA;
            set => this.Set(ref _resetOtherUsers2FA, value, Changed);
        }

        [NotMapped]
        public bool ResetOtherUsersPasswords
        {
            get => _resetOtherUsersPasswords;
            set => this.Set(ref _resetOtherUsersPasswords, value, Changed);
        }

        [NotMapped]
        public bool ResetOwn2FA
        {
            get => _resetOwn2FA;
            set => this.Set(ref _resetOwn2FA, value, Changed);
        }
        [NotMapped]
        public bool ResetOwnPassword
        {
            get => _resetOwnPassword;
            set => this.Set(ref _resetOwnPassword, value, Changed);
        }

        [NotMapped]
        public bool RestoreBlogs
        {
            get => _restoreBlogs;
            set => this.Set(ref _restoreBlogs, value, Changed);
        }

        [NotMapped]
        public bool RestoreCategories
        {
            get => _restoreCategories;
            set => this.Set(ref _restoreCategories, value, Changed);
        }

        [NotMapped]
        public bool RestoreComments
        {
            get => _restoreComments;
            set => this.Set(ref _restoreComments, value, Changed);
        }

        [NotMapped]
        public bool RestorePages
        {
            get => _restorePages;
            set => this.Set(ref _restorePages, value, Changed);
        }

        [NotMapped]
        public bool RestorePosts
        {
            get => _restorePosts;
            set => this.Set(ref _restorePosts, value, Changed);
        }

        [NotMapped]
        public bool RestoreQuickNotes
        {
            get => _restoreQuickNotes;
            set => this.Set(ref _restoreQuickNotes, value, Changed);
        }

        [NotMapped]
        public bool RestoreRoles
        {
            get => _restoreRoles;
            set => this.Set(ref _restoreRoles, value, Changed);
        }

        [NotMapped]
        public bool RestoreSpecialMessages
        {
            get => _restoreSpecialMessages;
            set => this.Set(ref _restoreSpecialMessages, value, Changed);
        }

        [NotMapped]
        public bool RestoreSpecialPages
        {
            get => _restoreSpecialPages;
            set => this.Set(ref _restoreSpecialPages, value, Changed);
        }

        [NotMapped]
        public bool RestoreSystem
        {
            get => _restoreSystem;
            set => this.Set(ref _restoreSystem, value, Changed);
        }

        [NotMapped]
        public bool RestoreUsers
        {
            get => _restoreUsers;
            set => this.Set(ref _restoreUsers, value, Changed);
        }

        [NotMapped]
        public bool RestoreVideos
        {
            get => _restoreVideos;
            set => this.Set(ref _restoreVideos, value, Changed);
        }

        [NotMapped]
        public bool RevokeModeratorRolesFromSocialProfiles
        {
            get => _revokeModeratorRolesFromSocialProfiles;
            set => this.Set(ref _revokeModeratorRolesFromSocialProfiles, value, Changed);
        }

        [NotMapped]
        public bool SubmitRatingsOnPosts
        {
            get => _submitRatingsOnPosts;
            set => this.Set(ref _submitRatingsOnPosts, value, Changed);
        }

        [NotMapped]
        public bool SubmitRatingsOnVideos
        {
            get => _submitRatingsOnVideos;
            set => this.Set(ref _submitRatingsOnVideos, value, Changed);
        }

        [NotMapped]
        public bool TurnSocialProfilesIntoModerators
        {
            get => _turnSocialProfilesIntoModerators;
            set => this.Set(ref _turnSocialProfilesIntoModerators, value, Changed);
        }

        [NotMapped]
        public bool UnblockSocialProfiles
        {
            get => _unblockSocialProfiles;
            set => this.Set(ref _unblockSocialProfiles, value, Changed);
        }

        [NotMapped]
        public bool UnblockUsers
        {
            get => _unblockUsers;
            set => this.Set(ref _unblockUsers, value, Changed);
        }

        [NotMapped]
        public bool UnmarkAsFrontPage
        {
            get => _unmarkAsFrontPage;
            set => this.Set(ref _unmarkAsFrontPage, value, Changed);
        }

        [NotMapped]
        public bool UnpublishOtherUsersPages
        {
            get => _unpublishOtherUsersPages;
            set => this.Set(ref _unpublishOtherUsersPages, value, Changed);
        }

        [NotMapped]
        public bool UnpublishOtherUsersPosts
        {
            get => _unpublishOtherUsersPosts;
            set => this.Set(ref _unpublishOtherUsersPosts, value, Changed);
        }

        [NotMapped]
        public bool UnpublishOtherUsersQuickNotes
        {
            get => _unpublishOtherUsersQuickNotes;
            set => this.Set(ref _unpublishOtherUsersQuickNotes, value, Changed);
        }

        [NotMapped]
        public bool UnpublishOtherUsersSpecialMessages
        {
            get => _unpublishOtherUsersSpecialMessages;
            set => this.Set(ref _unpublishOtherUsersSpecialMessages, value, Changed);
        }

        [NotMapped]
        public bool UnpublishOtherUsersSpecialPages
        {
            get => _unpublishOtherUsersSpecialPages;
            set => this.Set(ref _unpublishOtherUsersSpecialPages, value, Changed);
        }

        [NotMapped]
        public bool UnpublishOtherUsersVideos
        {
            get => _unpublishOtherUsersVideos;
            set => this.Set(ref _unpublishOtherUsersVideos, value, Changed);
        }

        [NotMapped]
        public bool UnpublishOwnPages
        {
            get => _unpublishOwnPages;
            set => this.Set(ref _unpublishOwnPages, value, Changed);
        }

        [NotMapped]
        public bool UnpublishOwnPosts
        {
            get => _unpublishOwnPosts;
            set => this.Set(ref _unpublishOwnPosts, value, Changed);
        }

        [NotMapped]
        public bool UnpublishOwnQuickNotes
        {
            get => _unpublishOwnQuickNotes;
            set => this.Set(ref _unpublishOwnQuickNotes, value, Changed);
        }
        [NotMapped]
        public bool UnpublishOwnSpecialMessages
        {
            get => _unpublishOwnSpecialMessages;
            set => this.Set(ref _unpublishOwnSpecialMessages, value, Changed);
        }

        [NotMapped]
        public bool UnpublishOwnSpecialPages
        {
            get => _unpublishOwnSpecialPages;
            set => this.Set(ref _unpublishOwnSpecialPages, value, Changed);
        }

        [NotMapped]
        public bool UnpublishOwnVideos
        {
            get => _unpublishOwnVideos;
            set => this.Set(ref _unpublishOwnVideos, value, Changed);
        }

        [NotMapped]
        public bool UnsubcribeSocialProfiles
        {
            get => _unsubcribeSocialProfiles;
            set => this.Set(ref _unsubcribeSocialProfiles, value, Changed);
        }

        [NotMapped]
        public bool ViewBackupRestoreSystem
        {
            get => _viewBackupRestoreSystem;
            set => this.Set(ref _viewBackupRestoreSystem, value, Changed);
        }

        [NotMapped]
        public bool ViewBlogs
        {
            get => _viewBlogs;
            set => this.Set(ref _viewBlogs, value, Changed);
        }

        [NotMapped]
        public bool ViewCategories
        {
            get => _viewCategories;
            set => this.Set(ref _viewCategories, value, Changed);
        }

        [NotMapped]
        public bool ViewComments
        {
            get => _viewComments;
            set => this.Set(ref _viewComments, value, Changed);
        }

        [NotMapped]
        public bool ViewDashboard
        {
            get => _viewDashboard;
            set => this.Set(ref _viewDashboard, value, Changed);
        }

        [NotMapped]
        public bool ViewDetailedErrorMessages
        {
            get => _viewDetailedErrorMessages;
            set => this.Set(ref _viewDetailedErrorMessages, value, Changed);
        }

        [NotMapped]
        public bool ViewPages
        {
            get => _viewPages;
            set => this.Set(ref _viewPages, value, Changed);
        }

        [NotMapped]
        public bool ViewPosts
        {
            get => _viewPosts;
            set => this.Set(ref _viewPosts, value, Changed);
        }

        [NotMapped]
        public bool ViewQuickNotes
        {
            get => _viewQuickNotes;
            set => this.Set(ref _viewQuickNotes, value, Changed);
        }

        [NotMapped]
        public bool ViewRatingsOnPosts
        {
            get => _viewRatingsOnPosts;
            set => this.Set(ref _viewRatingsOnPosts, value, Changed);
        }

        [NotMapped]
        public bool ViewRatingsOnVideos
        {
            get => _viewRatingsOnVideos;
            set => this.Set(ref _viewRatingsOnVideos, value, Changed);
        }

        [NotMapped]
        public bool ViewRoles
        {
            get => _viewRoles;
            set => this.Set(ref _viewRoles, value, Changed);
        }

        [NotMapped]
        public bool ViewSettings
        {
            get => _viewSettings;
            set => this.Set(ref _viewSettings, value, Changed);
        }

        [NotMapped]
        public bool ViewSocialProfiles
        {
            get => _viewSocialProfiles;
            set => this.Set(ref _viewSocialProfiles, value, Changed);
        }

        [NotMapped]
        public bool ViewSpecialMessages
        {
            get => _viewSpecialMessages;
            set => this.Set(ref _viewSpecialMessages, value, Changed);
        }

        [NotMapped]
        public bool ViewSpecialPages
        {
            get => _viewSpecialPages;
            set => this.Set(ref _viewSpecialPages, value, Changed);
        }

        [NotMapped]
        public bool ViewTags
        {
            get => _viewTags;
            set => this.Set(ref _viewTags, value, Changed);
        }

        [NotMapped]
        public bool ViewTrashes
        {
            get => _viewTrashes;
            set => this.Set(ref _viewTrashes, value, Changed);
        }

        [NotMapped]
        public bool ViewUsers
        {
            get => _viewUsers;
            set => this.Set(ref _viewUsers, value, Changed);
        }

        [NotMapped]
        public bool ViewVideos
        {
            get => _viewVideos;
            set => this.Set(ref _viewVideos, value, Changed);
        }

        [NotMapped]
        public bool WipeDataOutFromSocialProfiles
        {
            get => _wipeDataOutFromSocialProfiles;
            set => this.Set(ref _wipeDataOutFromSocialProfiles, value, Changed);
        }
        /*
        purge
        restore
        */


        /// <summary>
        /// Extracts all the rights based on the property names
        /// </summary>
        /// <returns></returns>
        public static List<OrigamiRight> GetRights()
        {
            List<OrigamiRight> result = new();

            foreach (var property in typeof(OrigamiRole).GetProperties())
            {
                if (property.CanRead == false) continue;
                if (property.CanWrite == false) continue;
                if (property.GetCustomAttributes(true).OfType<NotMappedAttribute>().Any() == false) continue;
                if (property.PropertyType != typeof(bool)) continue;
                result.Add(new() { Name = property.Name });
            }

            return result;
        }

        /// <summary>
        /// Extracts all the rights based on the property names
        /// </summary>
        /// <returns></returns>
        public List<OrigamiRightRole> GetRightRoles(IEnumerable<OrigamiRight> rights)
        {
            List<OrigamiRightRole> result = new();

            foreach (var property in GetType().GetProperties())
            {
                if (property.CanRead == false) continue;
                if (property.CanWrite == false) continue;
                if (property.GetCustomAttributes(true).OfType<NotMappedAttribute>().Any() == false) continue;
                if (property.PropertyType != typeof(bool)) continue;
                if (property.GetValue(this)!.Equals(false)) continue;

                var right = rights.FirstOrDefault(x => x.Name == property.Name);
                if (right == null)
                {
                    throw new Exception($"{property.Name} could not be found in {this.GetType().FullName}");
                }

                result.Add(new() { RoleId = this.Id, RightId = right.Id, });
            }

            return result;
        }
        #endregion
    }
}
