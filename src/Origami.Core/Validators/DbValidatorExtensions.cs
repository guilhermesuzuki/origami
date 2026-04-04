using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.Core.Validators
{
    public static class DbValidatorExtensions
    {
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
    }
}
