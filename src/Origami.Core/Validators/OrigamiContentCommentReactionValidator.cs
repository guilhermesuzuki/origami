using FluentValidation;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiContentCommentReactionValidator : AbstractValidator<OrigamiContentCommentReaction>
    {
        public OrigamiContentCommentReactionValidator(Text text, IWebRootPath webRootPath) : base()
        {
            RuleFor(x => x.Id).Id(text);
        }
    }
}
