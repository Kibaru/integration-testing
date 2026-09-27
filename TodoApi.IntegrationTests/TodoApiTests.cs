using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;
using TodoApi.Models;

namespace TodoApi.IntegrationTests;

public class TodoApiTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TodoApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Todo_Lifecycle_WorksCorrectly()
    {
        // CREATE
        var createResponse = await _client.PostAsJsonAsync(
            "/api/todos",
            new
            {
                title = "Learn integration testing",
                isCompleted = false
            });

        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode);

        var createdTodo =
            await createResponse.Content.ReadFromJsonAsync<Todo>();

        Assert.NotNull(createdTodo);

        var id = createdTodo.Id;

        // GET
        var getResponse =
            await _client.GetAsync($"/api/todos/{id}");

        Assert.Equal(
            HttpStatusCode.OK,
            getResponse.StatusCode);

        // UPDATE
        var updateResponse =
            await _client.PutAsJsonAsync(
                $"/api/todos/{id}",
                new
                {
                    title = "Integration testing mastered",
                    isCompleted = true
                });

        Assert.Equal(
            HttpStatusCode.NoContent,
            updateResponse.StatusCode);

        // GET AGAIN
        var updatedResponse =
            await _client.GetAsync($"/api/todos/{id}");

        var updatedTodo =
            await updatedResponse.Content
                .ReadFromJsonAsync<Todo>();

        Assert.NotNull(updatedTodo);

        Assert.Equal(
            "Integration testing mastered",
            updatedTodo.Title);

        Assert.True(updatedTodo.IsCompleted);

        // DELETE
        var deleteResponse =
            await _client.DeleteAsync(
                $"/api/todos/{id}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode);

        // VERIFY DELETED
        var deletedResponse =
            await _client.GetAsync($"/api/todos/{id}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            deletedResponse.StatusCode);
    }
}