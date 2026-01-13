using FluentValidation;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiSpecialPageValidator : BaseValidator<OrigamiSpecialPage>
    {
        public OrigamiSpecialPageValidator(Text text, IWebRootPath webRootPath) : base(text, webRootPath)
        {
            RuleFor(x => x.Id).Id(text);
            RuleFor(x => x.NanoId).NanoId(text);
            RuleFor(x => x.Title).Title(text);
            RuleFor(x => x.Description).Description(text);
            RuleFor(x => x.Slug).Slug(text);
            RuleFor(x => x.Content).Html(text);
            RuleFor(x => x.HeaderImage).HeaderImage(text, webRootPath);

            RuleFor(x => x.Type)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(text.Original("Type is required for special pages"))
                .Must(x =>
                {
                    return Enum.GetValues<OrigamiSpecialPageTypes>().Select(e => e.ToString()).ToList().Contains(x);
                })
                .WithMessage(text.Original("Type must be a valid special page type"));

            RuleFor(x => x.LanguageWrittenOn).Language(text);
        }
    }
}
