using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace RadiusSearch.Tests.Api;

public class FeasibilityEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public FeasibilityEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_MissingLatitude_Returns400()
    {
        var response = await _client.GetAsync("/api/feasibility?longitude=-43.18298&radius=500");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_MissingLatitude_Returns400WithCorrectSchema()
    {
        var response = await _client.GetAsync("/api/feasibility?longitude=-43.18298&radius=500");
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.GetProperty("code").GetString().Should().Be("400");
        body.GetProperty("reason").GetString().Should().NotBeNullOrEmpty();
        body.GetProperty("message").GetString().Should().Contain("latitude");
        body.GetProperty("status").GetString().Should().Be("bad request");
        body.GetProperty("timestamp").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Get_InvalidRadius_Returns400()
    {
        var response = await _client.GetAsync(
            "/api/feasibility?latitude=-22.91016&longitude=-43.18298&radius=5");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_ValidRequest_Returns200WithHeaders()
    {
        var response = await _client.GetAsync(
            "/api/feasibility?latitude=-22.91016&longitude=-43.18298&radius=1000");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Contains("X-Request-Id").Should().BeTrue();
        response.Headers.Contains("X-Response-Time").Should().BeTrue();
    }

    [Fact]
    public async Task Get_CoordinateWithTrailingZeros_Returns200()
    {
        var response = await _client.GetAsync(
            "/api/feasibility?latitude=-22.91000&longitude=-43.18298&radius=1000");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task HealthCheck_Returns200()
    {
        var response = await _client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
