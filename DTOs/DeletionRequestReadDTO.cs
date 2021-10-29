using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAccountDeletionRequest.DTOs
{
    public class DeletionRequestReadDTO
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string DeletionReason { get; set; }
    }
}
