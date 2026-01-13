using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ISettingRepository : IRepository<OrigamiSetting>
    {
        /// <summary>
        /// Updates the setting with the given key to the new value.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Result<OrigamiSetting> UpdateOnlyThisSetting(DataOperationContext ctx, string key, string value);

        /// <summary>
        /// Updates the setting provided in the context.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        Result<OrigamiSetting> UpdateOnlyThisSetting(DataOperationContext<OrigamiSetting> ctx);
    }
}
