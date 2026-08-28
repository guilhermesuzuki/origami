using FluentValidation;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiContentCommentReactionValidator : AbstractValidator<OrigamiContentCommentReaction>
    {
        public OrigamiContentCommentReactionValidator(Text text, IWebRootPath webRootPath) : base()
        {
            RuleFor(x => x.Id).Id(text);
            RuleFor(x => x).Must(reaction =>
            {
                var fk = new bool[] { reaction.SocialProfileId.HasValue, reaction.UserId.HasValue };
                return fk.Distinct().Count() == 2;
            }).WithMessage(text.Original("Reaction needs one author"));
        }
    }
}
