using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Newtonsoft.Json;
using Movies.Api;
using Movies.Models.Models;
using Movies.Tests.Util;
using Movies.Tests.Helpers;
using Movies.Api.Enums;

namespace Movies.Tests;

public sealed class AuthControllerUnitTest(WebApplicationFactory<Program> factory) : BaseControllerUnitTest(factory)
{
    private const string BASE_URI = "/api/Auth";

    [Fact]
    public async Task Register_ShouldReturn400BadRequest()
    {
        var byteContent = RequestExtensions.CreateRequestBody(null);
        var response = await _client.PostAsync($"{BASE_URI}/{AuthUriEnum.Register.ToString().ToLower()}", byteContent);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_ShouldReturn200OK()
    {
        var byteContent = RequestExtensions.CreateRequestBody(AuthControllerHelper.CreateUser());
        var response = await _client.PostAsync($"{BASE_URI}/{AuthUriEnum.Register.ToString().ToLower()}", byteContent);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<User>(content);

        result.Username.Should().NotBeNull();
        result.PasswordHash.Should().NotBeNull();
    }

    [Fact]
    public async Task Login_ShouldReturn400BadRequest()
    {
        var byteContent = RequestExtensions.CreateRequestBody(AuthControllerHelper.CreateUser());
        var response = await _client.PostAsync($"{BASE_URI}/{AuthUriEnum.Login.ToString().ToLower()}", byteContent);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterAndLogin_ShouldReturn200Ok()
    {
        var byteContent = RequestExtensions.CreateRequestBody(AuthControllerHelper.CreateUser());
        await _client.PostAsync($"{BASE_URI}/{AuthUriEnum.Register.ToString().ToLower()}", byteContent);
        var response = await _client.PostAsync($"{BASE_URI}/{AuthUriEnum.Login.ToString().ToLower()}", byteContent);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadAsStringAsync();

        result.Should().NotBeNull();
    }
}
