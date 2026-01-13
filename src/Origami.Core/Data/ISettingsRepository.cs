using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ISettingsRepository : IRepository<OrigamiSettings>
    {
        /// <summary>
        /// Reads all settings from <paramref name="blog"/>
        /// </summary>
        /// <param name="blog"></param>
        /// <returns></returns>
        OrigamiSettings GetSettings();
    }
}
