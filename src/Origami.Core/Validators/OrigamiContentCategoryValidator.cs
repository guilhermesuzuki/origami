using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiContentCategoryValidator : AbstractValidator<OrigamiContentCategory>
    {
        public OrigamiContentCategoryValidator(Text text, IWebRootPath webRootPath, IDbContextFactory<OrigamiDbContext> dbContextFactory) : base()
        {
            RuleFor(x => x.Id).Id(text);
            //RuleFor(x => x.ContentId).ContentMustExist(text, dbContextFactory);
            RuleFor(x => x.CategoryId).CategoryMustExist(text, dbContextFactory);
        }
    }
}
