using CustomerAccountDeletionRequest.DomainModels;
using CustomerAccountDeletionRequest.Models;
using CustomerAccountDeletionRequest.Repositories.Interfaces;
using Invoices.Helpers.Interface;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Invoices.Helpers.Concrete
{
    public class MemoryCacheAutomater : IMemoryCacheAutomater
    {
        private readonly ICustomerAccountDeletionRequestRepository _customerAccountDeletionRequestRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheModel _memoryCacheModel;

        public MemoryCacheAutomater(IServiceScopeFactory serviceProvider, IMemoryCache memoryCache,
            IOptions<MemoryCacheModel> memoryCacheModel)
        {
            _customerAccountDeletionRequestRepository = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ICustomerAccountDeletionRequestRepository>();
            _memoryCache = memoryCache;
            _memoryCacheModel = memoryCacheModel.Value;
        }

        public void AutomateCache()
        {
            RegisterCache(_memoryCacheModel.CustomerAccountDeletionRequests, null, EvictionReason.None, null);
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
