using FluentValidation;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiContentRatingValidator : AbstractValidator<OrigamiContentRating>
    {
        public OrigamiContentRatingValidator(Text text, IWebRootPath webRootPath) : base()
        {
            RuleFor(x => x.Id).Id(text);
            RuleFor(x => x.Rating).Rating(text);
        }
    }
}
