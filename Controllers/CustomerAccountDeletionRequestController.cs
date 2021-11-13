using AutoMapper;
using CustomerAccountDeletionRequest.DomainModels;
using CustomerAccountDeletionRequest.DTOs;
using CustomerAccountDeletionRequest.Repositories.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
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

        public CustomerAccountDeletionRequestController(ICustomerAccountDeletionRequestRepository customerAccountDeletionRequestRepository,
            IMapper mapper, IMemoryCache memoryCache)
        {
            _customerAccountDeletionRequestRepository = customerAccountDeletionRequestRepository;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// GET all deletion requests.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeletionRequestReadDTO>>> GetAllDeletionRequests()
        {
            if(_memoryCache.TryGetValue("CustomerAccountDeletionRequests", out List<DeletionRequestModel> deletionRequestValues))
                return Ok(_mapper.Map<IEnumerable<DeletionRequestReadDTO>>(deletionRequestValues));

            var deletionRequestModels = await _customerAccountDeletionRequestRepository.GetAllDeletionRequestsAsync();
            _memoryCache.Set("CustomerAccountDeletionRequests", deletionRequestModels, GetMemoryCacheEntryOptions());
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
            DeletionRequestModel deletionRequestModel;
            //If cache exists and we find the entity.
            if (_memoryCache.TryGetValue("CustomerAccountDeletionRequests", out List<DeletionRequestModel> deletionRequestCacheValues))
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

                return NotFound();
            }

            //If cache has expired, get entity from DB and return it.
            deletionRequestModel = await _customerAccountDeletionRequestRepository.GetDeletionRequestAsync(ID);

            if (deletionRequestModel != null)
                return Ok(_mapper.Map<DeletionRequestReadDTO>(deletionRequestModel));
                
            return NotFound();
        }

        /// <summary>
        /// This function is used to create a deletion request for a customer.
        /// </summary>
        /// <param name="deletionRequestCreateDTO">The parameters supplied to create a deletion request by the POSTing API.</param>
        /// <returns></returns>
        [Route("Create")]
        [HttpPost]
        public async Task<ActionResult> CreateDeletionRequest([FromBody] DeletionRequestCreateDTO deletionRequestCreateDTO)
        {
            if (deletionRequestCreateDTO == null)
                return BadRequest();

            var deletionRequestModel = _mapper.Map<DeletionRequestModel>(deletionRequestCreateDTO);
            deletionRequestModel.DeletionRequestStatus = Enums.DeletionRequestStatusEnum.AwaitingDecision;
            deletionRequestModel.DateRequested = DateTime.Now;

            DeletionRequestModel newDeletionRequest = _customerAccountDeletionRequestRepository.CreateDeletionRequest(deletionRequestModel);
            await _customerAccountDeletionRequestRepository.SaveChangesAsync();

            if(_memoryCache.TryGetValue("CustomerAccountDeletionRequests", out List<DeletionRequestModel> deletionRequestValues))
                deletionRequestValues.Add(newDeletionRequest);

            return CreatedAtAction(nameof(GetDeletionRequest), new { ID = newDeletionRequest.CustomerID }, newDeletionRequest);
        }

        /// <summary>
        /// This function will approve a customer's account deletion request. It updates the deletion request with the StaffID that approved
        /// the request and the DateTime that the request was approved.
        /// </summary>
        /// <param name="ID">The ID of the customer account that will have their account request approved.</param>
        /// <returns></returns>
        [Route("Approve/{ID}")]
        [HttpPut]
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

            if (_memoryCache.TryGetValue("CustomerAccountDeletionRequests", out List<DeletionRequestModel> deletionRequestValues))
            {
                deletionRequestValues.RemoveAll(delReq => delReq.CustomerID == deletionRequestModel.CustomerID);
                deletionRequestValues.Add(deletionRequestModel);
            }

            return NoContent();
        }

        private MemoryCacheEntryOptions GetMemoryCacheEntryOptions()
        {
            return new MemoryCacheEntryOptions()
            {
                SlidingExpiration = new TimeSpan(0, 10, 0),
                AbsoluteExpirationRelativeToNow = new TimeSpan(0, 20, 0),
                Priority = CacheItemPriority.Normal,
                Size = 1028
            };
        }
    }
}
