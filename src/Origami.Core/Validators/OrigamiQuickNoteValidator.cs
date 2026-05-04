using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class OrigamiQuickNoteValidator : AbstractValidator<OrigamiQuickNote>
    {
        public OrigamiQuickNoteValidator(Text text, IWebRootPath webRootPath, IDbContextFactory<OrigamiDbContext> dbContextFactory) : base()
        {
            RuleFor(x => x.AuthorId).AuthorId(text);
            RuleFor(x => x.BlogId).BlogId(text);
            RuleFor(x => x.Id).Id(text);
            RuleFor(x => x.NanoId).NanoId(text);
            RuleFor(x => x.Title).Title(text);
            RuleFor(x => x).SlugMustBeUnique(text, dbContextFactory);
        }
    }
}
