namespace Origami.Core.Models
{
    public interface IVersion
    {
        /// <summary>
        /// Row Timestamp (Row Version)
        /// </summary>
        byte[] Version { get; set; }
    }
}
