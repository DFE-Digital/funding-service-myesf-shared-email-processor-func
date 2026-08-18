using Newtonsoft.Json;
using Pds.Core.AzureStorage.Interfaces;
using Pds.Core.AzureStorage.Models;
using Pds.Core.Logging;
using Pds.Shared.EmailProcessor.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pds.Shared.EmailProcessor.Services.Implementations
{
    /// <inheritdoc/>
    public class AzureTableStorageDbAuditService<T> : IStorageAuditService<T>
        where T : PdsAzureTableEntity, new()
    {
        private readonly IAzureTableStorageRepository<T> _tableStorageRepository;
        private readonly ILoggerAdapter<AzureTableStorageDbAuditService<T>> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTableStorageDbAuditService{T}"/> class.
        /// </summary>
        /// <param name="tableStorageRepository">The table storage repository c.</param>
        /// <param name="logger">The logger.</param>
        public AzureTableStorageDbAuditService(
            IAzureTableStorageRepository<T> tableStorageRepository,
            ILoggerAdapter<AzureTableStorageDbAuditService<T>> logger)
        {
            _tableStorageRepository = tableStorageRepository;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<bool> AddEntryAsync(T item)
        {
            try
            {
                await _tableStorageRepository.Insert(new List<T> { item });
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError($"AzureTableStorageDbAuditService AddEntryAsync() failed for {JsonConvert.SerializeObject(item)}", exception);

                return false;
            }
        }
    }
}