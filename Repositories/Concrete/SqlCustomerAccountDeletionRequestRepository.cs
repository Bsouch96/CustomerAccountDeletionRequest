using CustomerAccountDeletionRequest.Context;
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
        private readonly Context.Context _context;
        public SqlCustomerAccountDeletionRequestRepository(Context.Context customerAccountDeletionContext)
        {
            _context = customerAccountDeletionContext;
        }

        public async Task<List<DeletionRequestModel>> GetAllAwaitingDeletionRequestsAsync()
        {
            return await _context._deletionRequestContext.AsNoTracking().Where(dr => dr.DeletionRequestStatus == Enums.DeletionRequestStatusEnum.AwaitingDecision).ToListAsync();
        }

        public async Task<DeletionRequestModel> GetDeletionRequestAsync(int ID)
        {
            return await _context._deletionRequestContext.FirstOrDefaultAsync(d => d.CustomerID == ID);
        }

        public DeletionRequestModel CreateDeletionRequest(DeletionRequestModel deletionRequestModel)
        {
            return _context.Add(deletionRequestModel).Entity;
        }

        public void UpdateDeletionRequest(DeletionRequestModel deletionRequestModel)
        {
            _context._deletionRequestContext.Update(deletionRequestModel);
            //EF tracks the changes of updates. It pushes them to the DB when SaveChangesAsync() has been called.
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
