using AutoMapper;
using CustomerAccountDeletionRequest.DomainModels;
using CustomerAccountDeletionRequest.DTOs;
using CustomerAccountDeletionRequest.Repositories.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
            var deletionRequestModels = await _customerAccountDeletionRequestRepository.GetAllDeletionRequestsAsync();

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
            var deletionRequestModel = await _customerAccountDeletionRequestRepository.GetDeletionRequestAsync(ID);

            if (deletionRequestModel != null)
                return Ok(_mapper.Map<DeletionRequestReadDTO>(deletionRequestModel));
            
            return NotFound();
        }

        /// <summary>
        /// This function is used to create a deletion request for a customer.
        /// </summary>
        /// <param name="deletionRequestCreateDTO">The parameters supplied to create a deletion request by the POSTing API.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateDeletionRequest([FromBody] DeletionRequestCreateDTO deletionRequestCreateDTO)
        {
            var deletionRequestModel = _mapper.Map<DeletionRequestModel>(deletionRequestCreateDTO);
            deletionRequestModel.DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision;
            deletionRequestModel.DateRequested = DateTime.Now;

            int newDeletionRequestID = _customerAccountDeletionRequestRepository.CreateDeletionRequest(deletionRequestModel);
            await _customerAccountDeletionRequestRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDeletionRequest), new { ID = newDeletionRequestID});
        }

        /// <summary>
        /// This function will approve a customer's account deletion request. It updates the deletion request with the StaffID that approved
        /// the request and the DateTime that the request was approved.
        /// </summary>
        /// <param name="ID">The ID of the customer account that will have their account request approved.</param>
        /// <returns></returns>
        [HttpPut("{ID}")]
        public async Task<ActionResult> ApproveDeletionRequest(int ID, JsonPatchDocument<DeletionRequestApproveDTO> deletionRequestApprovePatch)
        {
            var deletionRequestModel = await _customerAccountDeletionRequestRepository.GetDeletionRequestAsync(ID);
            if (deletionRequestModel == null)
                return NotFound();

            var newDeletionRequest = _mapper.Map<DeletionRequestApproveDTO>(deletionRequestModel);
            deletionRequestApprovePatch.ApplyTo(newDeletionRequest, ModelState);

            if (!TryValidateModel(newDeletionRequest))
                return ValidationProblem(ModelState);

            deletionRequestModel.DateApproved = System.DateTime.Now;
            deletionRequestModel.DeletionRequestStatus = Enums.DeletionRequestStatusEnum.Approved;

            _mapper.Map(newDeletionRequest, deletionRequestModel);

            _customerAccountDeletionRequestRepository.UpdateDeletionRequest(deletionRequestModel);
            await _customerAccountDeletionRequestRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
