using System.Net;
using Xunit;

namespace FitForge.Api.IntegrationTests;

public class SwaggerTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SwaggerTests(TestWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetSwagger_ReturnsNotFoundInTestingEnvironment()
    {
        // Act
        var response = await _client.GetAsync("/swagger/v1/swagger.json");

        // Assert - Swagger is disabled in Testing environment
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
