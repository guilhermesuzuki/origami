using FluentValidation;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiUserValidator : BaseValidator<OrigamiUser>
    {
        public OrigamiUserValidator(Text text, IWebRootPath webRootPath) : base(text, webRootPath)
        {
            RuleFor(x => x.Id).Id(text);
            RuleFor(x => x.NanoId).NanoId(text);
            RuleFor(x => x.HeaderImage).HeaderImage(text, webRootPath);
            RuleFor(x => x.DisplayName).DisplayName(text);
            RuleFor(x => x).DisplayNameMustBeDifferentThanUsername(text);
            RuleFor(x => x.Username).Cascade(CascadeMode.Stop).Username(text);
            RuleFor(x => x.EmailAddress)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage(text.Original("Email address is required"))
                .MaximumLength(100).WithMessage(text.Original("Email address cannot exceed 100 characters"))
                .EmailAddress();
            RuleFor(x => x.FirstName)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage(text.Original("First name is required"))
                .NotEmpty().WithMessage(text.Original("First name is required"))
                .MaximumLength(100).WithMessage(text.Original("First name cannot exceed 100 characters"));
            RuleFor(x => x.LastName)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage(text.Original("Last name is required"))
                .NotEmpty().WithMessage(text.Original("Last name is required"))
                .MaximumLength(200).WithMessage(text.Original("Last name cannot exceed 200 characters"));

            RuleFor(x => x.Website).Website(text);
            RuleFor(x => x.GitHub).Website(text, field: "GitHub");
            RuleFor(x => x.LinkedIn).Website(text, field: "LinkedIn");
            RuleFor(x => x.Facebook).Website(text, field: "Facebook");
            RuleFor(x => x.Instagram).Website(text, field: "Instagram");
        }
    }
}
