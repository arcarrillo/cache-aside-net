using cacheaside.library.implementations.ef;
using cacheaside.library.interfaces;
using cacheaside.library.tests.model;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace cacheaside.library.tests.implementations.ef
{
    [Collection("EFIntegrationTests")]
    public class EFCacheRepositoryIntegrationTests : EFIntegrationTestsBase
    {

        [Fact]
        public async Task When_GetAllForTheFirstTime_Should_CallStorage()
        {
            // TEAR UP
            var serviceProvider = GetServiceProvider();
            var context = serviceProvider.GetRequiredService<Context>();
            var cacheProvider = serviceProvider.GetRequiredService<ICacheProvider>();
            var randomName = Guid.NewGuid().ToString();

            context.Add<Person>(new Person { Name = randomName, Surname = "test" });
            context.SaveChanges();

            try
            {
                //TEST
                var repository = new EFCacheReaderRepository<Person>(context, cacheProvider, TimeSpan.FromMinutes(1));
                var result = await repository.GetAll();

                result.Should().NotBeEmpty();
                result.First().Name.Should().Be(randomName);
            }
            finally
            {
                //TEAR DOWN
                context.Database.ExecuteSqlRaw("DELETE FROM People");
                cacheProvider?.RemoveByPattern("Person:*");
            }
        }

        [Fact]
        public async Task When_GetAllForMoreThanOnce_Should_CallStorageFirstAndThenCache()
        {
            // TEAR UP
            var serviceProvider = GetServiceProvider();
            var context = serviceProvider.GetRequiredService<Context>();
            var cacheProvider = serviceProvider.GetRequiredService<ICacheProvider>();
            var randomName = Guid.NewGuid().ToString();

            context.Add<Person>(new Person { Name = randomName, Surname = "test" });
            context.SaveChanges();

            try
            {
                //TEST
                var repository = new EFCacheReaderRepository<Person>(context, cacheProvider, TimeSpan.FromMinutes(1));
                var result = await repository.GetAll();

                result.Should().NotBeEmpty();
                result.First().Name.Should().Be(randomName);

                context.Database.ExecuteSqlRaw("DELETE FROM People");

                var resultTwo = await repository.GetAll();

                resultTwo.Should().NotBeEmpty();
                resultTwo.First().Name.Should().Be(randomName);
            }
            finally
            {
                //TEAR DOWN
                context.Database.ExecuteSqlRaw("DELETE FROM People");
                cacheProvider?.RemoveByPattern("Person:*");
            }
        }

        [Fact]
        public async Task When_GetAllWithExpressionForTheFirstTime_Should_CallStorage()
        {
            // TEAR UP
            var serviceProvider = GetServiceProvider();
            var context = serviceProvider.GetRequiredService<Context>();
            var cacheProvider = serviceProvider.GetRequiredService<ICacheProvider>();
            var randomName = Guid.NewGuid().ToString();

            context.Add<Person>(new Person { Name = randomName, Surname = "test1" });
            context.Add<Person>(new Person { Name = randomName, Surname = "test2" });
            context.SaveChanges();

            try
            {
                //TEST
                var repository = new EFCacheReaderRepository<Person>(context, cacheProvider, TimeSpan.FromMinutes(1));
                var result = await repository.GetAll(x => x.Surname == "test1");

                result.Should().NotBeNull();
                result.Should().HaveCount(1);
                result.First().Name.Should().Be(randomName);
                result.First().Surname.Should().Be("test1");

                var resultTwo = await repository.GetAll(x => x.Surname == "test2");

                resultTwo.Should().NotBeNull();
                resultTwo.Should().HaveCount(1);
                resultTwo.First().Name.Should().Be(randomName);
                resultTwo.First().Surname.Should().Be("test2");
            }
            finally
            {
                //TEAR DOWN
                context.Database.ExecuteSqlRaw("DELETE FROM People");
                cacheProvider?.RemoveByPattern("Person:*");
            }
        }

        [Fact]
        public async Task When_GetAllWithExpressionTwice_Should_CallCache()
        {
            // TEAR UP
            var serviceProvider = GetServiceProvider();
            var context = serviceProvider.GetRequiredService<Context>();
            var cacheProvider = serviceProvider.GetRequiredService<ICacheProvider>();
            var randomName = Guid.NewGuid().ToString();

            context.Add<Person>(new Person { Name = randomName, Surname = "test1" });
            context.Add<Person>(new Person { Name = randomName, Surname = "test2" });
            context.SaveChanges();

            try
            {
                //TEST
                var repository = new EFCacheReaderRepository<Person>(context, cacheProvider, TimeSpan.FromMinutes(1));
                var result = await repository.GetAll(x => x.Surname == "test1");

                result.Should().NotBeNull();
                result.Should().HaveCount(1);
                result.First().Name.Should().Be(randomName);
                result.First().Surname.Should().Be("test1");

                context.Database.ExecuteSqlRaw("DELETE FROM People");

                var resultTwo = await repository.GetAll(x => x.Surname == "test1");

                resultTwo.Should().NotBeNull();
                resultTwo.Should().HaveCount(1);
                resultTwo.First().Name.Should().Be(randomName);
                resultTwo.First().Surname.Should().Be("test1");
            }
            finally
            {
                //TEAR DOWN
                context.Database.ExecuteSqlRaw("DELETE FROM People");
                cacheProvider?.RemoveByPattern("Person:*");
            }
        }

        [Fact]
        public async Task When_GetOneTwice_Should_CallCache()
        {
            // TEAR UP
            var serviceProvider = GetServiceProvider();
            var context = serviceProvider.GetRequiredService<Context>();
            var cacheProvider = serviceProvider.GetRequiredService<ICacheProvider>();
            var randomName = Guid.NewGuid().ToString();

            context.Add<Person>(new Person { Name = randomName, Surname = "test1" });
            context.Add<Person>(new Person { Name = randomName, Surname = "test2" });
            context.SaveChanges();

            try
            {
                //TEST
                var repository = new EFCacheReaderRepository<Person>(context, cacheProvider, TimeSpan.FromMinutes(1));
                var result = await repository.GetOne(x => x.Surname == "test1");

                result.Should().NotBeNull();
                result!.Name.Should().Be(randomName);
                result!.Surname.Should().Be("test1");

                context.Database.ExecuteSqlRaw("DELETE FROM People");

                var resultTwo = await repository.GetOne(x => x.Surname == "test1");

                resultTwo.Should().NotBeNull();
                resultTwo!.Name.Should().Be(randomName);
                resultTwo!.Surname.Should().Be("test1");
            }
            finally
            {
                //TEAR DOWN
                context.Database.ExecuteSqlRaw("DELETE FROM People");
                cacheProvider?.RemoveByPattern("Person:*");
            }
        }

        [Fact]
        public async Task When_AddOne_Should_RefreshCache()
        {
            // TEAR UP
            var serviceProvider = GetServiceProvider();
            var context = serviceProvider.GetRequiredService<Context>();
            var cacheProvider = serviceProvider.GetRequiredService<ICacheProvider>();
            var randomName = Guid.NewGuid().ToString();

            try
            {
                //TEST
                var readerRepository = new EFCacheReaderRepository<Person>(context, cacheProvider, TimeSpan.FromMinutes(1));
                var writerRepository = new EFCacheWriterRepository<Person>(context, cacheProvider);

                var result = await readerRepository.GetAll(x => x.Surname == "test1");

                result.Should().NotBeNull();
                result.Should().BeEmpty();

                writerRepository.Add(new Person { Name = randomName, Surname = "test1" });

                var resultTwo = await readerRepository.GetAll(x => x.Surname == "test1");

                resultTwo.Should().NotBeNull();
                resultTwo.First().Name.Should().Be(randomName);
                resultTwo.First().Surname.Should().Be("test1");
            }
            finally
            {
                //TEAR DOWN
                context.Database.ExecuteSqlRaw("DELETE FROM People");
                cacheProvider?.RemoveByPattern("Person:*");
            }
        }

        [Fact]
        public async Task When_UpdateOne_Should_RefreshCache()
        {
            // TEAR UP
            var serviceProvider = GetServiceProvider();
            var context = serviceProvider.GetRequiredService<Context>();
            var cacheProvider = serviceProvider.GetRequiredService<ICacheProvider>();
            var randomName = Guid.NewGuid().ToString();
            var updatedRandomName = Guid.NewGuid().ToString();

            context.Add<Person>(new Person { Name = randomName, Surname = "test1" });
            context.SaveChanges();

            try
            {
                //TEST
                var readerRepository = new EFCacheReaderRepository<Person>(context, cacheProvider, TimeSpan.FromMinutes(1));
                var writerRepository = new EFCacheWriterRepository<Person>(context, cacheProvider);

                var result = await readerRepository.GetOne(x => x.Surname == "test1");

                result.Should().NotBeNull();
                result!.Name.Should().Be(randomName);

                result!.Name = updatedRandomName;

                writerRepository.Update(result);

                var resultTwo = await readerRepository.GetOne(x => x.Surname == "test1");

                resultTwo.Should().NotBeNull();
                resultTwo!.Name.Should().Be(updatedRandomName);
                resultTwo!.Surname.Should().Be("test1");
            }
            finally
            {
                //TEAR DOWN
                context.Database.ExecuteSqlRaw("DELETE FROM People");
                cacheProvider?.RemoveByPattern("Person:*");
            }
        }

        [Fact]
        public async Task When_RemoveOne_Should_RefreshCache()
        {
            // TEAR UP
            var serviceProvider = GetServiceProvider();
            var context = serviceProvider.GetRequiredService<Context>();
            var cacheProvider = serviceProvider.GetRequiredService<ICacheProvider>();
            var randomName = Guid.NewGuid().ToString();

            context.Add<Person>(new Person { Name = randomName, Surname = "test1" });
            context.SaveChanges();

            try
            {
                //TEST
                var readerRepository = new EFCacheReaderRepository<Person>(context, cacheProvider, TimeSpan.FromMinutes(1));
                var writerRepository = new EFCacheWriterRepository<Person>(context, cacheProvider);

                var result = await readerRepository.GetAll(x => x.Surname == "test1");

                result.Should().NotBeNull();
                result.Should().HaveCount(1);

                writerRepository.Remove(result.First());

                var resultTwo = await readerRepository.GetAll(x => x.Surname == "test1");

                resultTwo.Should().NotBeNull();
                resultTwo.Should().BeEmpty();
            }
            finally
            {
                //TEAR DOWN
                context.Database.ExecuteSqlRaw("DELETE FROM People");
                cacheProvider?.RemoveByPattern("Person:*");
            }
        }

        [Fact]
        public async Task When_ActsInTransaction_Should_RefreshCacheOnCommit()
        {
            // TEAR UP
            var serviceProvider = GetServiceProvider();
            var context = serviceProvider.GetRequiredService<Context>();
            var cacheProvider = serviceProvider.GetRequiredService<ICacheProvider>();
            var randomName = Guid.NewGuid().ToString();

            context.Add<Person>(new Person { Name = randomName, Surname = "test1" });
            context.SaveChanges();

            try
            {
                //TEST
                var readerRepository = new EFCacheReaderRepository<Person>(context, cacheProvider, TimeSpan.FromMinutes(1));
                var writerRepository = new EFCacheWriterRepository<Person>(context, cacheProvider);

                using (var transaction = writerRepository.BeginTransaction())
                {
                    var result = await readerRepository.GetAll(x => x.Surname == "test1");

                    result.Should().NotBeNull();
                    result.Should().HaveCount(1);

                    writerRepository.Remove(result.First());

                    var resultTwo = await readerRepository.GetAll(x => x.Surname == "test1");

                    resultTwo.Should().NotBeNull();
                    resultTwo.Should().HaveCount(1);

                    transaction.Commit();

                    var resultThree = await readerRepository.GetAll(x => x.Surname == "test1");

                    resultThree.Should().NotBeNull();
                    resultThree.Should().BeEmpty();
                }
            }
            finally
            {
                //TEAR DOWN
                context.Database.ExecuteSqlRaw("DELETE FROM People");
                cacheProvider?.RemoveByPattern("Person:*");
            }
        }

    }
}