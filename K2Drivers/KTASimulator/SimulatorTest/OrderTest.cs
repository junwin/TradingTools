using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace SimulatorTest
{
    

    [TestClass]
    public class OrderTest
    {
        KTASimulator.KTASimulator _driver = null;
        static int s_Count = 0;

        [TestMethod]
        public void CreateDriverInstance()
        {
            _driver = new KTASimulator.KTASimulator();
            Assert.IsNotNull(_driver);
        }
        [TestMethod]
        public void SubmitOrder()
        {
            _driver.Start("");

            K2DataObjects.SubmitRequest nos = new K2DataObjects.SubmitRequest();
            nos.Account = "TEST";
            nos.ClOrdID = "TEST" + s_Count.ToString();
            s_Count++;

            nos.Mnemonic = "DELL";
            nos.OrderQty = 100;
            nos.OrdType = KaiTrade.Interfaces.OrderType.LIMIT;
            nos.Price = 11.99M;
            nos.Side = KaiTrade.Interfaces.Side.BUY;

            K2DataObjects.Message msg = new K2DataObjects.Message();
            msg.Data = JsonConvert.SerializeObject(nos);

            _driver.SendMessage(msg);
            
        }
    }
}
