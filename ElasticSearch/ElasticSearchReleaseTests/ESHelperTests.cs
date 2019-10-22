using Microsoft.VisualStudio.TestTools.UnitTesting;
using ElasticSearchRelease;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchRelease.Tests
{
    [TestClass()]
    public class ESHelperTests
    {
        [TestMethod()]
        public void GetMappingTest()
        {
            ESHelper.GetMapping();
        }

        [TestMethod()]
        public void SearchTest()
        {
            ESHelper.Search();
        }

        [TestMethod()]
        public void Search1Test()
        {
            ESHelper.Search1();
        }

        [TestMethod()]
        public void CreateIndexTest()
        {
            ESHelper.CreateIndex();
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateIndexDataTest()
        {
            ESHelper.CreateIndexData();
            Assert.Fail();
        }
    }
}