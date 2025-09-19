using AutoFixture;
using Azure;
using Microsoft.AspNetCore.Identity.Data;
using Newtonsoft.Json.Linq;
using Server_Ad_Baturina.Models.Requests;
using Server_Ad_Baturina.Models.Responses;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Server_Ad_Baturina.IntegrationTest.Api;

public class UserControllerIntegrationTests : IClassFixture<TestingWebAppFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly Fixture _fixture;

    public UserControllerIntegrationTests(TestingWebAppFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetAll_GET_ReturnsOkResponse()
    {
        //Arrange
        var token = await GetTokenAsync();

        //Act
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/v1/users");
        var responseData = await response.Content.ReadFromJsonAsync<CustomSuccessResponse<List<PublicUserResponse>>>();

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseData);
        Assert.True(responseData.Success);
        Assert.Equal(200, responseData.StatusCode);
    }

    [Fact]
    public async Task Update_PUT_ReturnsOkResponse()
    {
        //Arrange
        var token = await GetTokenAsync();

        var request = _fixture.Build<PutUserRequest>()
             .With(r => r.Name, _fixture.Create<string>().Substring(0, 25))
             .With(r => r.Role, _fixture.Create<string>().Substring(0, 25))
             .With(r => r.Email, "testuser@example.com")
             .Create();

        //Act 
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.PutAsJsonAsync("api/v1/users", request);
        var responseData = await response.Content.ReadFromJsonAsync<CustomSuccessResponse<PutUserResponse>>();

        //Assert 
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseData);
        Assert.True(responseData.Success);
        Assert.Equal(200, responseData.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_DELETE_ReturnsOkResponse()
    {
        //Arrahge
        var token = await GetTokenAsync();

        //Act
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.DeleteAsync("api/v1/users");
        var responseData = await response.Content.ReadFromJsonAsync<BaseSuccessResponse>();

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseData);
        Assert.True(responseData.Success);
        Assert.Equal(200, responseData.StatusCode);
    }

    [Fact]
    public async Task GetForId_GET_ReturnsOkResponse()
    {
        //Arrange
        var userId = await GetUserIdAsync();

        //Act 
        var response = await _client.GetAsync($"api/v1/users/{userId}");
        var responseData = await response.Content.ReadFromJsonAsync<CustomSuccessResponse<PublicUserResponse>>();

        //Assert 
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseData);
        Assert.True(responseData.Success);
        Assert.Equal(200, responseData.StatusCode);
    }

    [Fact]
    public async Task Get_GET_ReturnOKResponse()
    {
        //Arrange
        var token = await GetTokenAsync();
        
        //Act
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("api/v1/users/find");
        var responseData = await response.Content.ReadFromJsonAsync<CustomSuccessResponse<PublicUserResponse>>();

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseData);
        Assert.True(responseData.Success);
        Assert.Equal(200, responseData.StatusCode);
    }

    private async Task<string> GetTokenAsync()
    {
        var loginRequest = new SignInRequest
        {
            Email = "alice@example.com",
            Password = "Passsword12345&9",
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/sign-in", loginRequest);
        var loginData = await loginResponse.Content.ReadFromJsonAsync<CustomSuccessResponse<SignInUserResponse>>();

        return loginData.Data.Token;
    }

    private async Task<Guid> GetUserIdAsync()
    {
        var token = await GetTokenAsync();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/v1/users/find");

        var data = await response.Content.ReadFromJsonAsync<CustomSuccessResponse<PublicUserResponse>>();

        return data.Data.Id;
    }
}