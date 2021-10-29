using CustomerAccountDeletionRequest.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAccountDeletionRequest.Repositories.Interfaces
{
   public interface ICustomerAccountDeletionRequestRepository
   {
        public Task<IEnumerable<DeletionRequestModel>> GetAllDeletionRequests();
        public Task<DeletionRequestModel> GetDeletionRequest(int ID);
   }
}
