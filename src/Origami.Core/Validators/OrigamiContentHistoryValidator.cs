using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiContentHistoryValidator : BaseValidator<OrigamiContentHistory>
    {
        public OrigamiContentHistoryValidator(Text text, IWebRootPath webRootPath) : base(text, webRootPath)
        {
            RuleFor(x => x.Id).Id(text);
        }
    }
}
