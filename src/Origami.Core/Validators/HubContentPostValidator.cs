using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Data;
using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Validators
{
    public class HubContentPostValidator : AbstractValidator<HubContentPost>
    {
        public HubContentPostValidator(Text text, IWebRootPath webRootPath, IDbContextFactory<OrigamiDbContext> dbContextFactory) : base()
        {
            RuleFor(x => x.Entity).SetValidator(new OrigamiContentValidator(text, webRootPath, dbContextFactory));
            RuleFor(x => x.Categories).CategoriesMustBeUnique(text);
            RuleFor(x => x.Tags).TagsMustBeUnique(text);
            RuleForEach(x => x.Categories).SetValidator(new OrigamiContentCategoryValidator(text, webRootPath, dbContextFactory));
            RuleForEach(x => x.Tags).SetValidator(new OrigamiContentTagValidator(text, webRootPath, dbContextFactory));
        }
    }
}
