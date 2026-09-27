using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoApi.Data;

namespace TodoApi.IntegrationTests.Fixtures;

public class TodoApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName =
        $"TodoTestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Find the existing DbContext registration
            var descriptor =
                services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<TodoDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Register a unique database for this factory
            services.AddDbContext<TodoDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });
        });
    }
}