using AutoMapper;
using CustomerAccountDeletionRequest.Controllers;
using CustomerAccountDeletionRequest.CustomExceptionMiddleware;
using CustomerAccountDeletionRequest.DomainModels;
using CustomerAccountDeletionRequest.DTOs;
using CustomerAccountDeletionRequest.Models;
using CustomerAccountDeletionRequest.Profiles;
using CustomerAccountDeletionRequest.Repositories.Interfaces;
using CustomerAccountDeletionRequestTests.Data;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CustomerAccountDeletionRequestTests
{
    public class CustomerAccountDeletionRequestControllerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IMemoryCache> _memoryCacheMock;
        private readonly IOptions<MemoryCacheModel> _memoryCacheModel;

        /// <summary>
        /// Constructor to setup the reusable objects.
        /// </summary>
        public CustomerAccountDeletionRequestControllerTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DeletionRequestProfile());
            });

            _memoryCacheMock = new Mock<IMemoryCache>();
            _memoryCacheModel = Options.Create(new MemoryCacheModel());
            _mapper = config.CreateMapper();
        }

        /// <summary>
        /// Gets the expected deletion requests used for repo returns to isolate the controller.
        /// </summary>
        /// <returns>A list of DeletionRequestModel</returns>
        private List<DeletionRequestModel> GetDeletionRequests()
        {
            return new List<DeletionRequestModel>()
            {
                new DeletionRequestModel { DeletionRequestID = 1, CustomerID = 1, DeletionReason = "Terrible Site.", DateRequested = new System.DateTime(2010, 10, 01, 8, 5, 3), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 1, DeletionRequestStatus = CustomerAccountDeletionRequest.Enums.DeletionRequestStatusEnum.AwaitingDecision },
                new DeletionRequestModel { DeletionRequestID = 2, CustomerID = 2, DeletionReason = "Prefer Amazon.", DateRequested = new System.DateTime(2012, 01, 02, 10, 3, 45), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 1, DeletionRequestStatus = CustomerAccountDeletionRequest.Enums.DeletionRequestStatusEnum.AwaitingDecision },
                new DeletionRequestModel { DeletionRequestID = 3, CustomerID = 3, DeletionReason = "Too many clicks.", DateRequested = new System.DateTime(2013, 02, 03, 12, 2, 40), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 2, DeletionRequestStatus = CustomerAccountDeletionRequest.Enums.DeletionRequestStatusEnum.AwaitingDecision },
                new DeletionRequestModel { DeletionRequestID = 4, CustomerID = 4, DeletionReason = "Scammed into signing up.", DateRequested = new System.DateTime(2014, 03, 04, 14, 1, 35), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 3, DeletionRequestStatus = CustomerAccountDeletionRequest.Enums.DeletionRequestStatusEnum.AwaitingDecision },
                new DeletionRequestModel { DeletionRequestID = 5, CustomerID = 5, DeletionReason = "If Wish was built by students...", DateRequested = new System.DateTime(2007, 04, 05, 16, 50, 30), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 4, DeletionRequestStatus = CustomerAccountDeletionRequest.Enums.DeletionRequestStatusEnum.AwaitingDecision }
            };
        }

        /// <summary>
        /// Gets the expected deletion requests used to populate the _memoryCacheMock.
        /// </summary>
        /// <returns>A list of DeletionRequestModel from the _memoryCacheMock</returns>
        private List<DeletionRequestModel> GetDeletionRequestsForMemoryCache()
        {
            return new List<DeletionRequestModel>()
            {
                new DeletionRequestModel { DeletionRequestID = 1, CustomerID = 1, DeletionReason = "Terrible Site.", DateRequested = new System.DateTime(2010, 10, 01, 8, 5, 3), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 1, DeletionRequestStatus = CustomerAccountDeletionRequest.Enums.DeletionRequestStatusEnum.AwaitingDecision },
                new DeletionRequestModel { DeletionRequestID = 3, CustomerID = 3, DeletionReason = "Too many clicks.", DateRequested = new System.DateTime(2013, 02, 03, 12, 2, 40), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 2, DeletionRequestStatus = CustomerAccountDeletionRequest.Enums.DeletionRequestStatusEnum.AwaitingDecision },
                new DeletionRequestModel { DeletionRequestID = 5, CustomerID = 5, DeletionReason = "If Wish was built by students...", DateRequested = new System.DateTime(2007, 04, 05, 16, 50, 30), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 4, DeletionRequestStatus = CustomerAccountDeletionRequest.Enums.DeletionRequestStatusEnum.AwaitingDecision }
            };
        }

        [Fact]
        public async  void GetAll_ShouldReturnActionResultType()
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            var repoExpected = GetDeletionRequests();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            mockDeletionRequestRepo.Setup(dr => dr.GetAllAwaitingDeletionRequestsAsync()).ReturnsAsync(repoExpected).Verifiable();
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act
            var result = await customerAccountDeletionRequestController.GetAllDeletionRequests();

            //Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }
        
        [Fact]
        public async void GetAll_ShouldReturnAllDeletionRequestsCount()
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>();
            var repoExpected = GetDeletionRequests();
            mockDeletionRequestRepo.Setup(dr => dr.GetAllAwaitingDeletionRequestsAsync()).ReturnsAsync(repoExpected).Verifiable();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);
            
            //Act
            var result = await customerAccountDeletionRequestController.GetAllDeletionRequests();

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsAssignableFrom<IEnumerable<DeletionRequestReadDTO>>(actionResult.Value);
            Assert.Equal(repoExpected.Count, model.Count());
            
            mockDeletionRequestRepo.Verify(dr => dr.GetAllAwaitingDeletionRequestsAsync(), Times.Once());
        }

        [Fact]
        public async void GetAll_ShouldReturnAllDeletionRequestsContent()
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            var repoExpected = GetDeletionRequests();
            mockDeletionRequestRepo.Setup(dr => dr.GetAllAwaitingDeletionRequestsAsync()).ReturnsAsync(repoExpected).Verifiable();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);
            List<DeletionRequestReadDTO> expected = _mapper.Map<IEnumerable<DeletionRequestReadDTO>>(repoExpected).ToList();

            //Act
            var result = await customerAccountDeletionRequestController.GetAllDeletionRequests();

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            List<DeletionRequestReadDTO> model = Assert.IsAssignableFrom<IEnumerable<DeletionRequestReadDTO>>(actionResult.Value).ToList();
            model.Should().BeEquivalentTo(expected);
            mockDeletionRequestRepo.Verify(dr => dr.GetAllAwaitingDeletionRequestsAsync(), Times.Once());
        }

        [Fact]
        public async void GetAll_ShouldReturnAllDeletionRequestsContentFromCache()
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            var repoExpected = GetDeletionRequestsForMemoryCache();
            mockDeletionRequestRepo.Setup(dr => dr.GetAllAwaitingDeletionRequestsAsync()).ReturnsAsync(repoExpected).Verifiable();
            object tryGetValueExpected = GetDeletionRequestsForMemoryCache();
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(true);
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);
            List<DeletionRequestReadDTO> expected = _mapper.Map<IEnumerable<DeletionRequestReadDTO>>(repoExpected).ToList();

            //Act
            var result = await customerAccountDeletionRequestController.GetAllDeletionRequests();

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            List<DeletionRequestReadDTO> model = Assert.IsAssignableFrom<IEnumerable<DeletionRequestReadDTO>>(actionResult.Value).ToList();
            model.Should().BeEquivalentTo(expected);
            mockDeletionRequestRepo.Verify(dr => dr.GetAllAwaitingDeletionRequestsAsync(), Times.Never);
        }

        [Fact]
        public async void GetAll_ThrowsException()
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            mockDeletionRequestRepo.Setup(dr => dr.GetAllAwaitingDeletionRequestsAsync()).ThrowsAsync(new Exception()).Verifiable();
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act and Assert
            await Assert.ThrowsAsync<Exception>(async () => await customerAccountDeletionRequestController.GetAllDeletionRequests());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        [InlineData(int.MinValue)]
        public async void GetDeletionRequest_ThrowsArgumentOutOfRangeException_IDLessThanOne(int ID)
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act and Assert
            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await customerAccountDeletionRequestController.GetDeletionRequest(ID));
            Assert.Equal("IDs cannot be less than 1. (Parameter 'ID')", exception.Message);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public async void GetDeletionRequest_ShouldReturnFromMemoryCache(int ID)
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            object tryGetValueExpected = GetDeletionRequestsForMemoryCache();
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(true);
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act
            var result = await customerAccountDeletionRequestController.GetDeletionRequest(ID);

            //Assert
            Assert.IsType<OkObjectResult>(result.Result);
            mockDeletionRequestRepo.Verify(dr => dr.GetDeletionRequestAsync(ID), Times.Never);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public async void GetDeletionRequest_ShouldAddToMemoryCacheIfNotExists(int ID)
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            var repoExpected = GetDeletionRequests();
            mockDeletionRequestRepo.Setup(dr => dr.GetDeletionRequestAsync(ID)).ReturnsAsync(repoExpected.Find(dr => dr.CustomerID == ID)).Verifiable();
            object tryGetValueExpected = GetDeletionRequestsForMemoryCache();
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(true);
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act and Assert
            var expectedReturn = _mapper.Map<DeletionRequestReadDTO>(repoExpected.Find(dr => dr.CustomerID == ID));
            //Perform GET to add to the memory cache.
            await customerAccountDeletionRequestController.GetDeletionRequest(ID);
            mockDeletionRequestRepo.Verify(dr => dr.GetDeletionRequestAsync(ID), Times.Once());
            //Get the newly added DeletionRequest from the memoryCache.
            var action = await customerAccountDeletionRequestController.GetDeletionRequest(ID);
            var actionResult = Assert.IsType<OkObjectResult>(action.Result);
            DeletionRequestReadDTO returnedModel = Assert.IsAssignableFrom<DeletionRequestReadDTO>(actionResult.Value);
            returnedModel.Should().BeEquivalentTo(expectedReturn);
            mockDeletionRequestRepo.Verify(dr => dr.GetDeletionRequestAsync(ID), Times.Once());
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public async void GetDeletionRequest_ShouldReturnFromDBWhenNotInAnExistingCache(int ID)
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            var repoExpected = GetDeletionRequests();
            mockDeletionRequestRepo.Setup(dr => dr.GetDeletionRequestAsync(ID)).ReturnsAsync(repoExpected.Find(dr => dr.CustomerID == ID)).Verifiable();
            object tryGetValueExpected = GetDeletionRequestsForMemoryCache();
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(true);
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act
            var expectedReturn = _mapper.Map<DeletionRequestReadDTO>(repoExpected.Find(dr => dr.CustomerID == ID));
            var action = await customerAccountDeletionRequestController.GetDeletionRequest(ID);

            //Assert
            mockDeletionRequestRepo.Verify(dr => dr.GetDeletionRequestAsync(ID), Times.Once());
            var actionResult = Assert.IsType<OkObjectResult>(action.Result);
            DeletionRequestReadDTO returnedModel = Assert.IsAssignableFrom<DeletionRequestReadDTO>(actionResult.Value);
            returnedModel.Should().BeEquivalentTo(expectedReturn);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(int.MaxValue)]
        public async void GetDeletionRequest_ThrowsResourceNotFoundExceptionWhenIDNotInCacheOrDB(int ID)
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            var repoExpected = GetDeletionRequests();
            mockDeletionRequestRepo.Setup(dr => dr.GetDeletionRequestAsync(ID)).ReturnsAsync(repoExpected.Find(dr => dr.CustomerID == ID)).Verifiable();
            object tryGetValueExpected = GetDeletionRequestsForMemoryCache();
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(true);
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act and Assert
            var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(async () => await customerAccountDeletionRequestController.GetDeletionRequest(ID));
            Assert.Equal("A resource for ID: " + ID + " does not exist.", exception.Message);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(int.MaxValue)]
        public async void GetDeletionRequest_ThrowsResourceNotFoundExceptionWhenIDNotDBWithExpiredCache(int ID)
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            var repoExpected = GetDeletionRequests();
            mockDeletionRequestRepo.Setup(dr => dr.GetDeletionRequestAsync(ID)).ReturnsAsync(repoExpected.Find(dr => dr.CustomerID == ID)).Verifiable();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act and Assert
            var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(async () => await customerAccountDeletionRequestController.GetDeletionRequest(ID));
            Assert.Equal("A resource for ID: " + ID + " does not exist.", exception.Message);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public async void GetDeletionRequest_ShouldReturnFromDBWhenCacheExpired(int ID)
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            var repoExpected = GetDeletionRequests();
            mockDeletionRequestRepo.Setup(dr => dr.GetDeletionRequestAsync(ID)).ReturnsAsync(repoExpected.Find(dr => dr.CustomerID == ID)).Verifiable();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act
            var expectedReturn = _mapper.Map<DeletionRequestReadDTO>(repoExpected.Find(dr => dr.CustomerID == ID));
            var action = await customerAccountDeletionRequestController.GetDeletionRequest(ID);

            //Assert
            mockDeletionRequestRepo.Verify(dr => dr.GetDeletionRequestAsync(ID), Times.Once());
            var actionResult = Assert.IsType<OkObjectResult>(action.Result);
            DeletionRequestReadDTO returnedModel = Assert.IsAssignableFrom<DeletionRequestReadDTO>(actionResult.Value);
            returnedModel.Should().BeEquivalentTo(expectedReturn);
        }

        [Fact]
        public async void GetDeletionRequest_ThrowsException()
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            mockDeletionRequestRepo.Setup(dr => dr.GetDeletionRequestAsync(1)).ThrowsAsync(new Exception()).Verifiable();
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act and Assert
            await Assert.ThrowsAsync<Exception>(async () => await customerAccountDeletionRequestController.GetDeletionRequest(1));
        }

        [Theory, MemberData(nameof(DeletionRequestCreateDTOObjects.GetDeletionRequestCreateDTOObjects),
                 MemberType = typeof(DeletionRequestCreateDTOObjects))]
        public async void CreateDeletionRequest_ShouldReturnCreatedAtAction(int CustomerID, string DeletionReason)
        {
            //Arrange
            DeletionRequestCreateDTO newDeletionRequestCreateDTO = new DeletionRequestCreateDTO()
            {
                CustomerID = CustomerID,
                DeletionReason = DeletionReason
            };

            DeletionRequestModel repoDeletionRequestModel = new DeletionRequestModel()
            {
                DeletionRequestID = (GetDeletionRequests().Count + 1),
                CustomerID = CustomerID,
                DateRequested = DateTime.Now,
                DeletionRequestStatus = CustomerAccountDeletionRequest.Enums.DeletionRequestStatusEnum.AwaitingDecision,
                DeletionReason = DeletionReason,
                DateApproved = DateTime.MinValue,
                StaffID = 0
            };

            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            mockDeletionRequestRepo.Setup(dr => dr.CreateDeletionRequest(It.IsAny<DeletionRequestModel>())).Returns(repoDeletionRequestModel).Verifiable();
            mockDeletionRequestRepo.Setup(dr => dr.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act
            var result = await customerAccountDeletionRequestController.CreateDeletionRequest(newDeletionRequestCreateDTO);

            //Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);
            DeletionRequestReadDTO model = Assert.IsAssignableFrom<DeletionRequestReadDTO>(actionResult.Value);
        }

        [Fact]
        public async void CreateDeletionRequest_ThrowsArgumentNullException()
        {
            //Arrange
            DeletionRequestCreateDTO newDeletionRequestCreateDTO = null;

            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await customerAccountDeletionRequestController.CreateDeletionRequest(newDeletionRequestCreateDTO));
            Assert.Equal("The deletion request to be created cannot be null. (Parameter 'deletionRequestCreateDTO')", exception.Message);
        }

        [Theory, MemberData(nameof(DeletionRequestCreateDTOObjects.GetDeletionRequestCreateDTOObjects),
                 MemberType = typeof(DeletionRequestCreateDTOObjects))]
        public async void CreateDeletionRequest_ShouldCreateInDB(int CustomerID, string DeletionReason)
        {
            //Arrange
            DeletionRequestCreateDTO newDeletionRequestCreateDTO = new DeletionRequestCreateDTO()
            {
                CustomerID = CustomerID,
                DeletionReason = DeletionReason
            };

            DeletionRequestModel repoDeletionRequestModel = new DeletionRequestModel()
            {
                DeletionRequestID = (GetDeletionRequests().Count + 1),
                CustomerID = CustomerID,
                DateRequested = DateTime.Now,
                DeletionRequestStatus = CustomerAccountDeletionRequest.Enums.DeletionRequestStatusEnum.AwaitingDecision,
                DeletionReason = DeletionReason,
                DateApproved = DateTime.MinValue,
                StaffID = 0
            };

            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            mockDeletionRequestRepo.Setup(dr => dr.CreateDeletionRequest(It.IsAny<DeletionRequestModel>())).Returns(repoDeletionRequestModel).Verifiable();
            mockDeletionRequestRepo.Setup(dr => dr.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act
            var result = await customerAccountDeletionRequestController.CreateDeletionRequest(newDeletionRequestCreateDTO);

            //Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);
            DeletionRequestReadDTO model = Assert.IsAssignableFrom<DeletionRequestReadDTO>(actionResult.Value);
            mockDeletionRequestRepo.Verify(dr => dr.CreateDeletionRequest(It.IsAny<DeletionRequestModel>()), Times.Once());
            mockDeletionRequestRepo.Verify(dr => dr.SaveChangesAsync(), Times.Once());
        }

        [Theory, MemberData(nameof(DeletionRequestCreateDTOObjects.GetDeletionRequestCreateDTOObjects),
                 MemberType = typeof(DeletionRequestCreateDTOObjects))]
        public async void CreateDeletionRequest_ShouldAddToCache(int CustomerID, string DeletionReason)
        {
            //Arrange
            DeletionRequestCreateDTO newDeletionRequestCreateDTO = new DeletionRequestCreateDTO()
            {
                CustomerID = CustomerID,
                DeletionReason = DeletionReason
            };

            DeletionRequestModel repoDeletionRequestModel = new DeletionRequestModel()
            {
                DeletionRequestID = (GetDeletionRequests().Count + 1),
                CustomerID = CustomerID,
                DateRequested = DateTime.Now,
                DeletionRequestStatus = CustomerAccountDeletionRequest.Enums.DeletionRequestStatusEnum.AwaitingDecision,
                DeletionReason = DeletionReason,
                DateApproved = DateTime.MinValue,
                StaffID = 0
            };

            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            mockDeletionRequestRepo.Setup(dr => dr.CreateDeletionRequest(It.IsAny<DeletionRequestModel>())).Returns(repoDeletionRequestModel).Verifiable();
            mockDeletionRequestRepo.Setup(dr => dr.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();
            object tryGetValueExpected = GetDeletionRequestsForMemoryCache();
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(true);
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act
            //Add to cache with create call.
            await customerAccountDeletionRequestController.CreateDeletionRequest(newDeletionRequestCreateDTO);
            var result = customerAccountDeletionRequestController.GetDeletionRequest(repoDeletionRequestModel.CustomerID);

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result.Result);
            DeletionRequestReadDTO model = Assert.IsAssignableFrom<DeletionRequestReadDTO>(actionResult.Value);
            mockDeletionRequestRepo.Verify(dr => dr.GetDeletionRequestAsync(repoDeletionRequestModel.CustomerID), Times.Never());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        [InlineData(int.MinValue)]
        public async void ApproveDeletionRequest_ThrowsArgumentOutOfRangeException(int ID)
        {
            //Arrange
            JsonPatchDocument<DeletionRequestApproveDTO> jsonPatchDocument = new JsonPatchDocument<DeletionRequestApproveDTO>();

            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act and Assert
            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await customerAccountDeletionRequestController.ApproveDeletionRequest(ID, jsonPatchDocument));
            Assert.Equal("IDs cannot be less than 1. (Parameter 'ID')", exception.Message);
        }

        [Fact]
        public async void ApproveDeletionRequest_ThrowsArgumentNullException()
        {
            //Arrange
            JsonPatchDocument<DeletionRequestApproveDTO> jsonPatchDocument = null;
            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act and Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await customerAccountDeletionRequestController.ApproveDeletionRequest(1, jsonPatchDocument));
            Assert.Equal("The deletion request used to update cannot be null. (Parameter 'deletionRequestApprovePatch')", exception.Message);
        }

        [Theory]
        [InlineData(10, 1)]
        [InlineData(30, 2)]
        [InlineData(50, 3)]
        public async void ApproveDeletionRequest_ThrowsResourceNotFoundException(int ID, int staffID)
        {
            //Arrange
            JsonPatchDocument<DeletionRequestApproveDTO> jsonPatchDocument = new JsonPatchDocument<DeletionRequestApproveDTO>();
            DeletionRequestApproveDTO deletionRequestApproveDTO = new DeletionRequestApproveDTO()
            {
                StaffID = staffID
            };
            jsonPatchDocument.Replace<int>(dr => dr.StaffID, staffID);
            var repoExpected = GetDeletionRequests();
            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            mockDeletionRequestRepo.Setup(dr => dr.GetDeletionRequestAsync(ID)).ReturnsAsync(repoExpected.Find(dr => dr.CustomerID == ID)).Verifiable();
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act and Assert
            var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(async () => await customerAccountDeletionRequestController.ApproveDeletionRequest(ID, jsonPatchDocument));
            Assert.Equal("A resource for ID: " + ID + " does not exist.", exception.Message);
            mockDeletionRequestRepo.Verify(dr => dr.GetDeletionRequestAsync(ID), Times.Once());
        }

        [Theory, MemberData(nameof(DeletionRequestApproveDTOObjects.GetDeletionRequestApproveDTOObjects),
                 MemberType = typeof(DeletionRequestApproveDTOObjects))]
        public async void ApproveDeletionRequest_ReturnsNoContent(int ID, int staffID)
        {
            //Arrange
            JsonPatchDocument<DeletionRequestApproveDTO> jsonPatchDocument = new JsonPatchDocument<DeletionRequestApproveDTO>();
            DeletionRequestApproveDTO deletionRequestApproveDTO = new DeletionRequestApproveDTO()
            {
                StaffID = staffID
            };
            jsonPatchDocument.Replace<int>(dr => dr.StaffID, staffID);
            var repoExpected = GetDeletionRequests();
            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            mockDeletionRequestRepo.Setup(dr => dr.GetDeletionRequestAsync(ID)).ReturnsAsync(repoExpected.Find(dr => dr.CustomerID == ID)).Verifiable();
            mockDeletionRequestRepo.Setup(dr => dr.UpdateDeletionRequest(It.IsAny<DeletionRequestModel>())).Verifiable();
            mockDeletionRequestRepo.Setup(dr => dr.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();
            object tryGetValueExpected = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(false);
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<Object>()));
            customerAccountDeletionRequestController.ObjectValidator = objectValidator.Object;

            //Act
            var action = await customerAccountDeletionRequestController.ApproveDeletionRequest(ID, jsonPatchDocument);

            //Assert
            var actionResult = Assert.IsType<NoContentResult>(action);
            mockDeletionRequestRepo.Verify(dr => dr.GetDeletionRequestAsync(ID), Times.Once());
            mockDeletionRequestRepo.Verify(dr => dr.UpdateDeletionRequest(It.IsAny<DeletionRequestModel>()), Times.Once());
            mockDeletionRequestRepo.Verify(dr => dr.SaveChangesAsync(), Times.Once());
        }

        [Theory, MemberData(nameof(DeletionRequestApproveDTOObjects.GetDeletionRequestApproveDTOObjects),
                 MemberType = typeof(DeletionRequestApproveDTOObjects))]
        public async void ApproveDeletionRequest_ShouldAddToCache(int ID, int staffID)
        {
            //Arrange
            JsonPatchDocument<DeletionRequestApproveDTO> jsonPatchDocument = new JsonPatchDocument<DeletionRequestApproveDTO>();
            DeletionRequestApproveDTO deletionRequestApproveDTO = new DeletionRequestApproveDTO()
            {
                StaffID = staffID
            };
            jsonPatchDocument.Replace<int>(dr => dr.StaffID, staffID);
            var repoExpected = GetDeletionRequests();
            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            mockDeletionRequestRepo.Setup(dr => dr.GetDeletionRequestAsync(ID)).ReturnsAsync(repoExpected.Find(dr => dr.CustomerID == ID)).Verifiable();
            mockDeletionRequestRepo.Setup(dr => dr.UpdateDeletionRequest(It.IsAny<DeletionRequestModel>())).Verifiable();
            mockDeletionRequestRepo.Setup(dr => dr.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();
            object tryGetValueExpected = GetDeletionRequestsForMemoryCache();
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out tryGetValueExpected)).Returns(true);
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<Object>()));
            customerAccountDeletionRequestController.ObjectValidator = objectValidator.Object;

            //Act
            //Perform GET to add to the memory cache.
            await customerAccountDeletionRequestController.ApproveDeletionRequest(ID, jsonPatchDocument);
            //Get the newly added DeletionRequest from the memoryCache.
            var action = await customerAccountDeletionRequestController.GetDeletionRequest(ID);
            mockDeletionRequestRepo.Verify(dr => dr.GetDeletionRequestAsync(ID), Times.Once());

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(action.Result);
            DeletionRequestReadDTO returnedModel = Assert.IsAssignableFrom<DeletionRequestReadDTO>(actionResult.Value);
            Assert.Equal(returnedModel.CustomerID, ID);
            mockDeletionRequestRepo.Verify(dr => dr.GetDeletionRequestAsync(ID), Times.Once());
            mockDeletionRequestRepo.Verify(dr => dr.UpdateDeletionRequest(It.IsAny<DeletionRequestModel>()), Times.Once());
            mockDeletionRequestRepo.Verify(dr => dr.SaveChangesAsync(), Times.Once());
        }

        /*[Theory, MemberData(nameof(DeletionRequestApproveDTOObjects.GetDeletionRequestApproveDTOObjects),
                 MemberType = typeof(DeletionRequestApproveDTOObjects))]
        public async Task Update_WhenCalledWithModelStateError_ThrowsArgumentException(int ID, int StaffID)
        {
            // Arrange
            JsonPatchDocument<DeletionRequestApproveDTO> jsonPatchDocument = new JsonPatchDocument<DeletionRequestApproveDTO>();
            DeletionRequestApproveDTO deletionRequestApproveDTO = new DeletionRequestApproveDTO 
            { 
                StaffID = StaffID
            };
            jsonPatchDocument.Replace<int>(dr => dr.StaffID, StaffID);
            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            var repoExpected = GetDeletionRequests();

            mockDeletionRequestRepo.Setup(dr => dr.GetDeletionRequestAsync(ID)).ReturnsAsync(repoExpected.Find(dr => dr.CustomerID == ID)).Verifiable();

            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            customerAccountDeletionRequestController.ObjectValidator = new FaultyValidator();

            // Act
            await Assert.ThrowsAsync<ArgumentException>(async() => await customerAccountDeletionRequestController.ApproveDeletionRequest(ID, jsonPatchDocument));

            // Assert
            mockDeletionRequestRepo.Verify(dr => dr.GetDeletionRequestAsync(ID), Times.Once());
        }*/
    }
}
