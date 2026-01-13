namespace Origami.Core.Models
{
    public interface IHas
    {
        /// <summary>
        /// Determines whether the Entity is ready to be saved (contains all required fields filled in)
        /// </summary>
        /// <returns></returns>
        bool Has();
    }
}
