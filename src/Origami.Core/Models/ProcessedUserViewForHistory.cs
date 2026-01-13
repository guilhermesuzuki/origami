namespace Origami.Core.Models
{
    /// <summary>
    /// TODO: comment this
    /// Can't inherit from a class because of the way EF works
    /// </summary>
    public class ProcessedUserViewForHistory :
        IDescription,
        ICount
    {
        public string Description { get; set; } = string.Empty;

        public int Count { get; set; } = 0;

        /// <summary>
        /// Flag indicating whether the information comes from a bot (crawler) or not.
        /// </summary>
        public bool IsBot { get; set; }
    }
}
