using FluentValidation;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiContentReactionValidator : AbstractValidator<OrigamiContentReaction>
    {
        public OrigamiContentReactionValidator(Text text, IWebRootPath webRootPath) : base()
        {
            RuleFor(x => x.Id).Id(text);
        }
    }
}
