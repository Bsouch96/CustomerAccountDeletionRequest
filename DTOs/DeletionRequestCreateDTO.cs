using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAccountDeletionRequest.DTOs
{
    public class DeletionRequestCreateDTO
    {
        public string CustomerName { get; set; }
        public string DeletionReason { get; set; }
    }
}
