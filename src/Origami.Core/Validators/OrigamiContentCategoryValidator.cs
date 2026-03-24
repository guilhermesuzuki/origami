using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiContentCategoryValidator : BaseValidator<OrigamiContentCategory>
    {
        public OrigamiContentCategoryValidator(Text text, IWebRootPath webRootPath) : base(text, webRootPath)
        {
            RuleFor(x => x.Id).Id(text);
        }
    }
}
