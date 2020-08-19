using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            CreateMap<BaseControllerTests.TestEntity, BaseControllerTests.TestDto>();
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

            var mapper = new Mapper(new MapperConfiguration(autoMapper =>
            {
                autoMapper.AddProfile(typeof(TestProfile));
            }));

            _testDbContext = new TestDbContext(ContextOptions);
            _testBaseController = new TestBaseController(_testDbContext, mapper);

            using var setupDbContext = new TestDbContext(ContextOptions);
            SetupDatabase(setupDbContext);
        }

        [TestMethod]
        public async Task GetById_Should_Return_Entity()
        {
            // Arrange
            var testEntityId = Guid.NewGuid();

            var testEntity = new TestEntity
            {
                Id = testEntityId,
                TestProperty = "Test"
            };

            SeedDatabase(testEntity);

            // Act
            var result = await _testBaseController.GetByIdTest(testEntityId);

            // Assert
            Assert.AreEqual(testEntity.Id, result.Value.Id);
            Assert.AreEqual(testEntity.TestProperty, result.Value.TestProperty);
        }

        [TestMethod]
        public async Task DeleteByIdTest_Should_Delete_Entity()
        {
            // Arrange
            var testEntity1 = new TestEntity
            {
                Id = Guid.NewGuid()
            };

            var testEntity2 = new TestEntity
            {
                Id = Guid.NewGuid()
            };

            SeedDatabase(new List<TestEntity> { testEntity1, testEntity2 });

            using (var testDbContextBeforeDelete = new TestDbContext(ContextOptions))
            {
                Assert.AreEqual(2, testDbContextBeforeDelete.TestEntities.Count());
            }

            // Act
            var result = await _testBaseController.DeleteByIdTest(testEntity1.Id);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);

            using var testDbContext = new TestDbContext(ContextOptions);
            Assert.AreEqual(1, testDbContext.TestEntities.Count());

            var remainingEntity = testDbContext.TestEntities.First();
            Assert.AreEqual(testEntity2.Id, remainingEntity.Id);
        }

        [TestMethod]
        public async Task PutById_Should_Update_Entity()
        {
            // Arrange
            var testEntity1 = new TestEntity
            {
                Id = Guid.NewGuid()
            };

            var testEntity2 = new TestEntity
            {
                Id = Guid.NewGuid()
            };

            SeedDatabase(new List<TestEntity> { testEntity1, testEntity2 });

            var testSaveDto = new TestSaveDto
            {
                Id = testEntity2.Id,
                TestProperty = "Updated"
            };

            // Act
            var result = await _testBaseController.PutByIdTest(testEntity2.Id, testSaveDto);

            // Assert
            var noContentResult = result as NoContentResult;

            Assert.IsNotNull(noContentResult);

            using var testDbContext = new TestDbContext(ContextOptions);
            Assert.AreEqual(2, testDbContext.TestEntities.Count());

            var updatedEntity = testDbContext.TestEntities.Single(t => t.Id == testEntity2.Id);
            var unchangedEntity = testDbContext.TestEntities.Single(t => t.Id == testEntity1.Id);

            Assert.AreEqual(testSaveDto.TestProperty, updatedEntity.TestProperty);
            Assert.IsNull(unchangedEntity.TestProperty);
        }

        [TestMethod]
        public async Task PostById_Should_Create_Entity()
        {
            // Arrange
            var testSaveDto = new TestSaveDto
            {
                TestProperty = "New"
            };

            // Act
            var result = await _testBaseController.PostByIdTest(testSaveDto, nameof(_testBaseController.GetByIdTest));

            // Assert
            var actionResult = result as CreatedAtActionResult;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual(nameof(_testBaseController.GetByIdTest), actionResult.ActionName);
            Assert.IsNull(actionResult.Value);

            var id = (Guid)actionResult.RouteValues["id"];

            Assert.AreEqual(id, actionResult.RouteValues["id"]);

            using var testDbContext = new TestDbContext(ContextOptions);

            Assert.AreEqual(1, testDbContext.TestEntities.Count());
            
            var entity = testDbContext.TestEntities.First();

            Assert.AreEqual(id, entity.Id);
            Assert.AreEqual("New", entity.TestProperty);
        }

        private void SeedDatabase(List<TestEntity> testEntities)
        {
            using var seedTestDbContext = new TestDbContext(ContextOptions);

            seedTestDbContext.TestEntities.AddRange(testEntities);
            seedTestDbContext.SaveChanges();
        }

        private void SeedDatabase(TestEntity testEntity)
        {
            SeedDatabase(new List<TestEntity> { testEntity });
        }

        public class TestBaseController : BaseController<TestDbContext, TestDto, TestSaveDto, TestEntity>
        {
            public TestBaseController(TestDbContext dbContext, IMapper mapper) 
                : base(dbContext, mapper)
            {
            }

            public async Task<IActionResult> DeleteByIdTest(Guid id)
            {
                return await DeleteById(_dbContext.TestEntities, id);
            }

            public async Task<ActionResult<TestDto>> GetByIdTest(Guid id)
            {
                return await GetById(_dbContext.TestEntities, id);
            }

            public async Task<IActionResult> PutByIdTest(Guid id, TestSaveDto testSaveDto)
            {
                return await PutById(id, _dbContext.TestEntities, testSaveDto);
            }

            public async Task<IActionResult> PostByIdTest(TestSaveDto testSaveDto, string getActionName)
            {
                return await PostById(_dbContext.TestEntities, testSaveDto, nameof(GetByIdTest));
            }
        }

        public class TestSaveDto : BaseDto
        {
            public string TestProperty { get; set; }
        }

        public class TestDto : BaseDto
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
