using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using CustomerAccountDeletionRequest;
using CustomerAccountDeletionRequest.DomainModels;
using CustomerAccountDeletionRequest.DTOs;
using CustomerAccountDeletionRequest.Models;
using DeletionRequestIntegrationTests.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DeletionRequestIntegrationTests
{
    public class CustomerAccountDeletionRequestIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _auth0Settings;

        public CustomerAccountDeletionRequestIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();

            _configuration = new ConfigurationBuilder()
                .AddUserSecrets<CustomerAccountDeletionRequestIntegrationTests>()
                .AddJsonFile("appsettings.json")
                .Build();

            _auth0Settings = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("Auth0");
        }

        /// <summary>
        /// Acquire the access token via the Auth0 API to comply with authorisation and authentication rules.
        /// </summary>
        /// <returns>An access token that can be used to access the deletion request's API endpoints</returns>
        private async Task<string> GetAccessToken()
        {
            var auth0Client = new AuthenticationApiClient(_auth0Settings["Domain"]);
            var tokenRequest = new ClientCredentialsTokenRequest()
            {
                ClientId = (_configuration["AuthClientID"] == "" ? _configuration["Auth0:{{AuthClientID}}"] : _configuration["AuthClientID"]),
                ClientSecret = (_configuration["AuthClientSecret"] == "" ? _configuration["Auth0:{{AuthClientSecret}}"] : _configuration["AuthClientSecret"]),
                Audience = _auth0Settings["Audience"]
            };
            var tokenResponse = await auth0Client.GetTokenAsync(tokenRequest);

            return tokenResponse.AccessToken;
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

        [Fact]
        public async void GetAllDeletionRequests_ReturnsUnauthorisedAccessWithNoToken()
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "api/CustomerAccountDeletionRequest");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [Fact]
        public async void GetAllDeletionRequests_ReturnsUnauthorisedAccessWithInvalidToken()
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "api/CustomerAccountDeletionRequest");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "Invalid token");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [Fact]
        public async void GetAllDeletionRequests_ReturnsOKResponseWithValidToken()
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "api/CustomerAccountDeletionRequest");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public async void GetDeletionRequest_ReturnsUnauthorisedAccessWithNoToken(int ID)
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/CustomerAccountDeletionRequest/{ID}");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public async void GetDeletionRequest_ReturnsUnauthorisedAccessWithInvalidToken(int ID)
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/CustomerAccountDeletionRequest/{ID}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "Invalid token");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public async void GetDeletionRequest_ReturnsOKResponseWithValidToken(int ID)
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/CustomerAccountDeletionRequest/{ID}");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public async void GetDeletionRequest_ReturnsCorrectDeletionRequest(int ID)
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/CustomerAccountDeletionRequest/{ID}");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await _client.SendAsync(request);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var deletionRequest = JsonConvert.DeserializeObject<DeletionRequestReadDTO>(stringResponse);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(deletionRequest.CustomerID == ID);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        [InlineData(int.MinValue)]
        public async void GetDeletionRequest_ThrowsArgumentOutOfRangeException_HandledByGlobalExceptionHandler(int ID)
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/CustomerAccountDeletionRequest/{ID}");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string errorMessage = "IDs cannot be less than 1. (Parameter 'ID')";

            //Act and Assert
            var response = await _client.SendAsync(request);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var errorModel = JsonConvert.DeserializeObject<ErrorModel>(stringResponse);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal((int)HttpStatusCode.BadRequest, errorModel.StatusCode);
            Assert.Equal(errorMessage, errorModel.ErrorMessage);
        }
        
        [Theory]
        [InlineData(30)]
        [InlineData(300)]
        [InlineData(int.MaxValue)]
        public async void GetDeletionRequest_ThrowsResourceNotFoundException_HandledByGlobalExceptionHandler(int ID)
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/CustomerAccountDeletionRequest/{ID}");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string errorMessage = $"A resource for ID: {ID} does not exist.";

            //Act and Assert
            var response = await _client.SendAsync(request);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var errorModel = JsonConvert.DeserializeObject<ErrorModel>(stringResponse);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal((int)HttpStatusCode.NotFound, errorModel.StatusCode);
            Assert.Equal(errorMessage, errorModel.ErrorMessage);
        }

        [Theory, MemberData(nameof(DeletionRequestCreateDTOObjects.GetDeletionRequestCreateDTOObjects),
                 MemberType = typeof(DeletionRequestCreateDTOObjects))]
        public async void CreateDeletionRequest_ReturnsUnauthorisedAccessWithNoToken(int CustomerID, string DeletionReason)
        {
            //Arrange
            DeletionRequestCreateDTO deletionRequestCreateDTO = new DeletionRequestCreateDTO()
            {
                CustomerID = CustomerID,
                DeletionReason = DeletionReason
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "api/CustomerAccountDeletionRequest/Create");
            request.Content = new StringContent(JsonConvert.SerializeObject(deletionRequestCreateDTO), Encoding.UTF8, "application/json");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [Theory, MemberData(nameof(DeletionRequestCreateDTOObjects.GetDeletionRequestCreateDTOObjects),
                 MemberType = typeof(DeletionRequestCreateDTOObjects))]
        public async void CreateDeletionRequest_ReturnsUnauthorisedAccessWithInvalidToken(int CustomerID, string DeletionReason)
        {
            //Arrange
            DeletionRequestCreateDTO deletionRequestCreateDTO = new DeletionRequestCreateDTO()
            {
                CustomerID = CustomerID,
                DeletionReason = DeletionReason
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "api/CustomerAccountDeletionRequest/Create");
            request.Content = new StringContent(JsonConvert.SerializeObject(deletionRequestCreateDTO), Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "Invalid token");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [Theory, MemberData(nameof(DeletionRequestCreateDTOObjects.GetDeletionRequestCreateDTOObjects),
                 MemberType = typeof(DeletionRequestCreateDTOObjects))]
        public async void CreateDeletionRequest_ReturnsCreatedResponseWithValidToken(int CustomerID, string DeletionReason)
        {
            //Arrange
            DeletionRequestCreateDTO deletionRequestCreateDTO = new DeletionRequestCreateDTO()
            {
                CustomerID = CustomerID,
                DeletionReason = DeletionReason
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "api/CustomerAccountDeletionRequest/Create");
            request.Content = new StringContent(JsonConvert.SerializeObject(deletionRequestCreateDTO), Encoding.UTF8, "application/json");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
        
        [Theory, MemberData(nameof(DeletionRequestCreateDTOObjects.GetDeletionRequestCreateDTOObjects),
                 MemberType = typeof(DeletionRequestCreateDTOObjects))]
        public async void CreateDeletionRequest_CanCreateDeletionRequest(int CustomerID, string DeletionReason)
        {
            //Arrange
            DeletionRequestCreateDTO deletionRequestCreateDTO = new DeletionRequestCreateDTO()
            {
                CustomerID = CustomerID,
                DeletionReason = DeletionReason
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "api/CustomerAccountDeletionRequest/Create");
            request.Content = new StringContent(JsonConvert.SerializeObject(deletionRequestCreateDTO), Encoding.UTF8, "application/json");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await _client.SendAsync(request);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var returnedDeletionRequest = JsonConvert.DeserializeObject<DeletionRequestModel>(stringResponse);

            //Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal($"/api/CustomerAccountDeletionRequest/{CustomerID}", response.Headers.Location.AbsolutePath);
            Assert.Equal(deletionRequestCreateDTO.CustomerID, returnedDeletionRequest.CustomerID);
            Assert.Equal(deletionRequestCreateDTO.DeletionReason, returnedDeletionRequest.DeletionReason);
        }

        [Theory, MemberData(nameof(DeletionRequestApproveDTOObjects.GetDeletionRequestApproveDTOObjects),
                 MemberType = typeof(DeletionRequestApproveDTOObjects))]
        public async void ApproveDeletionRequest_ReturnsUnauthorisedAccessWithNoToken(int CustomerID, int StaffID)
        {
            //Arrange
            JsonPatchDocument<DeletionRequestApproveDTO> jsonPatchDocument = new JsonPatchDocument<DeletionRequestApproveDTO>();
            DeletionRequestApproveDTO deletionRequestApproveDTO = new DeletionRequestApproveDTO()
            {
                StaffID = StaffID
            };
            jsonPatchDocument.Replace<int>(dr => dr.StaffID, StaffID);
            var request = new HttpRequestMessage(HttpMethod.Patch, $"api/CustomerAccountDeletionRequest/Approve/{CustomerID}");
            request.Content = new StringContent(JsonConvert.SerializeObject(jsonPatchDocument), Encoding.UTF8, "application/json");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [Theory, MemberData(nameof(DeletionRequestApproveDTOObjects.GetDeletionRequestApproveDTOObjects),
                 MemberType = typeof(DeletionRequestApproveDTOObjects))]
        public async void ApproveDeletionRequest_ReturnsUnauthorisedAccessWithInvalidToken(int CustomerID, int StaffID)
        {
            //Arrange
            JsonPatchDocument<DeletionRequestApproveDTO> jsonPatchDocument = new JsonPatchDocument<DeletionRequestApproveDTO>();
            DeletionRequestApproveDTO deletionRequestApproveDTO = new DeletionRequestApproveDTO()
            {
                StaffID = StaffID
            };
            jsonPatchDocument.Replace<int>(dr => dr.StaffID, StaffID);
            var request = new HttpRequestMessage(HttpMethod.Patch, $"api/CustomerAccountDeletionRequest/Approve/{CustomerID}");
            request.Content = new StringContent(JsonConvert.SerializeObject(jsonPatchDocument), Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "Invalid token");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [Theory, MemberData(nameof(DeletionRequestApproveDTOObjects.GetDeletionRequestApproveDTOObjects),
                 MemberType = typeof(DeletionRequestApproveDTOObjects))]
        public async void ApproveDeletionRequest_ReturnsNoContent(int CustomerID, int StaffID)
        {
            //Arrange
            JsonPatchDocument<DeletionRequestApproveDTO> jsonPatchDocument = new JsonPatchDocument<DeletionRequestApproveDTO>();
            DeletionRequestApproveDTO deletionRequestApproveDTO = new DeletionRequestApproveDTO()
            {
                StaffID = StaffID
            };
            jsonPatchDocument.Replace<int>(dr => dr.StaffID, StaffID);
            var request = new HttpRequestMessage(HttpMethod.Patch, $"api/CustomerAccountDeletionRequest/Approve/{CustomerID}");
            request.Content = new StringContent(JsonConvert.SerializeObject(jsonPatchDocument), Encoding.UTF8, "application/json");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
        
        [Theory]
        [InlineData(0, 1)]
        [InlineData(-10, 2)]
        [InlineData(int.MinValue, 3)]
        public async void ApproveDeletionRequest_ThrowsArgumentOutOfRangeException_HandledByGlobalExceptionHandler(int CustomerID, int StaffID)
        {
            //Arrange
            JsonPatchDocument<DeletionRequestApproveDTO> jsonPatchDocument = new JsonPatchDocument<DeletionRequestApproveDTO>();
            jsonPatchDocument.Replace<int>(dr => dr.StaffID, StaffID);
            var request = new HttpRequestMessage(HttpMethod.Patch, $"api/CustomerAccountDeletionRequest/Approve/{CustomerID}");
            request.Content = new StringContent(JsonConvert.SerializeObject(jsonPatchDocument), Encoding.UTF8, "application/json");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string errorMessage = "IDs cannot be less than 1. (Parameter 'ID')";

            //Act
            var response = await _client.SendAsync(request);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var errorModel = JsonConvert.DeserializeObject<ErrorModel>(stringResponse);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal((int)HttpStatusCode.BadRequest, errorModel.StatusCode);
            Assert.Equal(errorMessage, errorModel.ErrorMessage);
        }
        
        [Theory]
        [InlineData(50, 1)]
        [InlineData(100, 2)]
        [InlineData(int.MaxValue, 3)]
        public async void ApproveDeletionRequest_ThrowsResourceNotFoundException_HandledByGlobalExceptionHandler(int CustomerID, int StaffID)
        {
            //Arrange
            JsonPatchDocument<DeletionRequestApproveDTO> jsonPatchDocument = new JsonPatchDocument<DeletionRequestApproveDTO>();
            jsonPatchDocument.Replace<int>(dr => dr.StaffID, StaffID);
            var request = new HttpRequestMessage(HttpMethod.Patch, $"api/CustomerAccountDeletionRequest/Approve/{CustomerID}");
            request.Content = new StringContent(JsonConvert.SerializeObject(jsonPatchDocument), Encoding.UTF8, "application/json");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string errorMessage = $"A resource for ID: {CustomerID} does not exist."; ;

            //Act
            var response = await _client.SendAsync(request);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var errorModel = JsonConvert.DeserializeObject<ErrorModel>(stringResponse);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal((int)HttpStatusCode.NotFound, errorModel.StatusCode);
            Assert.Equal(errorMessage, errorModel.ErrorMessage);
        }
    }
}
