using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KaiTrade.Interfaces;
using K2ServiceInterface;

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


        /// <summary>
        /// Test that the price source function works 
        /// </summary>
        [TestMethod]
        public void TestPriceFileStart()
        {
            KTASimulator.FilePriceSource priceSrc = new KTASimulator.FilePriceSource(_driver);

            priceSrc.PriceUpdate += new PriceUpdate(this.PriceUpdate);

            priceSrc.Start(@"TestData\AAPL_data.csv");

            ////List<KaiTrade.Interfaces.IProduct> products = _driver.Facade.GetProductManager().GetProducts("KTSIM", "", "");
           // Assert.AreEqual(products.Count, 1);
        }

        /// <summary>
        /// Tests that price files are loaded and run if available when the
        /// sim driver stats - should start DELL & AAPL
        /// </summary>
        [TestMethod]
        public void TestPriceFileStartDriverInit()
        {
            // HPQ will just fill over time an order of 5 will fill 2,1,2
            _driver = new KTASimulator.KTASimulator();
            _driver.Facade.AppPath = @"C:\Users\John\Documents\GitHub\TradingTools\K2Drivers\build\bin\";

            // Gets driver messages (fills, Accounts etc)
            _driver.Message += new KaiTrade.Interfaces.Message(OnMessage);
            (_driver as DriverBase.DriverBase).PriceUpdate += new K2ServiceInterface.PriceUpdate(this.PriceUpdate);

            _driver.Start("");

            // Test that S.DELL is set up as expected
            IProduct product = _driver.Facade.GetProductManager().GetProductMnemonic("S.DELL");
            IPublisher pub = _driver.Facade.CreatePxPub(product);
            if (pub != null)
            {
                // register this publisher with the driver - one per product
                _driver.Register(pub, 0, DateTime.Now.Ticks.ToString());
            }

         
            System.Threading.Thread.Sleep(110000);
             
            List<KaiTrade.Interfaces.IProduct> products = _driver.Facade.GetProductManager().GetProducts("KTSIM", "", "");
            Assert.AreEqual(products.Count, 13);
        }

        void OnMessage(KaiTrade.Interfaces.IMessage message)
        {
            
        }

        public void PriceUpdate(KaiTrade.Interfaces.IPXUpdate pxUpdate)
        {
        }
        
    }
}
