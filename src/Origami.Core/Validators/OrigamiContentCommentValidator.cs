using FluentValidation;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiContentCommentValidator : AbstractValidator<OrigamiContentComment>
    {
        public OrigamiContentCommentValidator(Text text, IWebRootPath webRootPath) : base()
        {
            RuleFor(x => x.Id).Id(text);
            RuleFor(x => x.Content).Cascade(CascadeMode.Stop).Html(text).HtmlInjection(text);
            RuleFor(x => x).ParentId(text);
        }
    }
}
