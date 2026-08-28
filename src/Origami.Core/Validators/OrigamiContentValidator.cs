using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiContentValidator : AbstractValidator<OrigamiContent>
    {
        public OrigamiContentValidator(Text text, IWebRootPath webRootPath, IDbContextFactory<OrigamiDbContext> dbContextFactory, bool isBlogIdRequired = true) : base()
        {
            RuleFor(x => x.Id).Id(text);
            RuleFor(x => x.Type).ContentType(text);
            RuleFor(x => x.AuthorId).AuthorId(text);
            RuleFor(x => x.NanoId).NanoId(text);
            RuleFor(x => x.Title).Title(text);
            RuleFor(x => x.Description).Description(text);
            RuleFor(x => x.Slug).Cascade(CascadeMode.Stop).Slug(text);
            RuleFor(x => x.Content).Html(text);
            RuleFor(x => x.HeaderImage).HeaderImage(text, webRootPath);
            RuleFor(x => x.LanguageWrittenOn).Language(text);
            RuleFor(x => x).TopLevelPageWhenFrontPage(text);
            RuleFor(x => x).ModificationMustHappenAfterCreation(text);
            RuleFor(x => x).InfiniteLoopsAreNotAllowed(text, dbContextFactory);

            if (isBlogIdRequired)
            {
                RuleFor(x => x.BlogId).BlogId(text);
                RuleFor(x => x).TitleMustBeUniqueByBlog(text, dbContextFactory);
                RuleFor(x => x).SlugMustBeUniqueByBlog(text, dbContextFactory);
            }
            else
            {
                RuleFor(x => x.BlogId).BlogIdMustBeNull(text);
                RuleFor(x => x).TitleMustBeUnique(text, dbContextFactory);
                RuleFor(x => x).SlugMustBeUnique(text, dbContextFactory);
            }
        }
    }
}
