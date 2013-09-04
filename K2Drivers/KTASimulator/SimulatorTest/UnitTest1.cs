using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SimulatorTest
{
    [TestClass]
    public class CannedDataTest
    {
        KTASimulator.KTASimulator _driver = null;

        public CannedDataTest()
        {
            _driver = new KTASimulator.KTASimulator();
        }

        [TestMethod]
        public void SetupDriver()
        {
            _driver.Start("");
            //_driver.st
        }
    }
}
