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
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public OrigamiCircuitHandler(IAppFacade appFacade, IMyMemoryCache myMemoryCache, IHttpContextAccessor httpContextAccessor) : base()
        {
            _appFacade = appFacade;
            _myMemoryCache = myMemoryCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _appFacade.OnlineUsers.Add(circuit.Id);
            return Task.CompletedTask;
        }

        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _appFacade.OnlineUsers.Remove(circuit.Id);
            _myMemoryCache.Remove($"Origami_UserLocation_{circuit.Id}");
            _myMemoryCache.Remove($"Origami_UserLocation_{_httpContextAccessor.HttpContext?.Connection.Id}");
            return Task.CompletedTask;
        }
    }
}
