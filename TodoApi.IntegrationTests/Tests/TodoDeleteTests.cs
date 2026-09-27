using System.Net;
using System.Net.Http.Json;
using TodoApi.IntegrationTests.Fixtures;
using TodoApi.Models;

namespace TodoApi.IntegrationTests.Tests;

public class TodoDeleteTests : IClassFixture<TodoApiFactory>
{
    private readonly HttpClient _client;

    public TodoDeleteTests(TodoApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task DeleteTodo_WithExistingId_ReturnsNoContent()
    {
        // Arrange
        var createResponse =
            await _client.PostAsJsonAsync(
                "/api/todos",
                new
                {
                    title = "Todo to delete",
                    isCompleted = false
                });

        var createdTodo =
            await createResponse.Content
                .ReadFromJsonAsync<Todo>();

        Assert.NotNull(createdTodo);

        // Act
        var deleteResponse =
            await _client.DeleteAsync(
                $"/api/todos/{createdTodo.Id}");

        // Assert
        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteTodo_RemovesTodo()
    {
        // Arrange
        var createResponse =
            await _client.PostAsJsonAsync(
                "/api/todos",
                new
                {
                    title = "Todo to remove",
                    isCompleted = false
                });

        var createdTodo =
            await createResponse.Content
                .ReadFromJsonAsync<Todo>();

        Assert.NotNull(createdTodo);

        // Act
        var deleteResponse =
            await _client.DeleteAsync(
                $"/api/todos/{createdTodo.Id}");

        // Assert delete
        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode);

        // Verify deletion
        var getResponse =
            await _client.GetAsync(
                $"/api/todos/{createdTodo.Id}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteTodo_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        const int nonExistingId = 999999;

        // Act
        var response =
            await _client.DeleteAsync(
                $"/api/todos/{nonExistingId}");

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task DeleteTodo_CannotBeRetrievedAfterDeletion()
    {
        // Arrange
        var createResponse =
            await _client.PostAsJsonAsync(
                "/api/todos",
                new
                {
                    title = "Temporary Todo",
                    isCompleted = false
                });

        var createdTodo =
            await createResponse.Content
                .ReadFromJsonAsync<Todo>();

        Assert.NotNull(createdTodo);

        var id = createdTodo.Id;

        // Verify it exists first
        var beforeDeleteResponse =
            await _client.GetAsync(
                $"/api/todos/{id}");

        Assert.Equal(
            HttpStatusCode.OK,
            beforeDeleteResponse.StatusCode);

        // Act
        var deleteResponse =
            await _client.DeleteAsync(
                $"/api/todos/{id}");

        // Assert deletion
        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode);

        // Verify it no longer exists
        var afterDeleteResponse =
            await _client.GetAsync(
                $"/api/todos/{id}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            afterDeleteResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteTodo_Twice_ReturnsNotFoundOnSecondAttempt()
    {
        // Arrange
        var createResponse =
            await _client.PostAsJsonAsync(
                "/api/todos",
                new
                {
                    title = "Delete twice test",
                    isCompleted = false
                });

        var createdTodo =
            await createResponse.Content
                .ReadFromJsonAsync<Todo>();

        Assert.NotNull(createdTodo);

        var id = createdTodo.Id;

        // Act - first delete
        var firstDeleteResponse =
            await _client.DeleteAsync(
                $"/api/todos/{id}");

        // Assert
        Assert.Equal(
            HttpStatusCode.NoContent,
            firstDeleteResponse.StatusCode);

        // Act - second delete
        var secondDeleteResponse =
            await _client.DeleteAsync(
                $"/api/todos/{id}");

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            secondDeleteResponse.StatusCode);
    }
}