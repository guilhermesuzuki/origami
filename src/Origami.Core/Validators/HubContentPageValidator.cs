using FluentValidation;
using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Validators
{
    public class HubContentPageValidator : AbstractValidator<HubContentPage>
    {
        public HubContentPageValidator(Text text, IWebRootPath webRootPath) : base()
        {
            RuleFor(x => x.Entity).SetValidator(new OrigamiContentValidator(text, webRootPath));
        }
    }
}
