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
    public class TestProfile : Profile
    {
        public TestProfile()
        {
            CreateMap<BaseControllerTests.TestSaveDto, BaseControllerTests.TestEntity>();
        }
    }

    [TestClass]
    public class BaseControllerTests : BaseSqlLiteTest<BaseControllerTests.TestBaseController, BaseControllerTests.TestDbContext>
    {
        private TestDbContext _testDbContext;
        private TestBaseController _testBaseController;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            var mapper = new Mapper(new MapperConfiguration(automapper =>
            {
                automapper.AddProfile(typeof(TestProfile));
                automapper.UseEntityFrameworkCoreModel<TestDbContext>();
            }));

            using (var setupDbContext = new TestDbContext(ContextOptions))
            {
                setupDbContext.Database.EnsureDeleted();
                setupDbContext.Database.EnsureCreated();
            }

            _testDbContext = new TestDbContext(ContextOptions);
            _testBaseController = new TestBaseController(_testDbContext, mapper);
        }


        [TestMethod]
        public async Task GetById_Should_Return_Entity()
        {
            // Arrange
            var testEntityId = Guid.NewGuid();

            using (var seedTestDbContext = new TestDbContext(ContextOptions))
            {
                var testEntity = new TestEntity
                {
                    Id = testEntityId
                };

                seedTestDbContext.TestEntities.Add(testEntity);
                seedTestDbContext.SaveChanges();
            }

            // Act
            var result = await _testBaseController.GetByIdTest(testEntityId);

            // Assert

            using var testDbContext = new TestDbContext(ContextOptions);
            Assert.AreEqual(1, testEntities.Count);
            Assert.AreEqual(testEntity, result.Value);
        }

        [TestMethod]
        public async Task DeleteByIdTest_Should_Delete_Entity()
        {
            // Arrange
            var testEntities = new List<TestEntity>();

            var testEntity = new TestEntity
            {
                Id = Guid.NewGuid()
            };

            testEntities.Add(testEntity);

            var dbSet = Substitute.For<DbSet<TestEntity>, IQueryable<TestEntity>>()
                .Initialize(testEntities)
                .WithRemove(testEntities);

            _testDbContext.TestEntities = dbSet;

            // Act
            var result = await _testBaseController.DeleteByIdTest(testEntity.Id);

            // Assert
            var noContentResult = result as NoContentResult;

            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(0, testEntities.Count);
        }

        [TestMethod]
        public async Task PutById_Should_Update_Entity()
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

            _testDbContext.TestEntities = dbSet;

            var testSaveDto = new TestSaveDto
            {
                Id = testEntity.Id,
                TestProperty = "Updated"
            };

            // Act
            var result = await _testBaseController.PutByIdTest(testEntity.Id, testSaveDto);

            // Assert
            var noContentResult = result as NoContentResult;

            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(1, testEntities.Count);
            Assert.AreEqual("Updated", testEntity.TestProperty);
        }

        [TestMethod]
        public async Task PostById_Should_Create_Entity()
        {
            // Arrange
            var testEntities = new List<TestEntity>();

            var dbSet = Substitute.For<DbSet<TestEntity>, IQueryable<TestEntity>>()
                .Initialize(testEntities)
                .WithAdd(testEntities);

            _testDbContext.TestEntities = dbSet;

            var testSaveDto = new TestSaveDto
            {
                TestProperty = "New"
            };

            // Act
            var result = await _testBaseController.PostByIdTest(testSaveDto, nameof(_testBaseController.GetByIdTest));

            // Assert
            var createdAtActionResult = result.Result as CreatedAtActionResult;

            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(nameof(_testBaseController.GetByIdTest), createdAtActionResult.ActionName);

            var id = (Guid)createdAtActionResult.RouteValues["id"];

            Assert.AreEqual(id, createdAtActionResult.RouteValues["id"]);
            Assert.AreEqual(1, testEntities.Count);
            Assert.AreEqual("New", result.Value.TestProperty);
            Assert.AreEqual(id, result.Value.Id);
        }

        public class TestBaseController : BaseController<TestDbContext, TestSaveDto, TestEntity>
        {
            public TestBaseController(TestDbContext dbContext, IMapper mapper) 
                : base(dbContext, mapper)
            {
            }

            public async Task<IActionResult> DeleteByIdTest(Guid id)
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

        public class TestDbContext : DbContext
        {
            public virtual DbSet<TestEntity> TestEntities { get; set; }

            
            public TestDbContext()
            {
            }

            public TestDbContext(DbContextOptions<TestDbContext> options)
                : base(options)
            {
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<TestEntity>(
                    b =>
                    {
                        b.Property(e => e.Id);
                        b.HasKey(e => e.Id);
                        b.Property(e => e.TestProperty);
                    });
            }
        }
    }
}
