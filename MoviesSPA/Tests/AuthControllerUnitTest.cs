using Api;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Newtonsoft.Json;
using Models.Models;
using Models.DTOs;
using Tests.Util;

namespace Tests
{
    public class AuthControllerUnitTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthControllerUnitTest(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_ShouldReturn400BadRequest()
        {
            var byteContent = RequestExtensions.CreateRequestBody(null);
            var response = await _client.PostAsync("/api/Auth/register", byteContent);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Register_ShouldReturn200OK()
        {
            var byteContent = RequestExtensions.CreateRequestBody(new UserDto
            {
                Username = "123456",
                Password = "654321",
            });
            var response = await _client.PostAsync("/api/Auth/register", byteContent);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<User>(content);

            result.Username.Should().NotBeNull();
            result.PasswordHash.Should().NotBeNull();
        }

        [Fact]
        public async Task Login_ShouldReturn400BadRequest()
        {
            var byteContent = RequestExtensions.CreateRequestBody(new UserDto
            {
                Username = "123456",
                Password = "654321",
            });
            var response = await _client.PostAsync("/api/Auth/login", byteContent);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RegisterAndLogin_ShouldReturn200Ok()
        {
            var byteContent = RequestExtensions.CreateRequestBody(new UserDto
            {
                Username = "123456",
                Password = "654321",
            });
            await _client.PostAsync("/api/Auth/register", byteContent);
            var response = await _client.PostAsync("/api/Auth/login", byteContent);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadAsStringAsync();

            result.Should().NotBeNull();
        }
    }
}
