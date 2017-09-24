using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPMAsignmentHandling;
using WPMAsignmentHandling.Controllers;
using System.Web.Mvc;

namespace ControllerTest
{
    [TestClass]
    public class ControllerTest
    {
        [TestMethod]
        public void IndexShouldReturnDefaultView()
        {
            var Controller = new HomeController();
            ViewResult result = Controller.Index();
            Assert.IsNotNull(result);
            //Assert.IsNull(result.ViewName);
        }
    }
}
