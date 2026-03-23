using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiContentTagValidator : BaseValidator<OrigamiContentTag>
    {
        public OrigamiContentTagValidator(Text text, IWebRootPath webRootPath) : base(text, webRootPath)
        {
            RuleFor(x => x.Id).Id(text);
            RuleFor(x => x.Slug).Slug(text);
            RuleFor(x => x.Tag).Tag(text);
        }
    }
}
