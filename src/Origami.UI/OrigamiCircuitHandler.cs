using Microsoft.AspNetCore.Components.Server.Circuits;
using Origami.Core.Models;

namespace Origami.UI
{
    public class OrigamiCircuitHandler : CircuitHandler
    {
        protected readonly IAppFacade _appFacade;

        public OrigamiCircuitHandler(IAppFacade appFacade) : base()
        {
            _appFacade = appFacade;
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
