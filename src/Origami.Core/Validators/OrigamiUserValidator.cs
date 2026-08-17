using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiUserValidator : AbstractValidator<OrigamiUser>
    {
        public OrigamiUserValidator(Text text, IWebRootPath webRootPath, IDbContextFactory<OrigamiDbContext> dbContextFactory) : base()
        {
            RuleFor(x => x.Id).Id(text);
            RuleFor(x => x.NanoId).NanoId(text);
            RuleFor(x => x.HeaderImage).HeaderImage(text, webRootPath);
            RuleFor(x => x.DisplayName).Cascade(CascadeMode.Stop).DisplayName(text);
            RuleFor(x => x).DisplayNameMustBeDifferentThanUsername(text);
            RuleFor(x => x.Username).Cascade(CascadeMode.Stop).Username(text);

            RuleFor(x => x.EmailAddress)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage(text.Original("Email address is required"))
                .MaximumLength(100).WithMessage(text.Original("Email address cannot exceed {0} characters", 100))
                .EmailAddress();

            RuleFor(x => x.FirstName)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage(text.Original("First name is required"))
                .NotEmpty().WithMessage(text.Original("First name is required"))
                .MaximumLength(100).WithMessage(text.Original("First name cannot exceed {0} characters", 100));

            RuleFor(x => x.LastName)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage(text.Original("Last name is required"))
                .NotEmpty().WithMessage(text.Original("Last name is required"))
                .MaximumLength(200).WithMessage(text.Original("Last name cannot exceed {0} characters", 200));

            RuleFor(x => x.Website).Website(text);
            RuleFor(x => x.GitHub).Website(text, field: "GitHub");
            RuleFor(x => x.LinkedIn).Website(text, field: "LinkedIn");
            RuleFor(x => x.Facebook).Website(text, field: "Facebook");
            RuleFor(x => x.Instagram).Website(text, field: "Instagram");

            RuleFor(x => x).Must(user =>
            {
                if (user.New == true)
                {
                    if (user.NewPassword1.Has() == false || user.NewPassword2.Has() == false) return false;
                }
                else
                {
                    if (user.NewPassword1.Has() == false && user.NewPassword2.Has() == false) return true;
                }
                if (user.NewPassword1 != user.NewPassword2) return false;
                if (user.NewPassword1.IsPasswordStrong(text) is { Ok: false }) return false;
                return true;
            }).WithMessage(text.Original("New passwords must be valid and match each other"));

            RuleFor(x => x).DisplayNameMustBeUnique(text, dbContextFactory);
            RuleFor(x => x).UsernameMustBeUnique(text, dbContextFactory);
        }
    }
}
