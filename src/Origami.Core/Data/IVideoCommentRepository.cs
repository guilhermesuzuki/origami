using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IVideoCommentRepository :
        IRepository<OrigamiVideoComment>,
        IComments<OrigamiVideo, OrigamiVideoComment>,
        IFastRead<VideoCommentTotal>
    {

    }
}
