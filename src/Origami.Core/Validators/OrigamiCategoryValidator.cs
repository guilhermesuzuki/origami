using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiCategoryValidator : AbstractValidator<OrigamiCategory>
    {
        public OrigamiCategoryValidator(Text text, IWebRootPath webRootPath, IDbContextFactory<OrigamiDbContext> dbContextFactory) : base()
        {
            RuleFor(x => x.BlogId).BlogId(text);
            RuleFor(x => x.Id).Id(text);
            RuleFor(x => x.NanoId).NanoId(text);
            RuleFor(x => x.Name).Name(text, 50);
            RuleFor(x => x.Description).Description(text);
            RuleFor(x => x.Slug).Cascade(CascadeMode.Stop).Slug(text, 50);
            RuleFor(x => x).ParentId(text);
            RuleFor(x => x).LoopsAreNotAllowed(text, dbContextFactory);
            RuleFor(x => x).NameMustBeUniqueByBlog(text, dbContextFactory);
            RuleFor(x => x).SlugMustBeUniqueByBlog(text, dbContextFactory);
        }
    }
}
