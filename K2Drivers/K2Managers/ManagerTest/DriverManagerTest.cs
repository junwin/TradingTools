using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using K2ServiceInterface;

namespace ManagerTest
{
    [TestClass]
    public class DriverManagerTest
    {
        [TestMethod]
        public void LoadDriverTest()
        {
            //test we can create an instance
            IDriverManager mgr = K2Managers.DriverManager.Instance();
            Assert.IsNotNull(mgr);

            // Dynamically load the simulator
            //path = "C:\\Users\\junwin\\Documents\\GitHub\\TradingTools\\build\\bin\\ManagerTest.dll"
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            path = System.IO.Path.GetDirectoryName(path);
            path += @"\KTASimulator.dll";
            mgr.DynamicLoad( path);

            // Should be oe driver
            Assert.AreEqual(1, mgr.GetDrivers().Count);

            // Create instance of the simulator driver
            IDriver driver = mgr.GetDriver("KTSIM");

            //mgr.AddDriverDefinition

            Assert.IsNotNull(driver);



        }
    }
}
