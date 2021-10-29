using CustomerAccountDeletionRequest.Enums;

namespace CustomerAccountDeletionRequest.DomainModels
{
    public class DeletionRequestCreateModel
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string DeletionReason { get; set; }
        public DeletionRequestStatusEnum DeletionRequestStatus { get; set; }
    }
}
