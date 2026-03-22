using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiCategoryValidator : BaseValidator<OrigamiCategory>
    {
        public OrigamiCategoryValidator(Text text, IWebRootPath webRootPath) : base(text, webRootPath)
        {
            RuleFor(x => x.BlogId).BlogId(text);
            RuleFor(x => x.Id).Id(text);
            RuleFor(x => x.NanoId).NanoId(text);
            RuleFor(x => x.Name).ShortName(text);
            RuleFor(x => x.Description).Description(text);
            RuleFor(x => x.Slug).Slug(text);
            RuleFor(x => x).ParentId(text);
        }
    }
}
