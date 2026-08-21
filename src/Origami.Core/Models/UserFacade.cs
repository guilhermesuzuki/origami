using Origami.Core.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Origami.Core.Models
{
    public class UserFacade :
        IUserFacade,
        IChanged
    {
        protected readonly ISuperRepository _super;
        protected Guid _blogId = new();
        protected IEnumerable<OrigamiBlog> _blogsTheUserHasAccessTo = Enumerable.Empty<OrigamiBlog>();
        protected Guid _id = Guid.NewGuid();
        protected bool _incognitoMode = false;
        protected ObservableCollection<Result> _results = new();
        protected string _searchTerm = string.Empty;
        protected bool _showCookieConsent = false;
        protected Guid _socialProfileId = Guid.Empty;
        protected Guid _userId = Guid.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public UserFacade(ISuperRepository super) : base()
        {
            _super = super;

            _results.CollectionChanged += (sender, e) =>
            {
                Changed?.Invoke(this, new PropertyChangedEventArgs(nameof(Results)));
            };
            this.Changed += (sender, e) =>
            {
                if (e.PropertyName == nameof(IUserFacade.UserId))
                {
                    this.BlogId = this.BlogsTheUserHasAccessTo.FirstOrDefault()?.Id ?? Guid.Empty;
                }
            };
        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, p) => { };
        public event EventHandler<EntityOperation>? EntityHasChanged;

        public Guid BlogId
        {
            get => _blogId;
            set => this.Set(ref _blogId, value, Changed);
        }

        public IEnumerable<OrigamiBlog> BlogsTheUserHasAccessTo
        {
            get
            {
                var query = from b in _super.Blogs.ReadFromCache()
                            join u in _super.UserBlogs.ReadFromCache() on b.Id equals u.BlogId
                            where u.UserId == User.Id
                            orderby b.IsPrimary ? 0 : 1, b.Name
                            select b;

                var blogs = query.ToList();
                if (blogs.Any(x => x.Id == this.BlogId) == false)
                {
                    this.BlogId = blogs.FirstOrDefault()?.Id ?? Guid.Empty;
                }

                return blogs;
            }
        }

        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
        }

        public bool IncognitoMode
        {
            get => _incognitoMode;
            set => this.Set(ref _incognitoMode, value, Changed);
        }

        public Result Result
        {
            set
            {
                _results.Add(value);
                Changed(this, new PropertyChangedEventArgs(nameof(Result)));
            }
        }

        public IList<Result> Results => _results;

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                this.Set(ref _searchTerm, value, Changed);
            }
        }

        public bool ShowCookieConsent
        {
            get => _showCookieConsent;
            set => this.Set(ref _showCookieConsent, value, Changed);
        }

        public OrigamiSocialProfile SocialProfile
        {
            get => this._super.SocialProfiles.ReadFromCache().Id(this.SocialProfileId) ?? OrigamiSocialProfile.AnonymousUser;
        }

        public Guid SocialProfileId
        {
            get => _socialProfileId;
            set => this.Set(ref _socialProfileId, value, Changed);
        }

        public OrigamiUser User
        {
            get => this._super.Users.ReadFromCache().Id(this.UserId) ?? OrigamiUser.AnonymousUser;
        }

        public Guid UserId
        {
            get => _userId;
            set => this.Set(ref _userId, value, Changed);
        }

        public void EntityChanged(object sender, EntityOperation operation)
        {
            EntityHasChanged?.Invoke(sender, operation);
        }
    }
}
