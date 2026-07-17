using Origami.Core.Models;
using Origami.Core.Models.Events;

namespace Origami.Core.Data
{
    public interface IEventRepository
    {
        Result<SocialProfileDeletesCommentEvent> SocialProfileDeletesComment(Guid socialProfile, Guid comment);
        Result<SocialProfileEditsCommentEvent> SocialProfileEditsComment(Guid socialProfile, Guid comment);
        Result<SocialProfileLogsIntoWebsiteEvent> SocialProfileLogsIntoWebsite(Guid socialProfile);
        Result<SocialProfileReactsToCommentEvent> SocialProfileReactsToComment(Guid socialProfile, Guid reaction);
        Result<SocialProfileReactsToContentEvent> SocialProfileReactsToContent(Guid socialProfile, Guid reaction);
        Result<SocialProfileRepliesToCommentEvent> SocialProfileRepliesToComment(Guid socialProfile, Guid comment);
        Result<SocialProfileRepliesToContentEvent> SocialProfileRepliesToContent(Guid socialProfile, Guid content);
        Result<SocialProfileSubscribesToWebsiteEvent> SocialProfileSubscribesToWebsite(Guid socialProfile);
        Result<SocialProfileUnsubscribesFromWebsiteEvent> SocialProfileUnsubscribesFromWebsite(Guid socialProfile);
    }
}
