using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IContentRatingRepository : 
        IRepository<OrigamiContentRating>,
        IRatings<OrigamiContent, OrigamiContentRating>
    {

    }
}
