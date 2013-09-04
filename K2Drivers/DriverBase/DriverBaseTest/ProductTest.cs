using System;
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
        }
    }
}
