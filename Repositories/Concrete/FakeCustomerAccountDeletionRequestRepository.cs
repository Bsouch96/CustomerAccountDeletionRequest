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
        private readonly DeletionRequestModel[] _deletionRequests =
        {
            new DeletionRequestModel { CustomerID = 1, CustomerName = "Ben Souch", DeletionReason = "Terrible Site", DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision },
            new DeletionRequestModel { CustomerID = 2, CustomerName = "Jacob Jardine", DeletionReason = "Terrible Site", DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision },
            new DeletionRequestModel { CustomerID = 3, CustomerName = "Cristian Tudor", DeletionReason = "Terrible Site", DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision },
            new DeletionRequestModel { CustomerID = 4, CustomerName = "Joseph Stavely", DeletionReason = "Terrible Site", DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision },
            new DeletionRequestModel { CustomerID = 5, CustomerName = "Teddy Teasdale", DeletionReason = "Terrible Site", DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision }
        };

        public FakeCustomerAccountDeletionRequestRepository()
        {

        }

        public Task<IEnumerable<DeletionRequestModel>> GetAllDeletionRequests()
        {
            return Task.FromResult(_deletionRequests.AsEnumerable());
        }

        public Task<DeletionRequestModel> GetDeletionRequest(int ID)
        {
            if (ID < 1)
                return null;

            DeletionRequestModel deletionRequestModel = _deletionRequests.FirstOrDefault(d => d.CustomerID == ID);
            return Task.FromResult(deletionRequestModel);
        }
    }
}