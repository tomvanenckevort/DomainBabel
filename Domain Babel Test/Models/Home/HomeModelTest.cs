using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainBabel.Test
{
    [TestClass]
    public class HomeModelTest
    {
        private HomeModel model;

        [TestInitialize]
        public void Init()
        {
            this.model = new HomeModel();
        }

        [TestMethod]
        public void GetModel()
        {
            Assert.IsNull(this.model.SearchText);
        }
    }
}
