namespace Origami.Core.Data
{
    public interface IFastRead<TFastRead>
    {
        /// <summary>
        /// Should return a list of <typeparamref name="TFastRead"/> using the fastest and simplest method possible
        /// </summary>
        /// <returns></returns>
        Task<List<TFastRead>> FastRead();

        /// <summary>
        /// TODO: comment this
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task Update(IEnumerable<TFastRead> entities);
    }
}
