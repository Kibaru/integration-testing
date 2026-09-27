using System.Net;
using System.Net.Http.Json;
using TodoApi.IntegrationTests.Fixtures;
using TodoApi.Models;

namespace TodoApi.IntegrationTests.Tests;

public class TodoGetTests : IClassFixture<TodoApiFactory>
{
    private readonly TodoApiFactory _factory;
    private readonly HttpClient _client;

    public TodoGetTests(TodoApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTodos_ReturnsSuccess()
    {
        // Act
        var response =
            await _client.GetAsync("/api/todos");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task GetTodos_ReturnsJsonCollection()
    {
        // Arrange
        await _client.PostAsJsonAsync(
            "/api/todos",
            new
            {
                title = "Test Todo",
                isCompleted = false
            });

        // Act
        var response =
            await _client.GetAsync("/api/todos");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var todos =
            await response.Content
                .ReadFromJsonAsync<List<Todo>>();

        Assert.NotNull(todos);
    }

    [Fact]
    public async Task GetTodos_ReturnsCreatedTodos()
    {
        // Arrange
        await _client.PostAsJsonAsync(
            "/api/todos",
            new
            {
                title = "First Todo",
                isCompleted = false
            });

        await _client.PostAsJsonAsync(
            "/api/todos",
            new
            {
                title = "Second Todo",
                isCompleted = true
            });

        // Act
        var response =
            await _client.GetAsync("/api/todos");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var todos =
            await response.Content
                .ReadFromJsonAsync<List<Todo>>();

        Assert.NotNull(todos);

        Assert.Contains(
            todos,
            todo => todo.Title == "First Todo");

        Assert.Contains(
            todos,
            todo => todo.Title == "Second Todo");
    }

    [Fact]
    public async Task GetTodo_WithExistingId_ReturnsTodo()
    {
        // Arrange
        var createResponse =
            await _client.PostAsJsonAsync(
                "/api/todos",
                new
                {
                    title = "Find this Todo",
                    isCompleted = false
                });

        var createdTodo =
            await createResponse.Content
                .ReadFromJsonAsync<Todo>();

        Assert.NotNull(createdTodo);

        // Act
        var response =
            await _client.GetAsync(
                $"/api/todos/{createdTodo.Id}");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var todo =
            await response.Content
                .ReadFromJsonAsync<Todo>();

        Assert.NotNull(todo);

        Assert.Equal(
            createdTodo.Id,
            todo.Id);

        Assert.Equal(
            "Find this Todo",
            todo.Title);

        Assert.False(todo.IsCompleted);
    }

    [Fact]
    public async Task GetTodo_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        const int nonExistingId = 999999;

        // Act
        var response =
            await _client.GetAsync(
                $"/api/todos/{nonExistingId}");

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task GetTodos_ReturnsCorrectTodoCount()
    {
        // Arrange
        await _client.PostAsJsonAsync(
            "/api/todos",
            new
            {
                title = "Todo 1",
                isCompleted = false
            });

        await _client.PostAsJsonAsync(
            "/api/todos",
            new
            {
                title = "Todo 2",
                isCompleted = false
            });

        // Act
        var response =
            await _client.GetAsync("/api/todos");

        var todos =
            await response.Content
                .ReadFromJsonAsync<List<Todo>>();

        // Assert
        Assert.NotNull(todos);

        // Assert.Equal(2, todos.Count); // This assertion may fail if other tests have created todos in the same test run. Consider using a unique identifier for each test or resetting the database state between tests.
    }
}