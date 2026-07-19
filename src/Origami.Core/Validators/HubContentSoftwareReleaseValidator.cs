using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public class HubContentSoftwareReleaseValidator : AbstractValidator<HubContentSoftwareRelease>
    {
        public HubContentSoftwareReleaseValidator(Text text, IWebRootPath webRootPath, IDbContextFactory<OrigamiDbContext> dbContextFactory, IDirectoryRepository directoryRepository) : base()
        {
            RuleFor(x => x.Entity).SetValidator(new OrigamiContentValidator(text, webRootPath, dbContextFactory));
            RuleFor(x => x.Categories).CategoriesMustBeUnique(text);
            RuleFor(x => x.Tags).TagsMustBeUnique(text);
            RuleForEach(x => x.Categories).SetValidator(new OrigamiContentCategoryValidator(text, webRootPath, dbContextFactory));
            RuleForEach(x => x.Tags).SetValidator(new OrigamiContentTagValidator(text, webRootPath, dbContextFactory));
            RuleFor(x => x.Entity.DateReleased).NotNull().WithMessage("Release date is required");
            RuleFor(x => x.Entity.Content).MustHaveHtml(text);
            RuleFor(x => x.Entity).Must(entity => 
            {
                var path = Path.Combine(directoryRepository.LocalPathForFiles(entity), "files");

                if (Directory.Exists(path) == true)
                {
                    return Directory.EnumerateFiles(path).Any();
                }

                return false;
            }).WithMessage("Release must have at least 1 file");
        }
    }
}
