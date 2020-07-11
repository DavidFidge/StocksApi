using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using StocksApi.Controllers;
using StocksApi.Model;
using StocksApi.Service.Tests;

namespace StocksApi.Tests
{
    [TestClass]
    public class BaseControllerTests : BaseTest<BaseControllerTests.TestBaseController>
    {
        private TestContext _testContext;
        private TestBaseController _testBaseController;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            _testContext = new TestContext();

            _testBaseController = new TestBaseController(_testContext, Substitute.For<IMapper>());
        }

        [TestMethod]
        public async Task GetById_Should_Return_Entity()
        {
            // Arrange
            var testEntities = new List<TestEntity>();

            var testEntity = new TestEntity
            {
                Id = Guid.NewGuid()
            };

            testEntities.Add(testEntity);

            var dbSet = Substitute.For<DbSet<TestEntity>, IQueryable<TestEntity>>()
                .Initialize(testEntities);

            _testContext.TestEntities = dbSet;

            // Act
            var result = await _testBaseController.GetByIdTest(testEntity.Id);

            // Assert
            Assert.AreEqual(testEntity, result.Value);
        }

        public class TestBaseController : BaseController<TestContext, TestSaveDto, TestEntity>
        {
            public TestBaseController(TestContext dbContext, IMapper mapper) 
                : base(dbContext, mapper)
            {
            }

            public async Task<ActionResult<TestEntity>> DeleteByIdTest(Guid id)
            {
                return await DeleteById(_dbContext.TestEntities, id);
            }

            public async Task<ActionResult<TestEntity>> GetByIdTest(Guid id)
            {
                return await GetById(_dbContext.TestEntities, id);
            }

            public async Task<IActionResult> PutByIdTest(Guid id, TestSaveDto testSaveDto)
            {
                return await PutById(id, _dbContext.TestEntities, testSaveDto);
            }

            public async Task<ActionResult<TestEntity>> PostByIdTest(TestSaveDto testSaveDto, string getActionName)
            {
                return await PostById(_dbContext.TestEntities, testSaveDto, nameof(GetByIdTest));
            }
        }

        public class TestSaveDto : BaseSaveDto
        {
            public string TestProperty { get; set; }
        }

        public class TestEntity : Entity
        {
            public string TestProperty { get; set; }
        }

        public class TestContext : DbContext
        {
            public virtual DbSet<TestEntity> TestEntities { get; set; }
        }
    }
}
