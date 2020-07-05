using System;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using StocksApi.Controllers;
using StocksApi.Model;
using StocksApi.Service.Tests;

namespace StocksApi.Tests
{
    [TestClass]
    public class BaseControllerTests : BaseTest<BaseControllerTests.TestBaseController>
    {

        [TestInitialize]
        public void TestMethod1()
        {
            base.Setup();
        }

        [TestMethod]
        public void GetById_Should_Return()
        {
        }

        public class TestBaseController : BaseController<TestContext, TestSaveDto, TestEntity>
        {
            public TestBaseController(TestContext dbContext, IMapper mapper) 
                : base(dbContext, mapper)
            {
            }

            public ActionResult<TestEntity> DeleteByIdTest(Guid id)
            {
                return DeleteById(_dbContext.TestEntities, id).Result;
            }

            public ActionResult<TestEntity> GetByIdTest(Guid id)
            {
                return GetById(_dbContext.TestEntities, id).Result;
            }

            public IActionResult PutByIdTest(Guid id, TestSaveDto testSaveDto)
            {
                return PutById(id, _dbContext.TestEntities, testSaveDto).Result;
            }

            public ActionResult<TestEntity> PostByIdTest(TestSaveDto testSaveDto, string getActionName)
            {
                return PostById(_dbContext.TestEntities, testSaveDto, nameof(GetByIdTest)).Result;
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
