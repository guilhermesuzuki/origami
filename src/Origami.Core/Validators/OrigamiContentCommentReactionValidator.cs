using FluentValidation;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiContentCommentReactionValidator : BaseValidator<OrigamiContentCommentReaction>
    {
        public OrigamiContentCommentReactionValidator(Text text, IWebRootPath webRootPath) : base(text, webRootPath)
        {
            RuleFor(x => x.Id).Id(text);
        }
    }
}
