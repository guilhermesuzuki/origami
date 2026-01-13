namespace Origami.Core.Models
{
    public interface ILocalPath
    {
        /// <summary>
        /// Path in the Local File System (c:\example.txt)
        /// </summary>
        string LocalPath { get; }
    }
}
