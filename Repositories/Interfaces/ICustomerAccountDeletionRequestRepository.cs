using CustomerAccountDeletionRequest.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAccountDeletionRequest.Repositories.Interfaces
{
   public interface ICustomerAccountDeletionRequestRepository
   {
        public IEnumerable<DeletionRequestModel> GetAllDeletionRequests();
        public DeletionRequestModel GetDeletionRequest(int ID);
   }
}
