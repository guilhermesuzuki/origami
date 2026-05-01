using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class HubContentSpecialPageValidator : AbstractValidator<HubContentSpecialPage>
    {
        public HubContentSpecialPageValidator(Text text, IWebRootPath webRootPath, IDbContextFactory<OrigamiDbContext> dbContextFactory) : base()
        {
            RuleFor(x => x.Entity).SetValidator(new OrigamiContentValidator(text, webRootPath, dbContextFactory, isBlogIdRequired: false));
        }
    }
}
