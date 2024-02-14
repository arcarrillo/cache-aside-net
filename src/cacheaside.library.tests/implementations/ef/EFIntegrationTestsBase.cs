using cacheaside.library.implementations.redis;
using cacheaside.library.interfaces;
using cacheaside.library.tests.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cacheaside.library.tests.implementations.ef;

public abstract class EFIntegrationTestsBase
{
    ServiceProvider? _serviceProvider;

    protected ServiceProvider GetServiceProvider()
    {
        if (_serviceProvider == null)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ICacheProvider>(sp =>
            {
                return new RedisCacheProvider(new RedisConnectionConfiguration("localhost:6379", 60));
            });

            serviceCollection.AddScoped<Context>(sp =>
            {
                return new Context();
            });

            _serviceProvider = serviceCollection.BuildServiceProvider();

            var ctx = _serviceProvider.GetRequiredService<Context>();
            ctx.Database.EnsureCreated();
        }

        return _serviceProvider;
    }
}
