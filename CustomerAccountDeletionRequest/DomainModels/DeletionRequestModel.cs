using CustomerAccountDeletionRequest.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace CustomerAccountDeletionRequest.DomainModels
{
    public class DeletionRequestModel
    {
        [Key]
        public int DeletionRequestID { get; set; }
        [Required]
        public int CustomerID { get; set; }
        [Required]
        public string DeletionReason { get; set; }
        [Required]
        public DateTime DateRequested { get; set; }
        public DateTime DateApproved { get; set; }
        public int StaffID { get; set; }
        [Required]
        public DeletionRequestStatusEnum DeletionRequestStatus { get; set; }
    }
}
