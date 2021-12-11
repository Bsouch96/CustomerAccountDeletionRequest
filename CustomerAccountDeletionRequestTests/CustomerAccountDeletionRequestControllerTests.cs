using AutoMapper;
using CustomerAccountDeletionRequest.Controllers;
using CustomerAccountDeletionRequest.DomainModels;
using CustomerAccountDeletionRequest.DTOs;
using CustomerAccountDeletionRequest.Models;
using CustomerAccountDeletionRequest.Profiles;
using CustomerAccountDeletionRequest.Repositories.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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

        public CustomerAccountDeletionRequestControllerTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DeletionRequestProfile());
            });

            _memoryCacheMock = new Mock<IMemoryCache>();
            object expectedValue = null;
            _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out expectedValue)).Returns(false);
            _memoryCacheModel = Options.Create(new MemoryCacheModel());
            _mapper = config.CreateMapper();
        }

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

        [Fact]
        public async  void GetAll_ShouldReturnActionResultType()
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            var expected = GetDeletionRequests();
            mockDeletionRequestRepo.Setup(dr => dr.GetAllAwaitingDeletionRequestsAsync()).ReturnsAsync(expected).Verifiable();
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act
            var result = await customerAccountDeletionRequestController.GetAllDeletionRequests();

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
        }
        
        [Fact]
        public async void GetAll_ShouldReturnAllDeletionRequestsCount()
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>();
            var expected = GetDeletionRequests();
            mockDeletionRequestRepo.Setup(dr => dr.GetAllAwaitingDeletionRequestsAsync()).ReturnsAsync(expected).Verifiable();
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);
            
            //Act
            var result = await customerAccountDeletionRequestController.GetAllDeletionRequests();

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsAssignableFrom<IEnumerable<DeletionRequestReadDTO>>(actionResult.Value);
            Assert.Equal(expected.Count, model.Count());
            
            mockDeletionRequestRepo.Verify(dr => dr.GetAllAwaitingDeletionRequestsAsync(), Times.Once);
        }

        [Fact]
        public async void GetAll_ShouldReturnAllDeletionRequestsContent()
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            var setup = GetDeletionRequests();
            mockDeletionRequestRepo.Setup(dr => dr.GetAllAwaitingDeletionRequestsAsync()).ReturnsAsync(setup).Verifiable();
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);
            List<DeletionRequestReadDTO> expected = _mapper.Map<IEnumerable<DeletionRequestReadDTO>>(setup).ToList();

            //Act
            var result = await customerAccountDeletionRequestController.GetAllDeletionRequests();

            //Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            List<DeletionRequestReadDTO> model = Assert.IsAssignableFrom<IEnumerable<DeletionRequestReadDTO>>(actionResult.Value).ToList();

            model.Should().BeEquivalentTo(expected);

            mockDeletionRequestRepo.Verify(dr => dr.GetAllAwaitingDeletionRequestsAsync(), Times.Once);
        }

        [Fact]
        public async void GetAll_ThrowsException()
        {
            //Arrange
            var mockDeletionRequestRepo = new Mock<ICustomerAccountDeletionRequestRepository>(MockBehavior.Strict);
            var expected = GetDeletionRequests();
            mockDeletionRequestRepo.Setup(dr => dr.GetAllAwaitingDeletionRequestsAsync()).ThrowsAsync(new Exception()).Verifiable();
            var customerAccountDeletionRequestController = new CustomerAccountDeletionRequestController(mockDeletionRequestRepo.Object, _mapper, _memoryCacheMock.Object, _memoryCacheModel);

            //Act and Assert
            await Assert.ThrowsAsync<Exception>(async () => await customerAccountDeletionRequestController.GetAllDeletionRequests());
        }
    }
}
