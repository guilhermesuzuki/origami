namespace Origami.Core
{
    /// <summary>
    /// file sizes
    /// </summary>
    public enum ePictureSizes : short
    {
        /// <summary>
        /// original (does not scale)
        /// </summary>
        original = 0,
        /// <summary>
        /// thumbnail (width: 50px)
        /// </summary>
        thumbnail = 50,
        /// <summary>
        /// small (width: 200px)
        /// </summary>
        small = 200,
        /// <summary>
        /// medium (width: 600px)
        /// </summary>
        medium = 600,
        /// <summary>
        /// large (width: 900px)
        /// </summary>
        large = 900,
    }
}
