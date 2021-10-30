using CustomerAccountDeletionRequest.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace CustomerAccountDeletionRequest.DomainModels
{
    public class DeletionRequestModel
    {
        [Key]
        public int CustomerID { get; set; }
        public string DeletionReason { get; set; }
        public DateTime DateRequested { get; set; }
        public DateTime DateApproved { get; set; }
        public int StaffID { get; set; }
        public DeletionRequestStatusEnum DeletionRequestStatus { get; set; }
    }
}
