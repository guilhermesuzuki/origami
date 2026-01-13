using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IPostRatingRepository :
        IBaseRepository<OrigamiPostRating>,
        IRead<OrigamiPostRating>,
        ICache<OrigamiPostRating>,
        ISmartPurge<OrigamiPostRating>,
        IMerge<OrigamiPostRating>,
        ISearch<OrigamiPostRating>,
        IRatings<OrigamiPost, OrigamiPostRating>
    {

    }
}
