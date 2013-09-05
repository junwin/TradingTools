using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimulatorTest
{
    [TestClass]
    public class PriceTest
    {
        KTASimulator.KTASimulator _driver = null;
        public PriceTest()
        {
            _driver = new KTASimulator.KTASimulator();
        }
        [TestMethod]
        public void SetupDriver()
        {
            _driver.Start("");
            //_driver.st
        }

        [TestMethod]
        public void TestPriceFile()
        {
            KTASimulator.FilePriceSource priceSrc = new KTASimulator.FilePriceSource(_driver);

            priceSrc.SetUpProductNoRun(@"TestData\AAPL_data.csv");

            List<KaiTrade.Interfaces.IProduct> products = _driver.Facade.GetProductManager().GetProducts("KTSIM", "", "");
            Assert.AreEqual(products.Count, 1);
        }

        [TestMethod]
        public void TestPriceFileStart()
        {
            KTASimulator.FilePriceSource priceSrc = new KTASimulator.FilePriceSource(_driver);

            priceSrc.PriceUpdate += new KTASimulator.PriceUpdate(this.PriceUpdate);

            priceSrc.Start(@"TestData\AAPL_data.csv");

            ////List<KaiTrade.Interfaces.IProduct> products = _driver.Facade.GetProductManager().GetProducts("KTSIM", "", "");
           // Assert.AreEqual(products.Count, 1);
        }

        public void PriceUpdate(KaiTrade.Interfaces.IPXUpdate pxUpdate)
        {
        }
        
    }
}
