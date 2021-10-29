using CustomerAccountDeletionRequest.Enums;
using System.ComponentModel.DataAnnotations;

namespace CustomerAccountDeletionRequest.DomainModels
{
    public class DeletionRequestModel
    {
        [Key]
        public int CustomerID { get; set; }
        [Required]
        [MaxLength(30)]
        public string CustomerName { get; set; }
        [Required]
        [MaxLength(300)]
        public string DeletionReason { get; set; }
        public DeletionRequestStatusEnum DeletionRequestStatus { get; set; }
    }
}
