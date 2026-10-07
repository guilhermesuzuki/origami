using Microsoft.AspNetCore.Components.Server.Circuits;
using Origami.Core.Models;

namespace Origami.UI
{
    public class OrigamiCircuitHandler(IAppFacade appFacade) : CircuitHandler
    {
        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            appFacade.OnlineUsers.Add(circuit.Id);
            return Task.CompletedTask;
        }

        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            appFacade.OnlineUsers.Remove(circuit.Id);
            return Task.CompletedTask;
        }
    }
}
