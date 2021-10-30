using CustomerAccountDeletionRequest.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAccountDeletionRequest.Repositories.Interfaces
{
   public interface ICustomerAccountDeletionRequestRepository
   {
        public Task<IEnumerable<DeletionRequestModel>> GetAllDeletionRequestsAsync();
        public Task<DeletionRequestModel> GetDeletionRequestAsync(int ID);
        public void CreateDeletionRequestAsync(DeletionRequestModel deletionRequestModel);
        public void SaveChanges();
   }
}
