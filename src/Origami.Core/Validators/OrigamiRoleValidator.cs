using FluentValidation;
using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Origami.Core.Validators
{
    public class OrigamiRoleValidator: BaseValidator<OrigamiRole>
    {
        public OrigamiRoleValidator(Text text, IWebRootPath webRootPath) : base(text, webRootPath)
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
