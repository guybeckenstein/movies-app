using Microsoft.AspNetCore.Mvc.Testing;
using Movies.Api;
using System.Net.Http;
using Xunit;

namespace Movies.Tests;

public abstract class BaseControllerUnitTest : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient _client;

    internal BaseControllerUnitTest(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
}
