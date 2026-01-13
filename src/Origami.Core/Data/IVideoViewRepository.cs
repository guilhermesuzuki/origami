using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IVideoViewRepository :
        IRepository<OrigamiVideoView>,
        IViews<OrigamiVideo>,
        IFastRead<VideoViewTotal>
    {

    }
}
