using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiContentReactionValidator : BaseValidator<OrigamiContentReaction>
    {
        public OrigamiContentReactionValidator(Text text, IWebRootPath webRootPath) : base(text, webRootPath)
        {
            RuleFor(x => x.Id).Id(text);
        }
    }
}
