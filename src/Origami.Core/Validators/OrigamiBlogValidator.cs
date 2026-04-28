using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiBlogValidator : AbstractValidator<OrigamiBlog>
    {
        public OrigamiBlogValidator(Text text, IWebRootPath webRootPath, IDbContextFactory<OrigamiDbContext> dbContextFactory) : base()
        {
            RuleFor(x => x.Id).Id(text);
            RuleFor(x => x.NanoId).NanoId(text);
            RuleFor(x => x.Name).Name(text);
            RuleFor(x => x.Slug).Slug(text);
            RuleFor(x => x).SlugMustBeUnique(text, dbContextFactory);
        }
    }
}
