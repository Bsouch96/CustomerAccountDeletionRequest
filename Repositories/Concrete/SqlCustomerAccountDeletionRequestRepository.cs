using CustomerAccountDeletionRequest.Context;
using CustomerAccountDeletionRequest.DomainModels;
using CustomerAccountDeletionRequest.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public async Task<IEnumerable<DeletionRequestModel>> GetAllDeletionRequestsAsync()
        {
            return await _customerAccountDeletionRequestContext.DeletionRequestContext.ToListAsync();
        }

        public async Task<DeletionRequestModel> GetDeletionRequestAsync(int ID)
        {
            return await _customerAccountDeletionRequestContext.DeletionRequestContext.FirstOrDefaultAsync(d => d.CustomerID == ID);
        }

        public int CreateDeletionRequest(DeletionRequestModel deletionRequestModel)
        {
            return _customerAccountDeletionRequestContext.Add(deletionRequestModel).Entity.DeletionRequestID;
        }

        public void UpdateDeletionRequest(DeletionRequestModel deletionRequestModel)
        {
            //EF tracks the changes of updates. It pushes them to the DB when SaveChangesAsync() has been called.
        }

        public async Task SaveChangesAsync()
        {
            await _customerAccountDeletionRequestContext.SaveChangesAsync();
        }
    }
}
