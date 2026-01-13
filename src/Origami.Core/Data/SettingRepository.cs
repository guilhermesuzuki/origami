using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class SettingRepository :
        RepositoryOuterLayer<OrigamiSetting>,
        ISettingRepository
    {
        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public SettingRepository(
            Text text,
            IMemoryCache memoryCache,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {

        }

        public Result<OrigamiSetting> UpdateOnlyThisSetting(DataOperationContext ctx, string key, string value)
        {
            try
            {
                var setting = this.ReadFromDatabase().FirstOrDefault(x => x.Name == key) ?? new OrigamiSetting() { Id = Guid.NewGuid(), Name = key };
                setting.Value = value;
                return this.UpdateOnlyThisSetting(setting.GetContext(ctx.User));
            }
            catch (Exception ex)
            {
                return new() { ErrorMessage = ex.GetMessage() };
            }
        }

        public Result<OrigamiSetting> UpdateOnlyThisSetting(DataOperationContext<OrigamiSetting> ctx)
        {
            try
            {
                return this.SmartSave(ctx, false);
            }
            catch (Exception ex)
            {
                return new() { ErrorMessage = ex.GetMessage() };
            }
        }
    }
}
