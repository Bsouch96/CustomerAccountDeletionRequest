﻿using CustomerAccountDeletionRequest.Context;
using CustomerAccountDeletionRequest.DomainModels;
using CustomerAccountDeletionRequest.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAccountDeletionRequest.Repositories.Concrete
{
    public class SqlCustomerAccountDeletionRequestRepository : ICustomerAccountDeletionRequestRepository
    {
        private readonly CustomerAccountDeletionRequestContext _customerAccountDeletionRequestContext;
        public SqlCustomerAccountDeletionRequestRepository(CustomerAccountDeletionRequestContext
            customerAccountDeletionContext)
        {
            _customerAccountDeletionRequestContext = customerAccountDeletionContext;
        }

        public async Task<IEnumerable<DeletionRequestModel>> GetAllDeletionRequests()
        {
            return await _customerAccountDeletionRequestContext.DeletionRequestContext.ToListAsync();
        }

        public async Task<DeletionRequestModel> GetDeletionRequest(int ID)
        {
            return await _customerAccountDeletionRequestContext.DeletionRequestContext.FirstOrDefaultAsync(d => d.CustomerID == ID);
        }

        public async void CreateDeletionRequest(DeletionRequestModel deletionRequestModel)
        {
            await _customerAccountDeletionRequestContext.AddAsync(deletionRequestModel);
        }
    }
}
