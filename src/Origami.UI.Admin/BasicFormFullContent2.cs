using Microsoft.AspNetCore.Components;
using Origami.Core.Data;
using Origami.Core.Models;
using System.Transactions;

namespace Origami.UI.Admin
{
    public class BasicFormFullContent2<T1, T2> : BasicForm<T2>
        where T1 : OrigamiContent, new()
        where T2 : class, IHubContent<T1>, new()
    {
        [Inject] public IRepository<T1> Repository { get; set; } = null!;
        [Inject] public IHubContentRepository<T2> HubContentRepository { get; set; } = null!;

        public override void Save()
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    var hub = HubContentRepository.Save(Entity, UserFacade.User);
                    if (hub.Ok)
                    {
                        transaction.Complete();
                    }
                    this.UserFacade.Result = hub;
                    this.Saved.InvokeAsync(Entity).Wait();
                }
            }
            catch (Exception ex)
            {
                UserFacade.Result = new(ex);
            }
        }

        public override void UndoChanges()
        {
            this.Entity = HubContentRepository.Get(Entity.Entity);
        }
    }
}
