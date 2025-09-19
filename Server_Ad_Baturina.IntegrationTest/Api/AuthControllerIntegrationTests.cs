using AutoFixture;
using Microsoft.AspNetCore.Identity.Data;
using Server_Ad_Baturina.Models.Requests;
using Server_Ad_Baturina.Models.Responses;
using System.Net;
using System.Net.Http.Json;

namespace Server_Ad_Baturina.IntegrationTest.Api;

public class AuthControllerIntegrationTests : IClassFixture<TestingWebAppFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly Fixture _fixture;

    public AuthControllerIntegrationTests(TestingWebAppFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Register_POST_ReturnsCreatedResponse()
    {
        //Arrange
        var registerRequest = _fixture.Build<RegisterUserRequest>()
             .With(r => r.Name, _fixture.Create<string>().Substring(0, 25))
             .With(r => r.Role, _fixture.Create<string>().Substring(0, 25))
             .With(r => r.Email, "testuser@example.com")
             .With(r => r.Password, "StrongPassword123!")
             .Create();

        //Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);
        var responseData = await response.Content.ReadFromJsonAsync<CustomSuccessResponse<SignInUserResponse>>();

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);    
        Assert.NotNull(responseData);
        Assert.True(responseData.Success);
        Assert.Equal(201, responseData.StatusCode);
        Assert.NotNull(responseData.Data);
        Assert.False(string.IsNullOrEmpty(responseData.Data.Token));
        Assert.Equal(registerRequest.Email, responseData.Data.Email);
    }

    [Fact]
    public async Task Login_POST_ReturnsOkResponse()
    {
        //Arrange
        var loginRequest = new SignInRequest
        {
            Email = "alice@example.com",
            Password = "Passsword12345&9",
        };

        //Act
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/sign-in", loginRequest);
        var responseBody = await loginResponse.Content.ReadFromJsonAsync<CustomSuccessResponse<SignInUserResponse>>();

        //Assert
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.NotNull(responseBody);
        Assert.True(responseBody.Success);
        Assert.Equal(200, responseBody.StatusCode);
        Assert.NotNull(responseBody.Data);
        Assert.False(string.IsNullOrEmpty(responseBody.Data.Token));
    }
}