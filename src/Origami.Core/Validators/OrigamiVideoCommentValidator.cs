using FluentValidation;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiVideoCommentValidator : BaseValidator<OrigamiVideoComment>
    {
        public OrigamiVideoCommentValidator(Text text, IWebRootPath webRootPath) : base(text, webRootPath)
        {
            RuleFor(x => x.Id).Id(text);
            RuleFor(x => x.Content).Cascade(CascadeMode.Stop).Html(text).HtmlInjection(text);
            RuleFor(x => x).ParentId(text);
        }
    }
}
