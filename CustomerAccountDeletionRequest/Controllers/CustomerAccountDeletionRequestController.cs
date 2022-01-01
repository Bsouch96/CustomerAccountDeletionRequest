using AutoMapper;
using CustomerAccountDeletionRequest.CustomExceptionMiddleware;
using CustomerAccountDeletionRequest.DomainModels;
using CustomerAccountDeletionRequest.DTOs;
using CustomerAccountDeletionRequest.Models;
using CustomerAccountDeletionRequest.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CustomerAccountDeletionRequest.Controllers
{
    [ApiController]
    [Route("api/CustomerAccountDeletionRequest")]
    public class CustomerAccountDeletionRequestController : ControllerBase
    {
        private readonly ICustomerAccountDeletionRequestRepository _customerAccountDeletionRequestRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheModel _memoryCacheModel;

        public CustomerAccountDeletionRequestController(ICustomerAccountDeletionRequestRepository customerAccountDeletionRequestRepository,
            IMapper mapper, IMemoryCache memoryCache, IOptions<MemoryCacheModel> memoryCacheModel)
        {
            _customerAccountDeletionRequestRepository = customerAccountDeletionRequestRepository;
            _mapper = mapper;
            _memoryCache = memoryCache;
            _memoryCacheModel = memoryCacheModel.Value;
        }

        /// <summary>
        /// GET all deletion requests.
        /// </summary>
        /// <returns>
        /// An Ok() (Statuscode 200) Object ActionResult alongside a collection of DeletionRequestReadDTOs
        /// or an appropriate statuscode based on the exception thrown.
        /// </returns>
        [Authorize("ReadAllCustomerAccountDeletionRequests")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeletionRequestReadDTO>>> GetAllDeletionRequests()
        {
            if(_memoryCache.TryGetValue(_memoryCacheModel.CustomerAccountDeletionRequests, out List<DeletionRequestModel> deletionRequestValues))
                return Ok(_mapper.Map<IEnumerable<DeletionRequestReadDTO>>(deletionRequestValues.Where(dr => dr.DeletionRequestStatus == Enums.DeletionRequestStatusEnum.AwaitingDecision)));

            var deletionRequestModels = await _customerAccountDeletionRequestRepository.GetAllAwaitingDeletionRequestsAsync();
            return Ok(_mapper.Map<IEnumerable<DeletionRequestReadDTO>>(deletionRequestModels));
        }

        /// <summary>
        /// GET individual customer account deletion request.
        /// </summary>
        /// <param name="ID">Represents the customer's ID and is used to get their deletion request.</param>
        /// <returns>
        /// An Ok() (Statuscode 200) Object ActionResult alongside a collection of DeletionRequestReadDTOs
        /// or an appropriate statuscode based on the exception thrown.
        /// </returns>
        [Authorize("ReadCustomerAccountDeletionRequest")]
        [HttpGet("{ID}")]
        public async Task<ActionResult<DeletionRequestReadDTO>> GetDeletionRequest(int ID)
        {
            if (ID < 1)
                throw new ArgumentOutOfRangeException(nameof(ID), "IDs cannot be less than 1.");

            DeletionRequestModel deletionRequestModel;
            //If cache exists and we find the entity.
            if (_memoryCache.TryGetValue(_memoryCacheModel.CustomerAccountDeletionRequests, out List<DeletionRequestModel> deletionRequestCacheValues))
            {   
                //Return the entity if we find it in the cache.
                deletionRequestModel = deletionRequestCacheValues.Find(delReq => delReq.CustomerID == ID);
                if(deletionRequestModel != null)
                    return Ok(_mapper.Map<DeletionRequestReadDTO>(deletionRequestModel));

                //Otherwise, get the entity from the DB, add it to the cache and return it.
                deletionRequestModel = await _customerAccountDeletionRequestRepository.GetDeletionRequestAsync(ID);
                if(deletionRequestModel != null)
                {
                    deletionRequestCacheValues.Add(deletionRequestModel);
                    return Ok(_mapper.Map<DeletionRequestReadDTO>(deletionRequestModel));
                }

                throw new ResourceNotFoundException("A resource for ID: " + ID + " does not exist.");
            }

            //If cache has expired, get entity from DB and return it.
            deletionRequestModel = await _customerAccountDeletionRequestRepository.GetDeletionRequestAsync(ID);

            if (deletionRequestModel != null)
                return Ok(_mapper.Map<DeletionRequestReadDTO>(deletionRequestModel));

            throw new ResourceNotFoundException("A resource for ID: " + ID + " does not exist.");
        }

        /// <summary>
        /// This function is used to create a deletion request for a customer.
        /// </summary>
        /// <param name="deletionRequestCreateDTO">The parameters supplied to create a deletion request by the POSTing API.</param>
        /// <returns>
        /// A CreatedAtAction() (Statuscode 201) ActionResult or an appropriate Statuscode based on the exception thrown.
        /// </returns>
        [Authorize("CreateCustomerAccountDeletionRequest")]
        [Route("Create")]
        [HttpPost]
        public async Task<ActionResult> CreateDeletionRequest([FromBody] DeletionRequestCreateDTO deletionRequestCreateDTO)
        {
            if (deletionRequestCreateDTO == null)
                throw new ArgumentNullException(nameof(deletionRequestCreateDTO), "The deletion request to be created cannot be null.");

            var deletionRequestModel = _mapper.Map<DeletionRequestModel>(deletionRequestCreateDTO);
            deletionRequestModel.DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision;
            deletionRequestModel.DateRequested = DateTime.Now;

            DeletionRequestModel newDeletionRequest = _customerAccountDeletionRequestRepository.CreateDeletionRequest(deletionRequestModel);
            await _customerAccountDeletionRequestRepository.SaveChangesAsync();

            if(_memoryCache.TryGetValue(_memoryCacheModel.CustomerAccountDeletionRequests, out List<DeletionRequestModel> deletionRequestValues))
                deletionRequestValues.Add(newDeletionRequest);

            var deletionRequestReadDTO = _mapper.Map<DeletionRequestReadDTO>(newDeletionRequest);

            return CreatedAtAction(nameof(GetDeletionRequest), new { ID = newDeletionRequest.CustomerID }, deletionRequestReadDTO);
        }

        /// <summary>
        /// This function will approve a customer's account deletion request. It updates the deletion request with the StaffID that approved
        /// the request and the DateTime that the request was approved.
        /// </summary>
        /// <param name="ID">The ID of the customer account that will have their account request approved.</param>
        /// <returns>
        /// A NoContent() (Statuscode 204) ActionResult or an appropriate statuscode based on the exception thrown.
        /// </returns>
        [Authorize("UpdateCustomerAccountDeletionRequest")]
        [Route("Approve/{ID}")]
        [HttpPatch]
        public async Task<ActionResult> ApproveDeletionRequest(int ID, JsonPatchDocument<DeletionRequestApproveDTO> deletionRequestApprovePatch)
        {
            if (ID < 1)
                throw new ArgumentOutOfRangeException(nameof(ID), "IDs cannot be less than 1.");

            if (deletionRequestApprovePatch == null)
                throw new ArgumentNullException(nameof(deletionRequestApprovePatch), "The deletion request used to update cannot be null.");

            var deletionRequestModel = await _customerAccountDeletionRequestRepository.GetDeletionRequestAsync(ID);
            if (deletionRequestModel == null)
                throw new ResourceNotFoundException("A resource for ID: " + ID + " does not exist.");

            var newDeletionRequest = _mapper.Map<DeletionRequestApproveDTO>(deletionRequestModel);
            deletionRequestApprovePatch.ApplyTo(newDeletionRequest, ModelState);

            if (!TryValidateModel(newDeletionRequest))
                return ValidationProblem(ModelState);

            deletionRequestModel.DateApproved = System.DateTime.Now;
            deletionRequestModel.DeletionRequestStatus = Enums.DeletionRequestStatusEnum.Approved;

            _mapper.Map(newDeletionRequest, deletionRequestModel);

            _customerAccountDeletionRequestRepository.UpdateDeletionRequest(deletionRequestModel);
            await _customerAccountDeletionRequestRepository.SaveChangesAsync();

            if (_memoryCache.TryGetValue(_memoryCacheModel.CustomerAccountDeletionRequests, out List<DeletionRequestModel> deletionRequestValues))
                deletionRequestValues.RemoveAll(delReq => delReq.CustomerID == deletionRequestModel.CustomerID);

            return NoContent();
        }
    }
}
