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
            RuleFor(x => x.Slug).Slug(text);
            RuleFor(x => x.Tag).Tag(text);
        }
    }
}
