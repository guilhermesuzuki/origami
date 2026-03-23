using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    /// <summary>
    /// C[R]UD operation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRead<T> where T : IId
    {
        /// <summary>
        /// Blazor requires a Database Context Factory for Queries and CRUD in general
        /// </summary>
        IDbContextFactory<OrigamiDbContext> DbContextFactory { get; }

        /// <summary>
        /// Retrieves an entity from the database that matches the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to retrieve. Cannot be null.</param>
        /// <returns>The entity of type T that matches the specified identifier, or null if no matching entity is found.</returns>
        T? ReadFromDatabase(IId id);
    }
}
