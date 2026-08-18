using Pds.Core.AzureStorage.Models;
using System.Threading.Tasks;

namespace Pds.Shared.EmailProcessor.Services.Interfaces
{
    /// <summary>
    /// The Storage Audit service interface.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    public interface IStorageAuditService<in T>
        where T : PdsAzureTableEntity
    {
        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <param name="item">The audit entry.</param>
        /// <returns>True if Added successfully.</returns>
        Task<bool> AddEntryAsync(T item);
    }
}