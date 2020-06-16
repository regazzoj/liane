using System;
using System.Threading.Tasks;
using Liane.Service.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Liane.Test
{
    [TestFixture]
    public sealed class RoutingTest
    {
        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public async Task ShouldGetAMigratedDbConnection()
        {
            // using var connection = await "";
            // var count = await connection.QuerySingleAsync<int>("SELECT count(*) FROM test_global_table");
            // Assert.AreEqual(3, count);
            var service = new ExampleServiceImpl(new Mock<ILogger<ExampleServiceImpl>>().Object);
            var result = await service.ExampleMethod();
            //Assert.IsFalse(result);
            Console.WriteLine(result);
        }
    }
}