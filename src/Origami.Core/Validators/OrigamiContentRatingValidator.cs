using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiContentRatingValidator : BaseValidator<OrigamiContentRating>
    {
        public OrigamiContentRatingValidator(Text text, IWebRootPath webRootPath) : base(text, webRootPath)
        {
            RuleFor(x => x.Id).Id(text);
            RuleFor(x => x.Rating).Rating(text);
        }
    }
}
