using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Caching;
using ECommerce.Infrastructure.Email;
using ECommerce.Infrastructure.Messaging;

using ECommerce.Application.Services.Implementations;
using ECommerce.Application.Services.Interfaces;
using ECommerce.Domain.Repositories;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.Repositories;



namespace ECommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        #region Add Database Context Based on Provider

        var provider = configuration["DatabaseProvider"] ?? "MySQL";

        if (string.Equals(provider, "SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            var conn = configuration.GetConnectionString("SqlServer");
            services.AddDbContext<AppDbContext, SqlServerDbContext>(options =>
                options.UseSqlServer(conn));
        }
        else if (string.Equals(provider, "MySQL", StringComparison.OrdinalIgnoreCase))
        {
            var conn = configuration.GetConnectionString("MySQL");
            services.AddDbContext<AppDbContext, MySqlDbContext>(options =>
                options.UseMySql(conn, ServerVersion.AutoDetect(conn)));
        }
        else if (string.Equals(provider, "PostgreSQL", StringComparison.OrdinalIgnoreCase))
        {

            var conn = configuration.GetConnectionString("PostgreSQL");
            services.AddDbContext<AppDbContext, PostgresDbContext>(options =>
                options.UseNpgsql(conn));
        }
        else
        {
            throw new InvalidOperationException($"Unsupported provider: {provider}");
        }

        #endregion

        #region Redis cache setup

        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnection))
        {
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(redisConnection));

            services.AddSingleton<ICacheService, RedisCacheService>();
        }

        #endregion

        #region RabbitMQ setup

        services.AddSingleton<IMessageQueueService, RabbitMQService>();
        services.AddSingleton<IEmailSenderService, EmailSenderService>();

        #endregion

        return services;
    }
}