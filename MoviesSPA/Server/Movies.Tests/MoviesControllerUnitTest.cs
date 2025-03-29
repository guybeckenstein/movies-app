using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Net;
using Xunit;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Movies.Api;
using Movies.Data.DTOs;
using Movies.Data.Models;
using Movies.Tests.Util;
using Movies.Api.Enums;

namespace Movies.Tests;

public sealed class MoviesControllerUnitTest(WebApplicationFactory<Program> factory) : BaseControllerUnitTest(factory)
{
    [Fact]
    public async Task GetExternalUrl_ShouldReturn200OK()
    {
        using var httpClient = new HttpClient(); // Different server than `https://localhost:7221`
        var response = await httpClient.GetAsync("https://gist.githubusercontent.com/saniyusuf/406b843afdfb9c6a86e25753fe2761f4/raw/523c324c7fcc36efab8224f9ebb7556c09b69a14/Film.JSON");

        response.StatusCode.Should().Be(HttpStatusCode.OK, becauseArgs: []);

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<MovieRequest>>(content);

        result.Should().AllSatisfy(movie => movie.Title.Should().NotBeNullOrEmpty(becauseArgs: []), becauseArgs: []);
        result.Should().AllSatisfy(movie => movie.Genre.Should().NotBeNullOrEmpty(becauseArgs: []), becauseArgs: []);
        result.Should().AllSatisfy(movie => movie.Year.Should().NotBeNullOrEmpty(becauseArgs: []), becauseArgs: []);
        result.Should().AllSatisfy(movie => movie.Poster.Should().NotBeNullOrEmpty(becauseArgs: []), becauseArgs: []);
    }

    [Fact]
    public async Task GetMovies_ShouldReturn401Unauthorized()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Wrong JWT token");
        var response = await _client.GetAsync($"{BASE_URI}/movies");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized, becauseArgs: []);
        response.Headers.WwwAuthenticate
            .Should().ContainSingle(becauseArgs: []).Which.ToString()
            .Should().BeEquivalentTo(@"Bearer error=""invalid_token""", becauseArgs: []);
    }

    [Fact]
    public async Task RegisterAndLoginAndGetMovies_ShouldReturn200Ok()
    {
        var byteContent = RequestExtensions.CreateRequestBody(new UserLogin
        {
            Username = "123456",
            Password = "654321",
            RememberMe = false,
        });
        await _client.PostAsync($"{BASE_URI}/Auth/{nameof(AuthUriEnum.Register).ToLower()}", byteContent);
        var response_1 = await _client.PostAsync($"{BASE_URI}/Auth/{nameof(AuthUriEnum.Login).ToLower()}", byteContent);

        var token = await response_1.Content.ReadAsStringAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response_2 = await _client.GetAsync($"{BASE_URI}/movies");

        response_2.StatusCode.Should().Be(HttpStatusCode.OK, becauseArgs: []);

        var content = await response_2.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<MovieResponse>>(content);

        result.Should().AllSatisfy(movie => movie.Title.Should().NotBeNullOrEmpty(becauseArgs: []), becauseArgs: []);
        result.Should().AllSatisfy(movie => movie.Genres.Should().NotBeNullOrEmpty(becauseArgs: []), becauseArgs: []);
        result.Should().AllSatisfy(movie => movie.Year.Should().NotBeNullOrEmpty(becauseArgs: []), becauseArgs: []);
        result.Should().AllSatisfy(movie => movie.PosterImg.Should().NotBeNullOrEmpty(becauseArgs: []), becauseArgs: []);
    }
}
