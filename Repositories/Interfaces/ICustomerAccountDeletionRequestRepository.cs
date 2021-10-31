using CustomerAccountDeletionRequest.DomainModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerAccountDeletionRequest.Repositories.Interfaces
{
   public interface ICustomerAccountDeletionRequestRepository
   {
        public Task<IEnumerable<DeletionRequestModel>> GetAllDeletionRequestsAsync();
        public Task<DeletionRequestModel> GetDeletionRequestAsync(int ID);
        public void CreateDeletionRequest(DeletionRequestModel deletionRequestModel);
        public void UpdateDeletionRequest(DeletionRequestModel deletionRequestModel);
        public Task SaveChangesAsync();
   }
}
