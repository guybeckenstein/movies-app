using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DTOs;
using Models.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Net;
using Xunit;
using Api;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Tests.Util;

namespace Tests
{
    public class MoviesControllerUnitTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public MoviesControllerUnitTest(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetExternalUrl_ShouldReturn200OK()
        {
            using var httpClient = new HttpClient(); // Different server than `https://localhost:7221`
            var response = await httpClient.GetAsync("https://gist.githubusercontent.com/saniyusuf/406b843afdfb9c6a86e25753fe2761f4/raw/523c324c7fcc36efab8224f9ebb7556c09b69a14/Film.JSON");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<MovieRequest>>(content);

            result.Should().AllSatisfy(movie => movie.Title.Should().NotBeNullOrEmpty());
            result.Should().AllSatisfy(movie => movie.Genre.Should().NotBeNullOrEmpty());
            result.Should().AllSatisfy(movie => movie.Year.Should().NotBeNullOrEmpty());
            result.Should().AllSatisfy(movie => movie.Poster.Should().NotBeNullOrEmpty());
        }

        [Fact]
        public async Task GetMovies_ShouldReturn401Unauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Wrong JWT token");
            var response = await _client.GetAsync("/api/movies");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            response.Headers.WwwAuthenticate
                .Should().ContainSingle().Which.ToString()
                .Should().BeEquivalentTo(@"Bearer error=""invalid_token""");
        }

        [Fact]
        public async Task RegisterAndLoginAndGetMovies_ShouldReturn200Ok()
        {
            var byteContent = RequestExtensions.CreateRequestBody(new UserDto
            {
                Username = "123456",
                Password = "654321",
            });
            await _client.PostAsync("/api/Auth/register", byteContent);
            var response_1 = await _client.PostAsync("/api/Auth/login", byteContent);

            var token = await response_1.Content.ReadAsStringAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response_2 = await _client.GetAsync("/api/movies");

            response_2.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response_2.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<MovieRequest>>(content);

            result.Should().AllSatisfy(movie => movie.Title.Should().NotBeNullOrEmpty());
            result.Should().AllSatisfy(movie => movie.Genre.Should().NotBeNullOrEmpty());
            result.Should().AllSatisfy(movie => movie.Year.Should().NotBeNullOrEmpty());
            result.Should().AllSatisfy(movie => movie.Poster.Should().NotBeNullOrEmpty());
        }
    }
}
