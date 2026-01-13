using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiVideoValidator : BaseValidator<OrigamiVideo>
    {
        public OrigamiVideoValidator(Text text, IWebRootPath webRootPath) : base(text, webRootPath)
        {
            RuleFor(x => x.Id).Id(text);
            RuleFor(x => x.NanoId).NanoId(text);
            RuleFor(x => x.Title).Title(text);
            RuleFor(x => x.Description).Description(text);
            RuleFor(x => x.Slug).Slug(text);
            RuleFor(x => x.Content).Html(text);
            RuleFor(x => x.HeaderImage).HeaderImage(text, webRootPath);
            RuleFor(x => x.LanguageWrittenOn).Language(text);
            RuleFor(x => x.EmbedIFrame).IFrame(text);
        }
    }
}
