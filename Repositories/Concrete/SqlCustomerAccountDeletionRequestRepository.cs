using CustomerAccountDeletionRequest.Context;
using CustomerAccountDeletionRequest.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAccountDeletionRequest.Repositories.Concrete
{
    public class SqlCustomerAccountDeletionRequestRepository
    {
        private readonly CustomerAccountDeletionRequestContext _customerAccountDeletionRequestContext;
        public SqlCustomerAccountDeletionRequestRepository(CustomerAccountDeletionRequestContext
            customerAccountDeletionContext)
        {
            _customerAccountDeletionRequestContext = customerAccountDeletionContext;
        }

        public IEnumerable<DeletionRequestModel> GetAllDeletionRequests()
        {
            return _customerAccountDeletionRequestContext.DeletionRequestContext.ToList();
        }

        public DeletionRequestModel GetDeletionRequest(int ID)
        {
            return _customerAccountDeletionRequestContext.DeletionRequestContext
                .FirstOrDefault(d => d.CustomerID == ID);
        }
    }
}
