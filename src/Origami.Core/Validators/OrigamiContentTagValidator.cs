using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiContentTagValidator : AbstractValidator<OrigamiContentTag>
    {
        public OrigamiContentTagValidator(Text text, IWebRootPath webRootPath, IDbContextFactory<OrigamiDbContext> dbContextFactory) : base()
        {
            RuleFor(x => x.Id).Id(text);
            RuleFor(x => x.Slug).Cascade(CascadeMode.Stop).Slug(text, 128);
            RuleFor(x => x.Tag).Tag(text);
            RuleFor(x => x).SlugMustBeUniqueByContent(text, dbContextFactory);
        }
    }
}
