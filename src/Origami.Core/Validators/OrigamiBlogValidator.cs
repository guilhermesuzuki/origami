using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiBlogValidator : BaseValidator<OrigamiBlog>
    {
        public OrigamiBlogValidator(Text text, IWebRootPath webRootPath) : base(text, webRootPath)
        {
            RuleFor(x => x.Id).Id(text);
            RuleFor(x => x.NanoId).NanoId(text);
            RuleFor(x => x.Name).Name(text);
        }
    }
}
