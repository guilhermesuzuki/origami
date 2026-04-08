using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Data;
using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Validators
{
    public class HubContentQuickNoteValidator : AbstractValidator<HubContentQuickNote>
    {
        public HubContentQuickNoteValidator(Text text, IWebRootPath webRootPath, IDbContextFactory<OrigamiDbContext> dbContextFactory) : base()
        {
            RuleFor(x => x.Entity).SetValidator(new OrigamiContentValidator(text, webRootPath, dbContextFactory, isBlogIdRequired: false));
        }
    }
}
