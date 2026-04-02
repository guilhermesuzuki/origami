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
        protected OrigamiSocialProfile _socialProfile = OrigamiSocialProfile.AnonymousUser;
        protected OrigamiUser _user = OrigamiUser.AnonymousUser;

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
                if (e.PropertyName == nameof(IUserFacade.User))
                {
                    this.LoadBlogsTheUserHasAccessTo();
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
            get => _blogsTheUserHasAccessTo;
            set => this.Set(ref _blogsTheUserHasAccessTo, value, Changed);
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
            get => _socialProfile;
            set
            {
                this.Set(ref _socialProfile, value, Changed);
            }
        }

        public OrigamiUser User
        {
            get => _user;
            set => this.Set(ref _user, value, Changed);
        }
        public void EntityChanged(object sender, EntityOperation operation)
        {
            EntityHasChanged?.Invoke(sender, operation);
        }

        protected void LoadBlogsTheUserHasAccessTo()
        {
            var query = from b in _super.Blogs.ReadFromCache()
                        join u in _super.UserBlogs.ReadFromCache() on b.Id equals u.BlogId
                        where u.UserId == User.Id
                        orderby b.IsPrimary ? 0 : 1, b.Name
                        select b;

            this.BlogsTheUserHasAccessTo = query.ToList();

            var blog = this.BlogsTheUserHasAccessTo.FirstOrDefault(x => x.Id == BlogId);

            if (blog == null)
            {
                this.BlogId = BlogsTheUserHasAccessTo.FirstOrDefault()?.Id ?? Guid.Empty;
            }
        }
    }
}
