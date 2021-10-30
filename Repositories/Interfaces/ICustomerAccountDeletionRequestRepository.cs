using CustomerAccountDeletionRequest.DomainModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerAccountDeletionRequest.Repositories.Interfaces
{
   public interface ICustomerAccountDeletionRequestRepository
   {
        public Task<IEnumerable<DeletionRequestModel>> GetAllDeletionRequestsAsync();
        public Task<DeletionRequestModel> GetDeletionRequestAsync(int ID);
        public Task CreateDeletionRequestAsync(DeletionRequestModel deletionRequestModel);
        public Task UpdateDeletionRequestAsync(DeletionRequestModel deletionRequestModel);
        public Task SaveChangesAsync();
   }
}
