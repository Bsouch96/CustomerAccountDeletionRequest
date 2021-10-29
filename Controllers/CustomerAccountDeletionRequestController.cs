using AutoMapper;
using CustomerAccountDeletionRequest.DomainModels;
using CustomerAccountDeletionRequest.DTOs;
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
        private IMapper _mapper;
        public CustomerAccountDeletionRequestController(ICustomerAccountDeletionRequestRepository customerAccountDeletionRequestRepository,
            IMapper mapper)
        {
            _customerAccountDeletionRequestRepository = customerAccountDeletionRequestRepository;
            mapper = mapper;
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
        public ActionResult<DeletionRequestCreateDTO> GetDeletionRequest(int ID)
        {
            var deletionRequestModel = _customerAccountDeletionRequestRepository.GetDeletionRequest(ID);

            return _mapper.Map<DeletionRequestCreateDTO>(deletionRequestModel);
        }
    }
}
