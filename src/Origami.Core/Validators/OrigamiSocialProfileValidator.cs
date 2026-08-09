using FluentValidation;
using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Validators
{
    public class OrigamiSocialProfileValidator : AbstractValidator<OrigamiSocialProfile>
    {
        public OrigamiSocialProfileValidator(Text text, IWebRootPath webRootPath) : base()
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

            RuleFor(x => x.Email).Must(x => x.Has() ? x.Email() : true).WithMessage("When provided, email must be valid");
            RuleFor(x => x.EmailFromSocialNetwork).Must(x => x.Has() ? x.Email() : true).WithMessage("When informed, email from social network must be valid");

            RuleFor(x => x.ProfileCoverUrl).Website(text, isRequired: false, field: "Profile cover url");
            RuleFor(x => x.ProfilePage).Website(text, isRequired: false, field: "Profile page url");
            RuleFor(x => x.ProfilePictureUrl).Website(text, isRequired: false, field: "Profile picture url");

            RuleFor(x => x.ProfileCover).Base64(text, isRequired: false, field: "Profile cover image");
            RuleFor(x => x.ProfilePicture).Base64(text, isRequired: false, field: "Profile picture image");
        }
    }
}
