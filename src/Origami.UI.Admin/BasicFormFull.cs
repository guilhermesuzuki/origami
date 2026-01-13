using Microsoft.AspNetCore.Components;
using Octokit;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using System.Transactions;

namespace Origami.UI.Admin
{
    public class BasicFormFull<T> :
        BasicForm<T>
        where T : class, IId, INew, new()
    {
        [Inject] public IRepository<T> Repository { get; set; } = null!;

        /// <summary>
        /// Saves the entity in the database, updating cache
        /// </summary>
        public override void Save()
        {
            var beforeSaving = BeforeSaving();
            if (beforeSaving.Ok == false)
            {
                UserFacade.Result = beforeSaving;
                return;
            }

            var hub = new Result<T>(this.Entity);
            var before = Repository.ReadFromCache().Id(Entity.Id);
            var context = new DataOperationContext<T>(this.UserFacade.User, DateTime.UtcNow, Entity, before);

            try
            {
                using (var transaction = new TransactionScope())
                {
                    hub = Repository.SmartSave(context, true);
                    if (Entity.New)
                    {
                        hub.OnSuccess(() => this.CreateOthers().Push(hub));
                    }
                    else
                    {
                        hub.OnSuccess(() => this.UpdateOthers().Push(hub));
                    }
                    if (hub.Ok)
                    {
                        transaction.Complete();
                    }
                }
                Saved.InvokeAsync(hub.Entity).Wait();
            }
            catch (Exception ex)
            {
                hub.ErrorMessage = ex.GetMessage();
            }
            finally
            {
                UserFacade.Result = hub;
            }
        }

        public override void UndoChanges()
        {
            this.ParentSelector = false;
            this.Entity = Repository.ReadFromCache().Id(this.Entity.Id).Clone() ?? new();
        }

        protected override Result<T> BeforeSaving()
        {
            this.ParentSelector = false;

            //sets the FK Blog (or the save process will fail)
            if (Entity is IFKBlog fkBlog && fkBlog.BlogId == Guid.Empty)
            {
                fkBlog.BlogId = this.UserFacade.BlogId;
                if (fkBlog.BlogId == Guid.Empty) throw new InvalidOperationException("BlogId is empty");
            }

            return new(Entity);
        }

        protected virtual Result<T> CreateOthers()
        {
            return new(Entity);
        }

        /// <summary>
        /// Retrieves the parent entities of the current entity.
        /// </summary>
        /// <remarks>This method returns a collection of entities that are considered parents of the
        /// current entity. The result excludes the current entity and its descendants, as determined by the
        /// <c>EveryChildrenAndSelf</c> method.</remarks>
        /// <returns>An <see cref="IEnumerable{T}"/> containing the parent entities of the current entity.</returns>
        protected virtual IEnumerable<T> Parents()
        {
            return Repository.ReadFromCache().NonDeleted().Where(x => x.Id != Entity.Id);
        }

        protected virtual Result<T> UpdateOthers()
        {
            return new(Entity);
        }
    }
}
