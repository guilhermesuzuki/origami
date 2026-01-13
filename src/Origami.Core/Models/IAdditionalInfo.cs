namespace Origami.Core.Models
{
    public interface IAdditionalInfo
    {
        /// <summary>
        /// XML for Additional Information, like retrieving/adding new properties (without having to change the table)
        /// </summary>
        string? AdditionalInfo { get; set; }
    }

    public interface IAdditionalInfo<T> where T : AdditionalInfo
    {
        /// <summary>
        /// Converts the XML into <typeparamref name="T"/>
        /// </summary>
        T Get();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        T Set(Action<T> action);
    }
}
