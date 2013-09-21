using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DriverBaseTest
{
    [TestClass]
    public class ProductTest
    {
        [TestMethod]
        public void LoadFileTest()
        {
            DriverBase.DriverBase driver = new DriverBase.DriverBase();
            driver.AddProductDirect("SimProduct.txt");
            List<KaiTrade.Interfaces.IProduct> products = driver.Facade.GetProductManager().GetProducts("KTACQG", "", "");
            Assert.AreEqual(products.Count,0);
            products = driver.Facade.GetProductManager().GetProducts("KTASIM", "", "");
            Assert.AreEqual(products.Count, 3);
        }
    }
}
