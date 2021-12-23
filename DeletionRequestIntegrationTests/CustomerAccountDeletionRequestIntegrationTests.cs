using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using CustomerAccountDeletionRequest;
using CustomerAccountDeletionRequest.DTOs;
using DeletionRequestIntegrationTests.Data;
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

            _configuration = new ConfigurationBuilder().AddUserSecrets<CustomerAccountDeletionRequestIntegrationTests>().Build();

            _auth0Settings = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("Auth0");
        }

        private async Task<string> GetAccessToken()
        {
            var auth0Client = new AuthenticationApiClient(_auth0Settings["Domain"]);
            var tokenRequest = new ClientCredentialsTokenRequest()
            {
                ClientId = _configuration["AuthClientID"],
                ClientSecret = _configuration["AuthClientSecret"],
                Audience = _auth0Settings["Audience"]
            };
            var tokenResponse = await auth0Client.GetTokenAsync(tokenRequest);

            return tokenResponse.AccessToken;
        }

        [Fact]
        public async void GetAllDeletionRequests_ReturnsUnauthorisedAccessWithNoToken()
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "api/CustomerAccountDeletionRequest");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [Fact]
        public async void GetAllDeletionRequests_ReturnsUnauthorisedAccessWithInvalidToken()
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "api/CustomerAccountDeletionRequest");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "Invalid token");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [Fact]
        public async void GetAllDeletionRequests_ReturnsForbiddenWithValidTokenInvalidRoles()
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "api/CustomerAccountDeletionRequest");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
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

            // Act
            var response = await _client.SendAsync(request);

            // Assert
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

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public async void GetDeletionRequest_ReturnsForbiddenWithValidTokenInvalidRoles(int ID)
        {
            //Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/CustomerAccountDeletionRequest/{ID}");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
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

            // Act
            var response = await _client.SendAsync(request);

            // Assert
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

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [Theory, MemberData(nameof(DeletionRequestCreateDTOObjects.GetDeletionRequestCreateDTOObjects),
                 MemberType = typeof(DeletionRequestCreateDTOObjects))]
        public async void CreateDeletionRequest_ReturnsForbiddenWithValidTokenInvalidRoles(int CustomerID, string DeletionReason)
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

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Theory, MemberData(nameof(DeletionRequestApproveDTOObjects.GetDeletionRequestApproveDTOObjects),
                 MemberType = typeof(DeletionRequestApproveDTOObjects))]
        public async void ApproveDeletionRequest_ReturnsUnauthorisedAccessWithNoToken(int CustomerID, int StaffID)
        {
            //Arrange
            DeletionRequestApproveDTO deletionRequestApproveDTO = new DeletionRequestApproveDTO()
            {
                StaffID = StaffID
            };

            var request = new HttpRequestMessage(HttpMethod.Patch, $"api/CustomerAccountDeletionRequest/Approve/{CustomerID}");

            request.Content = new StringContent(JsonConvert.SerializeObject(deletionRequestApproveDTO), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [Theory, MemberData(nameof(DeletionRequestApproveDTOObjects.GetDeletionRequestApproveDTOObjects),
                 MemberType = typeof(DeletionRequestApproveDTOObjects))]
        public async void ApproveDeletionRequest_ReturnsUnauthorisedAccessWithInvalidToken(int CustomerID, int StaffID)
        {
            //Arrange
            DeletionRequestApproveDTO deletionRequestApproveDTO = new DeletionRequestApproveDTO()
            {
                StaffID = StaffID
            };

            var request = new HttpRequestMessage(HttpMethod.Patch, $"api/CustomerAccountDeletionRequest/Approve/{CustomerID}");
            request.Content = new StringContent(JsonConvert.SerializeObject(deletionRequestApproveDTO), Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "Invalid token");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [Theory, MemberData(nameof(DeletionRequestApproveDTOObjects.GetDeletionRequestApproveDTOObjects),
                 MemberType = typeof(DeletionRequestApproveDTOObjects))]
        public async void ApproveDeletionRequest_ReturnsForbiddenWithValidTokenInvalidRoles(int CustomerID, int StaffID)
        {
            //Arrange
            DeletionRequestApproveDTO deletionRequestApproveDTO = new DeletionRequestApproveDTO()
            {
                StaffID = StaffID
            };

            var request = new HttpRequestMessage(HttpMethod.Patch, $"api/CustomerAccountDeletionRequest/Approve/{CustomerID}");
            request.Content = new StringContent(JsonConvert.SerializeObject(deletionRequestApproveDTO), Encoding.UTF8, "application/json");
            var accessToken = await GetAccessToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
