using AutoFixture;
using Azure.Core;
using Microsoft.AspNetCore.Identity.Data;
using Server_Ad_Baturina.Models.Requests;
using Server_Ad_Baturina.Models.Responses;
using Server_advanced_Baturina.Models.DTOs;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Server_Ad_Baturina.IntegrationTest.Api;

public class NewsControllerIntegratioTests : IClassFixture<TestingWebAppFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly Fixture _fixture;

    public NewsControllerIntegratioTests(TestingWebAppFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Create_POST_ReturnsCreatedResponse()
    {
        //Arrange
        var request = _fixture.Build<NewsRequest>()
            .With(x => x.Title, _fixture.Create<string>().PadRight(25, 'x').Substring(0, 25))
            .With(x => x.Description, _fixture.Create<string>().PadRight(150, 'x').Substring(0, 150))
            .With(x => x.Tags, new[]
            {
                _fixture.Create<string>().PadRight(15, 'x').Substring(0, 15),
                _fixture.Create<string>().PadRight(10, 'x').Substring(0, 10),
                _fixture.Create<string>().PadRight(12, 'x').Substring(0, 12)
            })
            .With(x => x.Image, _fixture.Create<string>().PadRight(20, 'x').Substring(0, 20) + ".jpg")
            .Create();

        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await _client.PostAsJsonAsync("api/v1/news", request);
        var responseData = await response.Content.ReadFromJsonAsync<CustomSuccessResponse<CreateNewsSuccessResponse>>();

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(responseData);
        Assert.True(responseData.Success);
        Assert.Equal(201, responseData.StatusCode);
        Assert.NotNull(responseData.Data.Id);
    }

    [Fact]
    public async Task Put_PUT_ReturnsOkResponse()
    {
        //Arrange
        var newId = new Guid("b5c6d7e8-f9a0-1234-bcde-f56789012345");

        var request = _fixture.Build<NewsRequest>()
            .With(x => x.Title, _fixture.Create<string>().PadRight(25, 'x').Substring(0, 25))
            .With(x => x.Description, _fixture.Create<string>().PadRight(150, 'x').Substring(0, 150))
            .With(x => x.Tags, new[]
            {
                _fixture.Create<string>().PadRight(15, 'x').Substring(0, 15),
                _fixture.Create<string>().PadRight(10, 'x').Substring(0, 10),
                _fixture.Create<string>().PadRight(12, 'x').Substring(0, 12)
            })
            .With(x => x.Image, _fixture.Create<string>().PadRight(20, 'x').Substring(0, 20) + ".jpg")
            .Create();

        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await _client.PutAsJsonAsync($"api/v1/news/{newId}", request);
        var responseData = await response.Content.ReadFromJsonAsync<BaseSuccessResponse>();

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(responseData.Success);
        Assert.Equal(200, responseData.StatusCode);
    }

    [Fact]
    public async Task Delete_DELETE_ReturnsOkResponse()
    {
        //Arrange
        var newId = new Guid("b5c6d7e8-f9a0-1234-bcde-f56789012345");

        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await _client.DeleteAsync($"api/v1/news/{newId}");
        var responseData = await response.Content.ReadFromJsonAsync<BaseSuccessResponse>();

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(responseData.Success);
        Assert.Equal(200, responseData.StatusCode);
    }

    [Fact]
    public async Task Get_GET_ReturnsOkResponse()
    {
        //Arrange
        var page = 1;
        var perPage = 5;

        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await _client.GetAsync($"api/v1/news?page={page}&perPage={perPage}");
        var responseData = await response.Content.ReadFromJsonAsync<CustomSuccessResponse<PageableResponse<List<GetNewsOutDto>>>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseData);
        Assert.True(responseData.Success);
        Assert.NotNull(responseData.Data);
    }

    [Fact]
    public async Task Find_GET_ReturnsOkResponse()
    {
        //Arrange
        var page = 1;
        var perPage = 5;
        var keywords = "results";

        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await _client.GetAsync($"api/v1/news?keywords={Uri.EscapeDataString(keywords)}&page={page}&perPage={perPage}");
        var responseData = await response.Content.ReadFromJsonAsync<CustomSuccessResponse<PageableResponse<List<GetNewsOutDto>>>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseData);
        Assert.True(responseData.Success);
        Assert.NotNull(responseData.Data);
    }

    [Fact]
    public async Task GetUser_GET_ReturnsOkResponse()
    {
        //Arrange
        var page = 1;
        var perPage = 5;
        var userId = await GetUserIdAsync();

        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Art
        var response = await _client.GetAsync($"api/v1/news/userId/{userId}?page={page}&perPage={perPage}");
        var responseData = await response.Content.ReadFromJsonAsync<CustomSuccessResponse<PageableResponse<List<GetNewsOutDto>>>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseData);
        Assert.True(responseData.Success);
        Assert.NotNull(responseData.Data);
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
