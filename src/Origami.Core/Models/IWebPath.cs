namespace Origami.Core.Models
{
    public interface IWebPath
    {
        /// <summary>
        /// Web Path for a Resource (like a file). Example: /blogs/files/example.txt
        /// </summary>
        string WebPath { get; }
    }
}
