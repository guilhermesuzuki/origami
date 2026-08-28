using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiSpecialMessageValidator : AbstractValidator<OrigamiSpecialMessage>
    {
        public OrigamiSpecialMessageValidator(Text text, IWebRootPath webRootPath, IDbContextFactory<OrigamiDbContext> dbContextFactory) : base()
        {
            RuleFor(x => x.Id).Id(text);
            RuleFor(x => x.NanoId).NanoId(text);
            RuleFor(x => x.Title).Title(text);
            RuleFor(x => x.Content).Html(text);

            RuleFor(x => x.Type)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(text.Original("Type is required for special messages"))
                .Must(x =>
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
                })
                .WithMessage(text.Original("Type must be a valid special message type"));

            RuleFor(x => x)
                .Must(x => x.StartDate <= x.EndDate)
                .WithMessage(text.Original("Start date cannot be later than end date"));

            RuleFor(x => x).SlugMustBeUnique(text, dbContextFactory);
        }
    }
}
