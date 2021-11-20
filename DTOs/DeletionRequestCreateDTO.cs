using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAccountDeletionRequest.DTOs
{
    public class DeletionRequestCreateDTO
    {
        [Required]
        public int CustomerID { get; set; }
        [Required]
        public string DeletionReason { get; set; }
    }
}
