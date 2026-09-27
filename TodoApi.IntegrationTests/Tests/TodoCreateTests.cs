using System.Net;
using System.Net.Http.Json;
using TodoApi.IntegrationTests.Fixtures;
using TodoApi.Models;

namespace TodoApi.IntegrationTests.Tests;

public class TodoCreateTests : IClassFixture<TodoApiFactory>
{
    private readonly HttpClient _client;

    public TodoCreateTests(TodoApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTodo_WithValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new
        {
            title = "Learn integration testing",
            isCompleted = false
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/todos",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateTodo_WithValidRequest_ReturnsCreatedTodo()
    {
        // Arrange
        var request = new
        {
            title = "Learn ASP.NET Core",
            isCompleted = false
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/todos",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var createdTodo =
            await response.Content.ReadFromJsonAsync<Todo>();

        Assert.NotNull(createdTodo);

        Assert.True(createdTodo.Id > 0);

        Assert.Equal(
            "Learn ASP.NET Core",
            createdTodo.Title);

        Assert.False(createdTodo.IsCompleted);
    }

    [Fact]
    public async Task CreateTodo_WithValidRequest_ReturnsLocationHeader()
    {
        // Arrange
        var request = new
        {
            title = "Test Location Header",
            isCompleted = false
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/todos",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var createdTodo =
            await response.Content.ReadFromJsonAsync<Todo>();

        Assert.NotNull(createdTodo);

        Assert.NotNull(response.Headers.Location);

        Assert.EndsWith(
            $"/api/Todos/{createdTodo.Id}",
            response.Headers.Location.ToString());
    }

    [Fact]
    public async Task CreateTodo_WithValidRequest_PersistsTodo()
    {
        // Arrange
        var request = new
        {
            title = "Todo that should be persisted",
            isCompleted = false
        };

        // Act
        var createResponse =
            await _client.PostAsJsonAsync(
                "/api/todos",
                request);

        // Assert create
        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode);

        var createdTodo =
            await createResponse.Content
                .ReadFromJsonAsync<Todo>();

        Assert.NotNull(createdTodo);

        // Act - retrieve the Todo
        var getResponse =
            await _client.GetAsync(
                $"/api/todos/{createdTodo.Id}");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            getResponse.StatusCode);

        var persistedTodo =
            await getResponse.Content
                .ReadFromJsonAsync<Todo>();

        Assert.NotNull(persistedTodo);

        Assert.Equal(
            createdTodo.Id,
            persistedTodo.Id);

        Assert.Equal(
            "Todo that should be persisted",
            persistedTodo.Title);

        Assert.False(
            persistedTodo.IsCompleted);
    }

    [Fact]
    public async Task CreateTodo_CanCreateMultipleTodos()
    {
        // Arrange
        var firstRequest = new
        {
            title = "First Todo",
            isCompleted = false
        };

        var secondRequest = new
        {
            title = "Second Todo",
            isCompleted = true
        };

        // Act
        var firstResponse =
            await _client.PostAsJsonAsync(
                "/api/todos",
                firstRequest);

        var secondResponse =
            await _client.PostAsJsonAsync(
                "/api/todos",
                secondRequest);

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            firstResponse.StatusCode);

        Assert.Equal(
            HttpStatusCode.Created,
            secondResponse.StatusCode);

        var firstTodo =
            await firstResponse.Content
                .ReadFromJsonAsync<Todo>();

        var secondTodo =
            await secondResponse.Content
                .ReadFromJsonAsync<Todo>();

        Assert.NotNull(firstTodo);
        Assert.NotNull(secondTodo);

        Assert.NotEqual(
            firstTodo.Id,
            secondTodo.Id);
    }
}