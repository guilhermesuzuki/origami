using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IPageRepository : IRepository<OrigamiPage>, IPublish<OrigamiPage>
    {
        /// <summary>
        /// Mark the <paramref name="page"/> as front-page, unmarking any existing one
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        Result<OrigamiPage> MarkAsFrontPage(DataOperationContext<OrigamiPage> page, bool checkPermission);

        /// <summary>
        /// Unmarks the <paramref name="page"/> as front-page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        Result<OrigamiPage> UnmarkAsFrontPage(DataOperationContext<OrigamiPage> page, bool checkPermission);
    }
}
