using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ISettingsRepository : IRepository<OrigamiSettings>
    {
        /// <summary>
        /// Reads all settings 
        /// </summary>
        /// <returns></returns>
        OrigamiSettings GetSettings();
    }
}
