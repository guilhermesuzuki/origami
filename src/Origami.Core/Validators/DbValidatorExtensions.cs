using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public static class DbValidatorExtensions
    {
        public static IRuleBuilderOptions<T, Guid> CategoryMustExist<T>(this IRuleBuilder<T, Guid> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory)
        {
            return ruleBuilder
                .Must(categoryId =>
                {
                    using var db = dbContextFactory.CreateDbContext();
                    return db.Set<OrigamiCategory>().AsNoTracking().Id(categoryId) != null;
                })
                .WithMessage(text.Original("Category must exist"));
        }

        public static IRuleBuilderOptions<T, Guid> ContentMustExist<T>(this IRuleBuilder<T, Guid> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory)
        {
            return ruleBuilder
                .Must(contentId =>
                {
                    using var db = dbContextFactory.CreateDbContext();
                    return db.Set<OrigamiContent>().AsNoTracking().Id(contentId) != null;
                })
                .WithMessage(text.Original("Content must exist"));
        }

        public static IRuleBuilderOptions<T, T> DisplayNameMustBeUnique<T>(this IRuleBuilder<T, T> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory) where T : OrigamiUser
        {
            return ruleBuilder
                .Must(entity =>
                {
                    using var db = dbContextFactory.CreateDbContext();
                    var exists = db.Set<T>().AsNoTracking().Any(x => x.DisplayName == entity.DisplayName && x.Id != entity.Id);
                    return !exists;
                })
                // TODO: add string to resx files
                .WithMessage(text.Original("Display name is already in use"));
        }

        public static IRuleBuilderOptions<T, T> EmailMustBeUnique<T>(this IRuleBuilder<T, T> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory) where T : class, IId, IEmail
        {
            return ruleBuilder
                .Must(entity =>
                {
                    using var db = dbContextFactory.CreateDbContext();
                    var exists = db.Set<T>().AsNoTracking().Any(x => x.Email == entity.Email && x.Id != entity.Id);
                    return !exists;
                })
                // TODO: add string to resx files
                .WithMessage(text.Original("E-mail is already in use"));
        }

        public static IRuleBuilderOptions<T, T> InfiniteLoopsAreNotAllowed<T>(this IRuleBuilder<T, T> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory) where T : class, IId, IParentIdNull
        {
            return ruleBuilder
                .Must(entity =>
                {
                    using var db = dbContextFactory.CreateDbContext();
                    return IsInfiniteLoopDetected(db, entity, []) == false;
                })
                .WithMessage(text.Original("Loop in relationships are not allowed"));
        }

        public static bool IsInfiniteLoopDetected<T>(DbContext db, T? entity, IList<T> list) where T : class, IId, IParentIdNull
        {
            if (entity != null)
            {
                if (entity.Id == entity.ParentId) return true;
                if (list.Id(entity.Id) != null) return true;

                list.Add(entity);

                if (entity.ParentId != null)
                {
                    var parent = db.Set<T>().AsNoTracking().Id(entity.ParentId.GetValueOrDefault());
                    return IsInfiniteLoopDetected(db, parent, list);
                }
            }
            return false;
        }

        public static IRuleBuilderOptions<T, T> NameMustBeUnique<T>(this IRuleBuilder<T, T> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory) where T : class, IId, IName
        {
            return ruleBuilder
                .Must(entity =>
                {
                    using var db = dbContextFactory.CreateDbContext();
                    var exists = db.Set<T>().AsNoTracking().Any(x => x.Name == entity.Name && x.Id != entity.Id);
                    return !exists;
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
                    var exists = db.Set<T>().AsNoTracking().Any(x => x.Name == entity.Name && x.BlogId == entity.BlogId && x.Id != entity.Id);
                    return !exists;
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
                    var exists = db.Set<T>().AsNoTracking().Any(x => x.Slug == entity.Slug && x.Id != entity.Id);
                    return !exists;
                })
                .WithMessage(text.Original("Slug is already in use"));
        }

        public static IRuleBuilderOptions<T, T> SlugMustBeUniqueByBlog<T>(this IRuleBuilder<T, T> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory) where T : class, IId, IBlogIdNull, ISlug
        {
            return ruleBuilder
                .Must(entity =>
                {
                    using var db = dbContextFactory.CreateDbContext();
                    var exists = db.Set<T>().AsNoTracking().Any(x => x.Slug == entity.Slug && x.BlogId == entity.BlogId && x.Id != entity.Id);
                    return !exists;
                })
                .WithMessage(text.Original("Slug is already in use"));
        }

        public static IRuleBuilderOptions<T, T> SlugMustBeUniqueByContent<T>(this IRuleBuilder<T, T> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory) where T : class, IId, IContentId, ISlug
        {
            return ruleBuilder
                .Must(entity =>
                {
                    using var db = dbContextFactory.CreateDbContext();
                    return !db.Set<T>().AsNoTracking().Any(x => x.Slug == entity.Slug && x.ContentId == entity.ContentId && x.Id != entity.Id);
                })
                .WithMessage(text.Original("Slug is already in use"));
        }

        public static IRuleBuilderOptions<T, T> TitleMustBeUnique<T>(this IRuleBuilder<T, T> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory) where T : class, IId, ITitle
        {
            return ruleBuilder
                .Must(entity =>
                {
                    using var db = dbContextFactory.CreateDbContext();
                    var exists = db.Set<T>().AsNoTracking().Any(x => x.Title == entity.Title && x.Id != entity.Id);
                    return !exists;
                })
                .WithMessage(text.Original("Title is already in use"));
        }

        public static IRuleBuilderOptions<T, T> TitleMustBeUniqueByBlog<T>(this IRuleBuilder<T, T> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory) where T : class, IId, IBlogIdNull, ITitle
        {
            return ruleBuilder
                .Must(entity =>
                {
                    using var db = dbContextFactory.CreateDbContext();
                    var exists = db.Set<T>().AsNoTracking().Any(x => x.Title == entity.Title && x.BlogId == entity.BlogId && x.Id != entity.Id);
                    return !exists;
                })
                .WithMessage(text.Original("Title is already in use"));
        }

        public static IRuleBuilderOptions<T, T> UsernameMustBeUnique<T>(this IRuleBuilder<T, T> ruleBuilder, Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory) where T : OrigamiUser
        {
            return ruleBuilder
                .Must(entity =>
                {
                    using var db = dbContextFactory.CreateDbContext();
                    var exists = db.Set<T>().AsNoTracking().Any(x => x.Username == entity.Username && x.Id != entity.Id);
                    return !exists;
                })
                // TODO: add string to resx files
                .WithMessage(text.Original("Username is already in use"));
        }
    }
}
