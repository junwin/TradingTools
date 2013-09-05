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
            driver.AddProductDirect("Product.txt");
            List<KaiTrade.Interfaces.IProduct> products = driver.Facade.GetProductManager().GetProducts("KTACQG", "", "");
            Assert.AreEqual(products.Count,1);
        }
    }
}
