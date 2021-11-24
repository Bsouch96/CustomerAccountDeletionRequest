using CustomerAccountDeletionRequest.Context;
using CustomerAccountDeletionRequest.CustomExceptionMiddleware;
using CustomerAccountDeletionRequest.DomainModels;
using CustomerAccountDeletionRequest.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
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
            if (ID < 1)
                throw new ArgumentOutOfRangeException("IDs cannot be less than 0.", nameof(ArgumentOutOfRangeException));

            return await _context._deletionRequestContext.FirstOrDefaultAsync(d => d.CustomerID == ID) ?? throw new ResourceNotFoundException("A resource for ID: " + ID + " does not exist.");
        }

        public DeletionRequestModel CreateDeletionRequest(DeletionRequestModel deletionRequestModel)
        {
            if(deletionRequestModel == null)
                throw new ArgumentNullException("The deletion request to be created cannot be null.", nameof(ArgumentNullException));
            return _context.Add(deletionRequestModel).Entity;
        }

        public void UpdateDeletionRequest(DeletionRequestModel deletionRequestModel)
        {
            if (deletionRequestModel == null)
                throw new ArgumentNullException("The deletion request to be updated cannot be null.", nameof(ArgumentNullException));

            _context._deletionRequestContext.Update(deletionRequestModel);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
