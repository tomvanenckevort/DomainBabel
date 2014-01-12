using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainBabel.Test
{
    [TestClass]
    public class HomeControllerTests
    {
        private HomeController controller;

        [TestInitialize]
        public void Init()
        {
            this.controller = new HomeController();
        }

        [TestMethod]
        public void GetIndex()
        {
            var result = this.controller.Index() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void GetIndexModel()
        {
            var result = this.controller.Index() as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(HomeModel));
        }
    }
}
