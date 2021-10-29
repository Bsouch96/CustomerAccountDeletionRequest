using CustomerAccountDeletionRequest.DomainModels;
using CustomerAccountDeletionRequest.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAccountDeletionRequest.Repositories.Concrete
{
    public class FakeCustomerAccountDeletionRequestRepository : ICustomerAccountDeletionRequestRepository
    {
        public FakeCustomerAccountDeletionRequestRepository()
        {

        }

        public IEnumerable<DeletionRequestModel> GetAllDeletionRequests()
        {
            throw new NotImplementedException();
        }

        public DeletionRequestModel GetDeletionRequest(int ID)
        {
            throw new NotImplementedException();
        }
    }
}