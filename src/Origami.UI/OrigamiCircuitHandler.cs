using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Http;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.UI
{
    public class OrigamiCircuitHandler : CircuitHandler
    {
        protected readonly IAppFacade _appFacade;
        protected readonly IMyMemoryCache _myMemoryCache;

        public OrigamiCircuitHandler(IAppFacade appFacade, IMyMemoryCache myMemoryCache) : base()
        {
            _appFacade = appFacade;
            _myMemoryCache = myMemoryCache;
        }

        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _appFacade.OnlineUsers.Add(circuit.Id);
            return Task.CompletedTask;
        }

        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _appFacade.OnlineUsers.Remove(circuit.Id);
            return Task.CompletedTask;
        }
    }
}
