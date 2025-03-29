using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Newtonsoft.Json;
using Movies.Api;
using Movies.Tests.Util;
using Movies.Tests.Helpers;
using Movies.Api.Enums;
using Movies.Data.Entities;

namespace Movies.Tests;

public sealed class AuthControllerUnitTest(WebApplicationFactory<Program> factory) : BaseControllerUnitTest(factory)
{
    [Fact]
    public async Task Register_ShouldReturn400BadRequest()
    {
        var byteContent = RequestExtensions.CreateRequestBody(null);
        var response = await _client.PostAsync($"{BASE_URI}/Auth/{nameof(AuthUriEnum.Register).ToLower()}", byteContent);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, becauseArgs: []);
    }

    [Fact]
    public async Task Register_ShouldReturn200OK()
    {
        var byteContent = RequestExtensions.CreateRequestBody(AuthControllerHelper.CreateUser());
        var response = await _client.PostAsync($"{BASE_URI}/Auth/{nameof(AuthUriEnum.Register).ToLower()}", byteContent);

        response.StatusCode.Should().Be(HttpStatusCode.OK, becauseArgs: []);

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<User>(content);

        result.Username.Should().NotBeNull(becauseArgs: []);
        result.Password.Should().NotBeNull(becauseArgs: []);
    }

    [Fact]
    public async Task Login_ShouldReturn400BadRequest()
    {
        var byteContent = RequestExtensions.CreateRequestBody(AuthControllerHelper.CreateUser());
        var response = await _client.PostAsync($"{BASE_URI}/Auth/{nameof(AuthUriEnum.Login).ToLower()}", byteContent);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, becauseArgs: []);
    }

    [Fact]
    public async Task RegisterAndLogin_ShouldReturn200Ok()
    {
        var byteContent = RequestExtensions.CreateRequestBody(AuthControllerHelper.CreateUser());
        await _client.PostAsync($"{BASE_URI}/Auth/{nameof(AuthUriEnum.Register).ToLower()}", byteContent);
        var response = await _client.PostAsync($"{BASE_URI}/Auth/{nameof(AuthUriEnum.Login).ToLower()}", byteContent);

        response.StatusCode.Should().Be(HttpStatusCode.OK, becauseArgs: []);

        var result = await response.Content.ReadAsStringAsync();

        result.Should().NotBeNull(becauseArgs: []);
    }
}
