using FluentValidation;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiSubscriberValidator : AbstractValidator<OrigamiSubscriber>
    {
        public OrigamiSubscriberValidator(Text text, IWebRootPath webRootPath) : base()
        {
            RuleFor(x => x.Id).NotNull().NotEmpty().WithMessage(text.Original("Id is required"));
            RuleFor(x => x.SocialProfileId).NotNull().NotEmpty().WithMessage(text.Original("Social profile is required"));
            RuleFor(x => x.Email).Must(x => x.Has() ? x.Email() : false).WithMessage(text.Original("Email address is required"));
        }
    }
}
