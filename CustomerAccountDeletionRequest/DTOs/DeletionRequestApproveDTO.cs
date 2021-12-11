using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAccountDeletionRequest.DTOs
{
    public class DeletionRequestApproveDTO
    {
        [Required]
        public int StaffID { get; set; }
    }
}
