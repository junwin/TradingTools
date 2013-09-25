using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KaiTrade.Interfaces;
using K2DataObjects;
using Newtonsoft.Json;

namespace DriverBaseTest
{
    [TestClass]
    public class DriverDefTest
    {
        [TestMethod]
        public void TestCreateDriverDef()
        {
            IDriverDef def = new DriverDef();
            def.Name = "KTSIM";
            def.LoadPath = "KTASimulator.dll";
            def.LiveMarket = false;
            def.ManualStart = true;
            def.QueueReplaceRequests = true;
            def.AsyncPrices = true;

            string JSONData = JsonConvert.SerializeObject(def);

            IDriverDef def1 = JsonConvert.DeserializeObject<K2DataObjects.DriverDef>(JSONData);

            // make sure this can deserialized
            Assert.IsNotNull(def1);
            


        }
    }
}
