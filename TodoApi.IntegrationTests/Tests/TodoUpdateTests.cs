using System.Net;
using System.Net.Http.Json;
using TodoApi.IntegrationTests.Fixtures;
using TodoApi.Models;

namespace TodoApi.IntegrationTests.Tests;

public class TodoUpdateTests : IClassFixture<TodoApiFactory>
{
    private readonly HttpClient _client;

    public TodoUpdateTests(TodoApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UpdateTodo_WithExistingId_ReturnsNoContent()
    {
        // Arrange
        var createResponse =
            await _client.PostAsJsonAsync(
                "/api/todos",
                new
                {
                    title = "Original title",
                    isCompleted = false
                });

        var createdTodo =
            await createResponse.Content
                .ReadFromJsonAsync<Todo>();

        Assert.NotNull(createdTodo);

        var updateRequest = new
        {
            title = "Updated title",
            isCompleted = true
        };

        // Act
        var updateResponse =
            await _client.PutAsJsonAsync(
                $"/api/todos/{createdTodo.Id}",
                updateRequest);

        // Assert
        Assert.Equal(
            HttpStatusCode.NoContent,
            updateResponse.StatusCode);
    }

    [Fact]
    public async Task UpdateTodo_ChangesTitle()
    {
        // Arrange
        var createResponse =
            await _client.PostAsJsonAsync(
                "/api/todos",
                new
                {
                    title = "Original title",
                    isCompleted = false
                });

        var createdTodo =
            await createResponse.Content
                .ReadFromJsonAsync<Todo>();

        Assert.NotNull(createdTodo);

        // Act
        var updateResponse =
            await _client.PutAsJsonAsync(
                $"/api/todos/{createdTodo.Id}",
                new
                {
                    title = "Updated title",
                    isCompleted = false
                });

        // Assert update request
        Assert.Equal(
            HttpStatusCode.NoContent,
            updateResponse.StatusCode);

        // Verify persistence
        var getResponse =
            await _client.GetAsync(
                $"/api/todos/{createdTodo.Id}");

        var updatedTodo =
            await getResponse.Content
                .ReadFromJsonAsync<Todo>();

        Assert.NotNull(updatedTodo);

        Assert.Equal(
            "Updated title",
            updatedTodo.Title);
    }

    [Fact]
    public async Task UpdateTodo_ChangesCompletionStatus()
    {
        // Arrange
        var createResponse =
            await _client.PostAsJsonAsync(
                "/api/todos",
                new
                {
                    title = "Complete this Todo",
                    isCompleted = false
                });

        var createdTodo =
            await createResponse.Content
                .ReadFromJsonAsync<Todo>();

        Assert.NotNull(createdTodo);

        // Act
        var updateResponse =
            await _client.PutAsJsonAsync(
                $"/api/todos/{createdTodo.Id}",
                new
                {
                    title = "Complete this Todo",
                    isCompleted = true
                });

        // Assert
        Assert.Equal(
            HttpStatusCode.NoContent,
            updateResponse.StatusCode);

        // Verify persistence
        var getResponse =
            await _client.GetAsync(
                $"/api/todos/{createdTodo.Id}");

        var updatedTodo =
            await getResponse.Content
                .ReadFromJsonAsync<Todo>();

        Assert.NotNull(updatedTodo);

        Assert.True(
            updatedTodo.IsCompleted);
    }

    [Fact]
    public async Task UpdateTodo_PersistsAllChanges()
    {
        // Arrange
        var createResponse =
            await _client.PostAsJsonAsync(
                "/api/todos",
                new
                {
                    title = "Original title",
                    isCompleted = false
                });

        var createdTodo =
            await createResponse.Content
                .ReadFromJsonAsync<Todo>();

        Assert.NotNull(createdTodo);

        // Act
        var updateResponse =
            await _client.PutAsJsonAsync(
                $"/api/todos/{createdTodo.Id}",
                new
                {
                    title = "Completely updated title",
                    isCompleted = true
                });

        // Assert
        Assert.Equal(
            HttpStatusCode.NoContent,
            updateResponse.StatusCode);

        // Retrieve from API
        var getResponse =
            await _client.GetAsync(
                $"/api/todos/{createdTodo.Id}");

        Assert.Equal(
            HttpStatusCode.OK,
            getResponse.StatusCode);

        var updatedTodo =
            await getResponse.Content
                .ReadFromJsonAsync<Todo>();

        Assert.NotNull(updatedTodo);

        Assert.Equal(
            createdTodo.Id,
            updatedTodo.Id);

        Assert.Equal(
            "Completely updated title",
            updatedTodo.Title);

        Assert.True(
            updatedTodo.IsCompleted);
    }

    [Fact]
    public async Task UpdateTodo_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        const int nonExistingId = 999999;

        var updateRequest = new
        {
            title = "This should not exist",
            isCompleted = true
        };

        // Act
        var response =
            await _client.PutAsJsonAsync(
                $"/api/todos/{nonExistingId}",
                updateRequest);

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateTodo_DoesNotCreateNewTodo_WhenIdDoesNotExist()
    {
        // Arrange
        const int nonExistingId = 999999;

        var updateRequest = new
        {
            title = "Should not be created",
            isCompleted = true
        };

        // Act
        var updateResponse =
            await _client.PutAsJsonAsync(
                $"/api/todos/{nonExistingId}",
                updateRequest);

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            updateResponse.StatusCode);

        // Verify it wasn't accidentally created
        var getResponse =
            await _client.GetAsync(
                $"/api/todos/{nonExistingId}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            getResponse.StatusCode);
    }
}