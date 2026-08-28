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

        /// <summary>
        /// Returns true if the application is in maintenance mode, false otherwise.
        /// </summary>
        /// <returns></returns>
        bool GetMaintenanceMode();

        /// <summary>
        /// Returns true if the application is in safe mode, false otherwise.
        /// </summary>
        /// <returns></returns>
        bool GetSafeMode();
    }
}
