using cacheaside.library.implementations.ef;
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

            serviceCollection.AddScoped<IReaderRepository<Person>>(sp =>
            {
                return new EFCacheReaderRepository<Person>(
                    sp.GetRequiredService<Context>(),
                    sp.GetRequiredService<ICacheProvider>(),
                    TimeSpan.FromMinutes(60)
                );
            });
            serviceCollection.AddScoped<IWriterRepository<Person>, EFCacheWriterRepository<Person>>();

            _serviceProvider = serviceCollection.BuildServiceProvider();

            var ctx = _serviceProvider.GetRequiredService<Context>();
            ctx.Database.EnsureCreated();
        }

        return _serviceProvider;
    }
}

public interface IPeopleReaderService
{
    Task<IEnumerable<Person>> GetAllPeopleByName(string name);
}

public class PeopleReaderService(IReaderRepository<Person> repository) : IPeopleReaderService
{

    public async Task<IEnumerable<Person>> GetAllPeopleByName(string name)
    {
        return await repository.GetAll(x => x.Name == name);
    }

}

public interface IPeopleWriterService
{
    bool AddNewPerson(string name, string surname);
}

public class PeopleWriterService(IWriterRepository<Person> repository) : IPeopleWriterService
{
    public bool AddNewPerson(string name, string surname)
    {
        repository.Add(new Person { Name = name, Surname = surname });
        return true;
    }
}