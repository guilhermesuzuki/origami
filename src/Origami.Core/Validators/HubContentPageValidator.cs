using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class HubContentPageValidator : AbstractValidator<HubContentPage>
    {
        public HubContentPageValidator(Text text, IWebRootPath webRootPath, IDbContextFactory<OrigamiDbContext> dbContextFactory) : base()
        {
            RuleFor(x => x.Entity).SetValidator(new OrigamiContentValidator(text, webRootPath, dbContextFactory));
            // TODO: add this to resx files
            RuleFor(x => x.Categories).Empty().WithMessage(text.Original("Categories must be empty"));
            // TODO: add this to resx files
            RuleFor(x => x.Tags).Empty().WithMessage(text.Original("Tags must be empty"));
        }
    }
}
