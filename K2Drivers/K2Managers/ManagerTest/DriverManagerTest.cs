using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using K2ServiceInterface;

namespace ManagerTest
{
    [TestClass]
    public class DriverManagerTest
    {
        [TestMethod]
        public void BasicTest()
        {
            //test we can create an instance
            IDriverManager mgr = K2Managers.DriverManager.Instance();
            Assert.IsNotNull(mgr);

            // Dynamically load the simulator
            mgr.DynamicLoad( @"C:\Users\John\Documents\GitHub\TradingTools\build\bin\KTASimulator.dll");

            // Should be oe driver
            Assert.AreEqual(1, mgr.GetDrivers().Count);

            // Create instance of the simulator driver
            IDriver driver = mgr.GetDriver("KTSIM");

            //mgr.AddDriverDefinition

            Assert.IsNotNull(driver);



        }
    }
}
