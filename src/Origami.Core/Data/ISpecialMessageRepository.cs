using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ISpecialMessageRepository : IPublish<OrigamiSpecialMessage>
    {
        /// <summary>
        /// Gets all the published site messages, the ones within the date range.
        /// </summary>
        /// <returns></returns>
        IEnumerable<OrigamiSpecialMessage> GetVisibleMessages();
    }
}
