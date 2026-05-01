using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public static class DbValidatorExtensions
    {
        public static IRuleBuilderOptions<T, Guid> Category<T>(this IRuleBuilder<T, Guid> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory)
        {
            return ruleBuilder
                .Must(categoryId =>
                {
                    using var db = dbContextFactory.CreateDbContext();
                    return db.Set<OrigamiCategory>().AsNoTracking().Id(categoryId) != null;
                })
                .WithMessage(text.Original("Category must exist"));
        }

        public static IRuleBuilderOptions<T, Guid> Content<T>(this IRuleBuilder<T, Guid> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory)
        {
            return ruleBuilder
                .Must(contentId =>
                {
                    using var db = dbContextFactory.CreateDbContext();
                    return db.Set<OrigamiContent>().AsNoTracking().Id(contentId) != null;
                })
                .WithMessage(text.Original("Content must exist"));
        }

        public static bool IsCycleDetected<T>(DbContext db, T? entity, IList<T> list) where T : class, IId, IParentIdNull
        {
            if (entity != null)
            {
                if (entity.Id == entity.ParentId) return true;
                if (list.Id(entity.Id) != null) return true;

                list.Add(entity);

                if (entity.ParentId != null)
                {
                    var parent = db.Set<T>().AsNoTracking().Id(entity.ParentId.GetValueOrDefault());
                    return IsCycleDetected(db, parent, list);
                }
            }
            return false;
        }

        public static IRuleBuilderOptions<T, T> LoopsAreNotAllowed<T>(this IRuleBuilder<T, T> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory) where T : class, IId, IParentIdNull
        {
            return ruleBuilder
                .Must(entity =>
                {
                    using var db = dbContextFactory.CreateDbContext();
                    return IsCycleDetected(db, entity, []) == false;
                })
                .WithMessage(text.Original("Loop in relationships are not allowed"));
        }

        public static IRuleBuilderOptions<T, T> NameMustBeUnique<T>(this IRuleBuilder<T, T> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory) where T : class, IId, IName
        {
            return ruleBuilder
                .Must(entity =>
                {
                    using var db = dbContextFactory.CreateDbContext();
                    var slug = db.Set<T>().AsNoTracking().FirstOrDefault(x => x.Name == entity.Name);
                    return slug == null || slug.Id == entity.Id;
                })
                // TODO: add string to resx files
                .WithMessage(text.Original("Name is already in use"));
        }

        public static IRuleBuilderOptions<T, T> NameMustBeUniqueByBlog<T>(this IRuleBuilder<T, T> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory) where T : class, IId, IBlogIdNull, IName
        {
            return ruleBuilder
                .Must(entity =>
                {
                    using var db = dbContextFactory.CreateDbContext();
                    var slug = db.Set<T>().AsNoTracking().FirstOrDefault(x => x.Name == entity.Name && x.BlogId == entity.BlogId);
                    return slug == null || slug.Id == entity.Id;
                })
                // TODO: add string to resx files
                .WithMessage(text.Original("Name is already in use"));
        }

        public static IRuleBuilderOptions<T, T> SlugMustBeUnique<T>(this IRuleBuilder<T, T> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory) where T : class, IId, ISlug
        {
            return ruleBuilder
                .Must(entity =>
                {
                    using var db = dbContextFactory.CreateDbContext();
                    var slug = db.Set<T>().AsNoTracking().FirstOrDefault(x => x.Slug == entity.Slug);
                    return slug == null || slug.Id == entity.Id;
                })
                .WithMessage(text.Original("Slug is already in use"));
        }

        public static IRuleBuilderOptions<T, T> SlugMustBeUniqueByBlog<T>(this IRuleBuilder<T, T> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory) where T : class, IId, IBlogIdNull, ISlug
        {
            return ruleBuilder
                .Must(entity =>
                {
                    using var db = dbContextFactory.CreateDbContext();
                    var slug = db.Set<T>().AsNoTracking().FirstOrDefault(x => x.Slug == entity.Slug && x.BlogId == entity.BlogId);
                    return slug == null || slug.Id == entity.Id;
                })
                .WithMessage(text.Original("Slug is already in use"));
        }

        public static IRuleBuilderOptions<T, T> SlugMustBeUniqueByContent<T>(this IRuleBuilder<T, T> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory) where T : class, IId, IContentId, ISlug
        {
            return ruleBuilder
                .Must(entity =>
                {
                    using var db = dbContextFactory.CreateDbContext();
                    var slug = db.Set<T>().AsNoTracking().FirstOrDefault(x => x.Slug == entity.Slug && x.ContentId == entity.ContentId);
                    return slug == null || slug.Id == entity.Id;
                })
                .WithMessage(text.Original("Slug is already in use"));
        }

        
    }
}
