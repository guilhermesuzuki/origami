using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class HubContentSpecialMessageValidator : AbstractValidator<HubContentSpecialMessage>
    {
        public HubContentSpecialMessageValidator(Text text, IWebRootPath webRootPath, IDbContextFactory<OrigamiDbContext> dbContextFactory) : base()
        {
            RuleFor(x => x.Entity).SetValidator(new OrigamiContentValidator(text, webRootPath, dbContextFactory, isBlogIdRequired: false));

            RuleFor(x => x.Entity.Subtype)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(text.Original("Type is required for special messages"))
                .Must(x =>
                {
                    if (x.Has() == true)
                    {
                        var allowedTypes = new List<string>
                        {
                            OrigamiSpecialMessageTypes.None.ToString(),
                            OrigamiSpecialMessageTypes.Danger.ToString(),
                            OrigamiSpecialMessageTypes.Info.ToString(),
                            OrigamiSpecialMessageTypes.Success.ToString(),
                            OrigamiSpecialMessageTypes.Warning.ToString(),
                        };
                        return allowedTypes.Contains(x);
                    }
                    return false;
                })
                .WithMessage(text.Original("Type must be a valid special message type"));

            RuleFor(x => x).Must(x => x.Entity.StartDate <= x.Entity.EndDate).WithMessage(text.Original("Start date cannot be later than end date"));
        }
    }
}
