namespace Origami.Core.Models
{
    /// <summary>
    /// Processed User View, class used to represent information that has been processed from user views
    /// </summary>
    public class ProcessedUserView :
        IDescription,
        ICount
    {
        public string Description { get; set; } = string.Empty;

        public int Count { get; set; } = 0;
    }
}
