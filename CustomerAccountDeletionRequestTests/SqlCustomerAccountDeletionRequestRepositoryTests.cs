using CustomerAccountDeletionRequest.Context;
using CustomerAccountDeletionRequest.CustomExceptionMiddleware;
using CustomerAccountDeletionRequest.DomainModels;
using CustomerAccountDeletionRequest.Enums;
using CustomerAccountDeletionRequest.Repositories.Concrete;
using CustomerAccountDeletionRequestTests.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Update;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CustomerAccountDeletionRequestTests
{
    public class SqlCustomerAccountDeletionRequestRepositoryTests
    {
        public SqlCustomerAccountDeletionRequestRepositoryTests(){}

        /// <summary>
        /// Gets the expected deletion requests used for repo returns to isolate the controller.
        /// </summary>
        /// <returns>A list of DeletionRequestModel</returns>
        private List<DeletionRequestModel> GetDeletionRequests()
        {
            return new List<DeletionRequestModel>()
            {
                new DeletionRequestModel { DeletionRequestID = 1, CustomerID = 1, DeletionReason = "Terrible Site.", DateRequested = new System.DateTime(2010, 10, 01, 8, 5, 3), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 1, DeletionRequestStatus = CustomerAccountDeletionRequest.Enums.DeletionRequestStatusEnum.AwaitingDecision },
                new DeletionRequestModel { DeletionRequestID = 2, CustomerID = 2, DeletionReason = "Prefer Amazon.", DateRequested = new System.DateTime(2012, 01, 02, 10, 3, 45), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 1, DeletionRequestStatus = CustomerAccountDeletionRequest.Enums.DeletionRequestStatusEnum.Approved },
                new DeletionRequestModel { DeletionRequestID = 3, CustomerID = 3, DeletionReason = "Too many clicks.", DateRequested = new System.DateTime(2013, 02, 03, 12, 2, 40), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 2, DeletionRequestStatus = CustomerAccountDeletionRequest.Enums.DeletionRequestStatusEnum.AwaitingDecision },
                new DeletionRequestModel { DeletionRequestID = 4, CustomerID = 4, DeletionReason = "Scammed into signing up.", DateRequested = new System.DateTime(2014, 03, 04, 14, 1, 35), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 3, DeletionRequestStatus = CustomerAccountDeletionRequest.Enums.DeletionRequestStatusEnum.Approved },
                new DeletionRequestModel { DeletionRequestID = 5, CustomerID = 5, DeletionReason = "If Wish was built by students...", DateRequested = new System.DateTime(2007, 04, 05, 16, 50, 30), DateApproved = new System.DateTime(1, 1, 1, 0, 0, 0), StaffID = 4, DeletionRequestStatus = CustomerAccountDeletionRequest.Enums.DeletionRequestStatusEnum.AwaitingDecision }
            };
        }

        /// <summary>
        /// Create a mock of the Context class seeded with DeletionRequestModels.
        /// </summary>
        /// <returns>A mock of type Context</returns>
        private Mock<Context> GetDbContext()
        {
            var context = new Mock<Context>();
            context.Object.AddRange(GetDeletionRequests());
            context.Object.SaveChanges();
            
            return context;
        }

        /// <summary>
        /// Create a mock DbSet of type DeletionRequestModel.
        /// </summary>
        /// <returns>A mock DbSet of type DeletionRequestModel</returns>
        private Mock<DbSet<DeletionRequestModel>> GetMockDbSet()
        {
            return GetDeletionRequests().AsQueryable().BuildMockDbSet();
        }

        [Fact]
        public void GetAllAwaitingDeletionRequestsAsync_ShouldReturnAllAwaitingTypeDeletionRequests()
        {
            //Arrange
            var dbContextMock = GetDbContext();
            var dbSetMock = GetMockDbSet();
            dbContextMock.SetupGet(c => c._deletionRequestContext).Returns(dbSetMock.Object);
            var sqlCustomerAccountDeletionRequestRepository = new SqlCustomerAccountDeletionRequestRepository(dbContextMock.Object);
            var expectedResult = GetDeletionRequests().Where(dr => dr.DeletionRequestStatus == CustomerAccountDeletionRequest.Enums.DeletionRequestStatusEnum.AwaitingDecision);
            
            //Act
            var result = sqlCustomerAccountDeletionRequestRepository.GetAllAwaitingDeletionRequestsAsync();

            //Assert
            Assert.NotNull(result);
            var actionResult = Assert.IsType<List<DeletionRequestModel>>(result.Result);
            var model = Assert.IsAssignableFrom<List<DeletionRequestModel>>(actionResult);
            Assert.Equal(expectedResult.Count(), model.Count());
            model.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        [InlineData(int.MinValue)]
        public async void GetDeletionRequestAsync_ThrowsArgumentOutOfRangeException(int ID)
        {
            //Arrange
            var dbContextMock = GetDbContext();
            var dbSetMock = GetMockDbSet();
            dbContextMock.SetupGet(c => c._deletionRequestContext).Returns(dbSetMock.Object);
            var sqlCustomerAccountDeletionRequestRepository = new SqlCustomerAccountDeletionRequestRepository(dbContextMock.Object);
            var expectedResult = GetDeletionRequests().Where(dr => dr.DeletionRequestStatus == CustomerAccountDeletionRequest.Enums.DeletionRequestStatusEnum.AwaitingDecision);

            //Act and Assert
            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await sqlCustomerAccountDeletionRequestRepository.GetDeletionRequestAsync(ID));
            Assert.Equal("IDs cannot be less than 1. (Parameter 'ID')", exception.Message);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public void GetDeletionRequestAsync_ShouldReturnDeletionRequest(int ID)
        {
            //Arrange
            var dbContextMock = GetDbContext();
            var dbSetMock = GetMockDbSet();
            dbContextMock.SetupGet(c => c._deletionRequestContext).Returns(dbSetMock.Object);
            var sqlCustomerAccountDeletionRequestRepository = new SqlCustomerAccountDeletionRequestRepository(dbContextMock.Object);
            var expectedResult = GetDeletionRequests().FirstOrDefault(dr => dr.CustomerID == ID);

            //Act
            var result = sqlCustomerAccountDeletionRequestRepository.GetDeletionRequestAsync(ID);

            //Assert
            Assert.NotNull(result);
            var actionResult = Assert.IsType<DeletionRequestModel>(result.Result);
            var model = Assert.IsAssignableFrom<DeletionRequestModel>(actionResult);
            model.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(int.MaxValue)]
        public async void GetDeletionRequestAsync_ThrowsResourceNotFoundException(int ID)
        {
            //Arrange
            var dbContextMock = GetDbContext();
            var dbSetMock = GetMockDbSet();
            dbContextMock.SetupGet(c => c._deletionRequestContext).Returns(dbSetMock.Object);
            var sqlCustomerAccountDeletionRequestRepository = new SqlCustomerAccountDeletionRequestRepository(dbContextMock.Object);
            var expectedResult = GetDeletionRequests().Where(dr => dr.DeletionRequestStatus == CustomerAccountDeletionRequest.Enums.DeletionRequestStatusEnum.AwaitingDecision);

            //Act and Assert
            var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(async () => await sqlCustomerAccountDeletionRequestRepository.GetDeletionRequestAsync(ID));
            Assert.Equal("A resource for ID: " + ID + " does not exist.", exception.Message);
        }

        [Fact]
        public void CreateDeletionRequest_ThrowsArgumentNullException()
        {
            //Arrange
            var dbContextMock = GetDbContext();
            var dbSetMock = GetMockDbSet();
            dbContextMock.SetupGet(c => c._deletionRequestContext).Returns(dbSetMock.Object);
            var sqlCustomerAccountDeletionRequestRepository = new SqlCustomerAccountDeletionRequestRepository(dbContextMock.Object);
            DeletionRequestModel deletionRequestModel = null;

            //Act and Assert
            var exception = Assert.Throws<ArgumentNullException>(() => sqlCustomerAccountDeletionRequestRepository.CreateDeletionRequest(deletionRequestModel));
            Assert.Equal("The deletion request to be created cannot be null. (Parameter 'deletionRequestModel')", exception.Message);
        }

        [Theory, MemberData(nameof(DeletionRequestModelObjects.GetDeletionRequestModelCreateObjects),
                 MemberType = typeof(DeletionRequestModelObjects))]
        public async void CreateDeletionRequest_ShouldReturnCreatedDeletionRequest(int ID, string DeletionReason, DateTime DateRequested,
            DateTime DateApproved, int StaffID, DeletionRequestStatusEnum DeletionRequestStatus)
        {
            //Arrange
            var options = new DbContextOptionsBuilder<Context>()
                        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                        .Options;
            var dbContextMock = new Context(options);
            dbContextMock._deletionRequestContext.AddRange(GetDeletionRequests());
            await dbContextMock.SaveChangesAsync();
            DeletionRequestModel deletionRequestModel = new DeletionRequestModel()
            {
                CustomerID = ID,
                DeletionReason = DeletionReason,
                DateRequested = DateRequested,
                DateApproved = DateApproved,
                StaffID = StaffID,
                DeletionRequestStatus = DeletionRequestStatus,
                DeletionRequestID = 0
            };
            DeletionRequestModel expectedDeletionRequestModel = new DeletionRequestModel()
            {
                CustomerID = ID,
                DeletionReason = DeletionReason,
                DateRequested = DateRequested,
                DateApproved = DateApproved,
                StaffID = StaffID,
                DeletionRequestStatus = DeletionRequestStatus,
                DeletionRequestID = (GetDeletionRequests().Count + 1)
            };
            var sqlCustomerAccountDeletionRequestRepository = new SqlCustomerAccountDeletionRequestRepository(dbContextMock);

            //Act
            var result = sqlCustomerAccountDeletionRequestRepository.CreateDeletionRequest(deletionRequestModel);

            //Assert
            var actionResult = Assert.IsType<DeletionRequestModel>(result);
            DeletionRequestModel deletionRequestModelResult = Assert.IsAssignableFrom<DeletionRequestModel>(actionResult);
            deletionRequestModelResult.Should().NotBeNull();
            deletionRequestModelResult.Should().BeEquivalentTo(expectedDeletionRequestModel);
        }

        [Fact]
        public void UpdateDeletionRequest_ThrowsArgumentNullException()
        {
            //Arrange
            var dbContextMock = GetDbContext();
            var dbSetMock = GetMockDbSet();
            dbContextMock.SetupGet(c => c._deletionRequestContext).Returns(dbSetMock.Object);
            var sqlCustomerAccountDeletionRequestRepository = new SqlCustomerAccountDeletionRequestRepository(dbContextMock.Object);
            DeletionRequestModel deletionRequestModel = null;

            //Act and Assert
            var exception = Assert.Throws<ArgumentNullException>(() => sqlCustomerAccountDeletionRequestRepository.UpdateDeletionRequest(deletionRequestModel));
            Assert.Equal("The deletion request to be updated cannot be null. (Parameter 'deletionRequestModel')", exception.Message);
        }
        
        [Theory, MemberData(nameof(DeletionRequestModelObjects.GetDeletionRequestModelUpdateObjects),
                 MemberType = typeof(DeletionRequestModelObjects))]
        public void UpdateDeletionRequest_ShouldUpdateDeletionRequest(int ID, string DeletionReason, DateTime DateRequested,
            DateTime DateApproved, int StaffID, DeletionRequestStatusEnum DeletionRequestStatus, int DeletionRequestID)
        {
            //Arrange
            var dbContextMock = GetDbContext();
            var dbSetMock = GetMockDbSet();
            dbContextMock.SetupGet(c => c._deletionRequestContext).Returns(dbSetMock.Object);
            var sqlCustomerAccountDeletionRequestRepository = new SqlCustomerAccountDeletionRequestRepository(dbContextMock.Object);
            DeletionRequestModel deletionRequestModel = new DeletionRequestModel()
            {
                CustomerID = ID,
                DeletionReason = DeletionReason,
                DateRequested = DateRequested,
                DateApproved = DateApproved,
                StaffID = StaffID,
                DeletionRequestStatus = DeletionRequestStatus,
                DeletionRequestID = DeletionRequestID
            };

            //Act
            sqlCustomerAccountDeletionRequestRepository.UpdateDeletionRequest(deletionRequestModel);

            //Assert
            dbSetMock.Verify(r => r.Update(It.IsAny<DeletionRequestModel>()), Times.Once);
        }

        [Fact]
        public async void SaveChangesAsync_ShouldOnlySaveOnce()
        {
            //Arrange
            var dbContextMock = GetDbContext();
            var dbSetMock = GetMockDbSet();
            dbContextMock.Setup(c => c.SaveChangesAsync(It.IsAny< CancellationToken>())).Returns(Task.FromResult(1)).Verifiable();
            var sqlCustomerAccountDeletionRequestRepository = new SqlCustomerAccountDeletionRequestRepository(dbContextMock.Object);

            //Act
            await sqlCustomerAccountDeletionRequestRepository.SaveChangesAsync();

            //Assert
            dbContextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
