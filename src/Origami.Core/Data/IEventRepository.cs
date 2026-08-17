using Origami.Core.Models;
using Origami.Core.Models.Events;

namespace Origami.Core.Data
{
    public interface IEventRepository: IReadFromCache<OrigamiEvent>
    {
        Result<SocialProfileCancelsReactionToCommentEvent> SocialProfileCancelsReactionToComment(OrigamiSocialProfile socialProfile, OrigamiContentCommentReaction reaction);
        Result<SocialProfileCancelsReactionToContentEvent> SocialProfileCancelsReactionToContent(OrigamiSocialProfile socialProfile, OrigamiContentReaction reaction);

        Result<SocialProfileDeletesCommentEvent> SocialProfileDeletesComment(OrigamiSocialProfile socialProfile, OrigamiContentComment comment);
        Result<SocialProfileEditsCommentEvent> SocialProfileEditsComment(OrigamiSocialProfile socialProfile, OrigamiContentComment comment);
        Result<SocialProfileLogsIntoWebsiteEvent> SocialProfileLogsIntoWebsite(OrigamiSocialProfile socialProfile);
        Result<SocialProfilePinsCommentEvent> SocialProfilePinsComment(OrigamiSocialProfile socialProfile, OrigamiContentComment comment);
        Result<SocialProfileReactsToCommentEvent> SocialProfileReactsToComment(OrigamiSocialProfile socialProfile, OrigamiContentCommentReaction reaction);
        Result<SocialProfileReactsToContentEvent> SocialProfileReactsToContent(OrigamiSocialProfile socialProfile, OrigamiContentReaction reaction);
        Result<SocialProfileRepliesToCommentEvent> SocialProfileRepliesToComment(OrigamiSocialProfile socialProfile, OrigamiContentComment comment);
        Result<SocialProfileRepliesToContentEvent> SocialProfileRepliesToContent(OrigamiSocialProfile socialProfile, OrigamiContentComment comment);
        Result<SocialProfileSubscribesToWebsiteEvent> SocialProfileSubscribesToWebsite(OrigamiSocialProfile socialProfile);
        Result<SocialProfileUnpinsCommentEvent> SocialProfileUnpinsComment(OrigamiSocialProfile socialProfile, OrigamiContentComment comment);
        Result<SocialProfileUnsubscribesFromWebsiteEvent> SocialProfileUnsubscribesFromWebsite(OrigamiSocialProfile socialProfile);
    }
}
