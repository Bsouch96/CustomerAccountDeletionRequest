using CustomerAccountDeletionRequest.DomainModels;
using CustomerAccountDeletionRequest.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAccountDeletionRequest.Repositories.Concrete
{
    public class FakeCustomerAccountDeletionRequestRepository : ICustomerAccountDeletionRequestRepository
    {
        private readonly List<DeletionRequestModel> _deletionRequests;

        public FakeCustomerAccountDeletionRequestRepository()
        {
            _deletionRequests = new List<DeletionRequestModel>
            {
                new DeletionRequestModel { CustomerID = 1, DeletionReason = "Terrible Site.", DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision },
                new DeletionRequestModel { CustomerID = 2, DeletionReason = "Prefer Amazon.", DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision },
                new DeletionRequestModel { CustomerID = 3, DeletionReason = "Too many clicks.", DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision },
                new DeletionRequestModel { CustomerID = 4, DeletionReason = "Scammed into signing up.", DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision },
                new DeletionRequestModel { CustomerID = 5, DeletionReason = "If Wish was built by students...", DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision }
            };
        }

        public async Task<IEnumerable<DeletionRequestModel>> GetAllDeletionRequests()
        {
            return await Task.FromResult(_deletionRequests.AsEnumerable());
        }

        public async Task<DeletionRequestModel> GetDeletionRequest(int ID)
        {
            if (ID < 1)
                return null;

            return await Task.FromResult(_deletionRequests.FirstOrDefault(d => d.CustomerID == ID));
        }

        public void CreateDeletionRequest(DeletionRequestModel deletionRequestModel)
        {
            _deletionRequests.Add(deletionRequestModel);
        }
    }
}