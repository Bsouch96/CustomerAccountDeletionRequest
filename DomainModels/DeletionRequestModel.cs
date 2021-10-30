using CustomerAccountDeletionRequest.Enums;
using System.ComponentModel.DataAnnotations;

namespace CustomerAccountDeletionRequest.DomainModels
{
    public class DeletionRequestModel
    {
        [Key]
        public int CustomerID { get; set; }

        public string DeletionReason { get; set; }

        public DeletionRequestStatusEnum DeletionRequestStatus { get; set; }
    }
}
