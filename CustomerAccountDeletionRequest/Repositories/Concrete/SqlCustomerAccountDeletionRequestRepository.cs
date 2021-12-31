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
        private Context.Context _context;

        /// <summary>
        /// Constructor to setup the DB Context for the Repository.
        /// </summary>
        /// <param name="customerAccountDeletionContext"></param>
        public SqlCustomerAccountDeletionRequestRepository(Context.Context customerAccountDeletionContext)
        {
            _context = customerAccountDeletionContext;
        }

        /// <summary>
        /// Asynchronously get all Deletion Request Models that are awaiting a decision on their status from the database.
        /// </summary>
        /// <returns>A list of all Deletion Request Models that are awaiting a decision on their status.</returns>
        public async Task<List<DeletionRequestModel>> GetAllAwaitingDeletionRequestsAsync()
        {
            return await _context._deletionRequestContext.AsNoTracking().Where(dr => dr.DeletionRequestStatus == Enums.DeletionRequestStatusEnum.AwaitingDecision).ToListAsync();
        }

        /// <summary>
        /// Asynchronously get the specified Deletion Request Model from the database based on the passed in CustomerID.
        /// </summary>
        /// <param name="ID">Represents the Customer ID</param>
        /// <returns>A DeletionRequestModel containing data relating to the passedin Customer ID</returns>
        public async Task<DeletionRequestModel> GetDeletionRequestAsync(int ID)
        {
            if (ID < 1)
                throw new ArgumentOutOfRangeException(nameof(ID), "IDs cannot be less than 1.");

            return await _context._deletionRequestContext.FirstOrDefaultAsync(d => d.CustomerID == ID) ?? throw new ResourceNotFoundException("A resource for ID: " + ID + " does not exist.");
        }

        /// <summary>
        /// Configures Entity Framework to begin tracking the created Deletion Request Model.
        /// This addition will be written to the database when SaveChangesAsync() is called.
        /// </summary>
        /// <param name="deletionRequestModel">The DeletionRequestModel to be created in the database.</param>
        /// <returns></returns>
        public DeletionRequestModel CreateDeletionRequest(DeletionRequestModel deletionRequestModel)
        {
            if(deletionRequestModel == null)
                throw new ArgumentNullException(nameof(deletionRequestModel), "The deletion request to be created cannot be null.");

            return _context.Add(deletionRequestModel).Entity;
        }

        /// <summary>
        /// Configures Entity Framework to begin tracking all changes to the Deletion Request Model.
        /// Changes will be written to the database when SaveChangesAsync() is called.
        /// </summary>
        /// <param name="deletionRequestModel">The Deletion Request Model that is to be updated in the database.</param>
        public void UpdateDeletionRequest(DeletionRequestModel deletionRequestModel)
        {
            if (deletionRequestModel == null)
                throw new ArgumentNullException(nameof(deletionRequestModel), "The deletion request to be updated cannot be null.");
            _context._deletionRequestContext.Update(deletionRequestModel);
        }

        /// <summary>
        /// Asynchronously save/push all additions and updates to the Database.
        /// </summary>
        /// <returns>A completed Task.</returns>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
