using CustomerAccountDeletionRequest.DomainModels;
using CustomerAccountDeletionRequest.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAccountDeletionRequest.Controllers
{
    [ApiController]
    [Route("API/CustomerAccountDeletionRequest")]
    public class CustomerAccountDeletionRequestController : ControllerBase
    {
        private ICustomerAccountDeletionRequestRepository _customerAccountDeletionRequestRepository;
        public CustomerAccountDeletionRequestController(ICustomerAccountDeletionRequestRepository customerAccountDeletionRequestRepository)
        {
            _customerAccountDeletionRequestRepository = customerAccountDeletionRequestRepository;
        }

        /// <summary>
        /// GET all deletion requests.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<DeletionRequestModel>> GetAllDeletionRequests()
        {
            return null;
        }

        /// <summary>
        /// GET individual customer account deletion request.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet("{ID}")]
        public ActionResult<DeletionRequestModel> GetDeletionRequest(int ID)
        {
            DeletionRequestModel deletionRequestModel = _customerAccountDeletionRequestRepository.GetDeletionRequest(ID);
        }
    }
}
