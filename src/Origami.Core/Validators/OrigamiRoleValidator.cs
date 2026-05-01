using FluentValidation;
using Origami.Core.Models;
using System.Diagnostics;

namespace Origami.Core.Validators
{
    public class OrigamiRoleValidator : AbstractValidator<OrigamiRole>
    {
        public OrigamiRoleValidator(Text text, IWebRootPath webRootPath) : base()
        {
            RuleFor(x => x.IsSystemRole).Must(system =>
            {
                if (Debugger.IsAttached) return true;
                return system == false;
            }).WithMessage(text.Original("You can't modify a system role"));

            RuleFor(x => x.Id).Id(text);
            RuleFor(x => x.Name).Name(text);
        }
    }
}
