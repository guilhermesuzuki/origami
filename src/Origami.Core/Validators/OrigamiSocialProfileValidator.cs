using FluentValidation;
using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Validators
{
    public class OrigamiSocialProfileValidator : AbstractValidator<OrigamiSocialProfile>
    {
        public OrigamiSocialProfileValidator() : base()
        {
            RuleFor(x => x.Id).NotNull().NotEmpty().WithMessage("Id is required");
            RuleFor(x => x.UserId).NotNull().NotEmpty().WithMessage("User Id is required");
            RuleFor(x => x.NanoId).NotNull().NotEmpty().WithMessage("Nano Id is required");

            RuleFor(x => x).Must(x =>
            {
                if (x.Name.Has() == true) return true;
                if (x.FirstName.Has() == true) return true;
                if (x.LastName.Has() == true) return true;
                return false;
            }).WithMessage("Name is required");

            RuleFor(x => x.Email).Must(x => x.Has() ? x.Email() : false).WithMessage("When informed, email must be valid");
            RuleFor(x => x.EmailFromSocialNetwork).Must(x => x.Has() ? x.Email() : false).WithMessage("When informed, email from social network must be valid");
        }
    }
}
