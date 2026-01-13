using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Origami.Core.Models
{
    public class UserFacade :
        IUserFacade,
        IChanged
    {
        protected Guid _blogId = new();
        protected Guid _id = Guid.NewGuid();
        protected ObservableCollection<Result> _results = new();
        protected string _search = string.Empty;
        protected OrigamiSocialProfile _socialProfile = new();
        protected OrigamiUser _user = OrigamiUser.AnonymousUser;

        /// <summary>
        /// Default constructor
        /// </summary>
        public UserFacade() : base()
        {
            _results.CollectionChanged += (sender, e) =>
            {
                Changed?.Invoke(this, new PropertyChangedEventArgs(nameof(Results)));
            };
        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, p) => { };
        public event EventHandler<EntityOperation>? EntityHasChanged;

        public Guid BlogId
        {
            get => _blogId;
            set => this.Set(ref _blogId, value, Changed);
        }

        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
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

        public string Search
        {
            get => _search;
            set
            {
                this.Set(ref _search, value, Changed);
            }
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
    }
}
