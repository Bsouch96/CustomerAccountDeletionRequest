using CustomerAccountDeletionRequest.DomainModels;
using CustomerAccountDeletionRequest.Repositories.Interfaces;
using Invoices.Helpers.Interface;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Invoices.Helpers.Concrete
{
    public class MemoryCacheAutomater : IMemoryCacheAutomater
    {
        private readonly ICustomerAccountDeletionRequestRepository _customerAccountDeletionRequestRepository;
        public readonly IMemoryCache _memoryCache;

        public MemoryCacheAutomater(ICustomerAccountDeletionRequestRepository customerAccountDeletionRequestRepository, IMemoryCache memoryCache)
        {
            _customerAccountDeletionRequestRepository = customerAccountDeletionRequestRepository;
            _memoryCache = memoryCache;
        }

        public void AutomateCache()
        {
            RegisterCache("CustomerAccountDeletionRequests", null, EvictionReason.None, null);
        }

        private MemoryCacheEntryOptions GetMemoryCacheEntryOptions()
        {
            int cacheExpirationMinutes = 1;
            DateTime cacheExpirationTime = DateTime.Now.AddMinutes(cacheExpirationMinutes);
            CancellationChangeToken cacheExpirationToken = new CancellationChangeToken
            (
                new CancellationTokenSource(TimeSpan.FromMinutes(cacheExpirationMinutes + 0.01)).Token
            );

            return new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(cacheExpirationTime)
                .SetPriority(CacheItemPriority.NeverRemove)
                .AddExpirationToken(cacheExpirationToken)
                .RegisterPostEvictionCallback(callback: RegisterCache, state: this);
        }

        private async void RegisterCache(object key, object value, EvictionReason reason, object state)
        {
            List<DeletionRequestModel> deletionRequestModels = await _customerAccountDeletionRequestRepository.GetAllAwaitingDeletionRequestsAsync();
            _memoryCache.Set(key, deletionRequestModels, GetMemoryCacheEntryOptions());
        }
    }
}
