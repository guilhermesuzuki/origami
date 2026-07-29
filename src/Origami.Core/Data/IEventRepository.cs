using Origami.Core.Models;
using Origami.Core.Models.Events;

namespace Origami.Core.Data
{
    public interface IEventRepository
    {
        Result<SocialProfileDeletesCommentEvent> SocialProfileDeletesComment(OrigamiSocialProfile socialProfile, OrigamiContentComment comment);
        Result<SocialProfileEditsCommentEvent> SocialProfileEditsComment(OrigamiSocialProfile socialProfile, OrigamiContentComment comment);
        Result<SocialProfileLogsIntoWebsiteEvent> SocialProfileLogsIntoWebsite(OrigamiSocialProfile socialProfile);
        Result<SocialProfileReactsToCommentEvent> SocialProfileReactsToComment(OrigamiSocialProfile socialProfile, OrigamiContentReaction reaction);
        Result<SocialProfileReactsToContentEvent> SocialProfileReactsToContent(OrigamiSocialProfile socialProfile, OrigamiContentReaction reaction);
        Result<SocialProfileRepliesToCommentEvent> SocialProfileRepliesToComment(OrigamiSocialProfile socialProfile, OrigamiContentComment comment);
        Result<SocialProfileRepliesToContentEvent> SocialProfileRepliesToContent(OrigamiSocialProfile socialProfile, OrigamiContentComment comment);
        Result<SocialProfileSubscribesToWebsiteEvent> SocialProfileSubscribesToWebsite(OrigamiSocialProfile socialProfile);
        Result<SocialProfileUnsubscribesFromWebsiteEvent> SocialProfileUnsubscribesFromWebsite(OrigamiSocialProfile socialProfile);
    }
}
