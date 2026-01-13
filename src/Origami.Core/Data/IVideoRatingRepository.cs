using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IVideoRatingRepository :
        IBaseRepository<OrigamiVideoRating>,
        IRead<OrigamiVideoRating>,
        ICache<OrigamiVideoRating>,
        ISmartPurge<OrigamiVideoRating>,
        IMerge<OrigamiVideoRating>,
        ISearch<OrigamiVideoRating>,
        IRatings<OrigamiVideo, OrigamiVideoRating>
    {

    }
}
