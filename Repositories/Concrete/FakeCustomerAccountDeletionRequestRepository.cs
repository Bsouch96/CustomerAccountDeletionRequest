using CustomerAccountDeletionRequest.DomainModels;
using CustomerAccountDeletionRequest.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAccountDeletionRequest.Repositories.Concrete
{
    public class FakeCustomerAccountDeletionRequestRepository : ICustomerAccountDeletionRequestRepository
    {
        private List<DeletionRequestModel> _deletionRequests;

        public FakeCustomerAccountDeletionRequestRepository()
        {
            _deletionRequests = new List<DeletionRequestModel>
            {
                new DeletionRequestModel { DeletionRequestID = 1, CustomerID = 1, DeletionReason = "Terrible Site.", DateRequested = new System.DateTime(2010, 10, 01, 8, 5, 3), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 2, DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision },
                new DeletionRequestModel { DeletionRequestID = 2, CustomerID = 2, DeletionReason = "Prefer Amazon.", DateRequested = new System.DateTime(2012, 01, 02, 10, 3, 45), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 2, DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision },
                new DeletionRequestModel { DeletionRequestID = 3, CustomerID = 3, DeletionReason = "Too many clicks.", DateRequested = new System.DateTime(2013, 02, 03, 12, 2, 40), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 2, DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision },
                new DeletionRequestModel { DeletionRequestID = 4, CustomerID = 4, DeletionReason = "Scammed into signing up.", DateRequested = new System.DateTime(2014, 03, 04, 14, 1, 35), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 2, DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision },
                new DeletionRequestModel { DeletionRequestID = 5, CustomerID = 5, DeletionReason = "If Wish was built by students...", DateRequested = new System.DateTime(2007, 04, 05, 16, 50, 30), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 2, DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision }
            };
        }

        public async Task<List<DeletionRequestModel>> GetAllDeletionRequestsAsync()
        {
            return await Task.FromResult(_deletionRequests);
        }

        public async Task<DeletionRequestModel> GetDeletionRequestAsync(int ID)
        {
            if (ID < 1)
                return null;

            return await Task.FromResult(_deletionRequests.FirstOrDefault(d => d.CustomerID == ID));
        }

        public DeletionRequestModel CreateDeletionRequest(DeletionRequestModel deletionRequestModel)
        {
            int deletionRequestID = (_deletionRequests.Count + 1);
            deletionRequestModel.DeletionRequestID = deletionRequestID;

            _deletionRequests.Add(deletionRequestModel);

            return deletionRequestModel;
        }

        public void UpdateDeletionRequest(DeletionRequestModel deletionRequestModel)
        {
            var productReviewModelOld = _deletionRequests.FirstOrDefault(r => r.CustomerID == deletionRequestModel.CustomerID);
            _deletionRequests.Remove(productReviewModelOld);
            _deletionRequests.Add(deletionRequestModel);
        }

        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }
    }
}