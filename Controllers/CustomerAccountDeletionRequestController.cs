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
    [Route("api/CustomerAccountDeletionRequest")]
    public class CustomerAccountDeletionRequestController : ControllerBase
    {
        private ICustomerAccountDeletionRequestRepository _customerAccountDeletionRequestRepository;
        private IMapper _mapper;
        public CustomerAccountDeletionRequestController(ICustomerAccountDeletionRequestRepository customerAccountDeletionRequestRepository,
            IMapper mapper)
        {
            _customerAccountDeletionRequestRepository = customerAccountDeletionRequestRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// GET all deletion requests.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeletionRequestReadDTO>>> GetAllDeletionRequests()
        {
            var deletionRequestModels = await _customerAccountDeletionRequestRepository.GetAllDeletionRequests();

            return Ok(_mapper.Map<IEnumerable<DeletionRequestReadDTO>>(deletionRequestModels));
        }

        /// <summary>
        /// GET individual customer account deletion request.
        /// </summary>
        /// <param name="ID">Represents the customer's ID and is used to get their deletion request.</param>
        /// <returns></returns>
        [HttpGet("{ID}")]
        public async Task<ActionResult<DeletionRequestReadDTO>> GetDeletionRequest(int ID)
        {
            var deletionRequestModel = await _customerAccountDeletionRequestRepository.GetDeletionRequest(ID);

            if (deletionRequestModel != null)
                return Ok(_mapper.Map<DeletionRequestReadDTO>(deletionRequestModel));
            
            return NotFound();
        }
    }
}
