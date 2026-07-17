using System.ComponentModel;

namespace Origami.Core.Models.Events
{
    public class SocialProfileRepliesToContentEvent : 
        OrigamiEvent,
        IChanged,
        IContentId
    {
        public SocialProfileRepliesToContentEvent() : base()
        {
            this.Type = nameof(SocialProfileRepliesToContentEvent);
        }

        protected Guid _contentId;

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, p) => { };

        public Guid ContentId
        {
            get => _contentId;
            set => this.Set(ref _contentId, value, Changed);
        }
    }
}
