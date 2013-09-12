using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace SimulatorTest
{
    

    [TestClass]
    public class OrderTest
    {
        static KTASimulator.KTASimulator _driver = null;

        [TestMethod]
        public void CreateDriverInstance()
        {
            _driver = new KTASimulator.KTASimulator();
            Assert.IsNotNull(_driver);
        }
        [TestMethod]
        public void SubmitOrderSimInBook()
        {
            // EAS will just go into the simulators order book - you can
            // Delete or modify it
            _driver = new KTASimulator.KTASimulator();
            _driver.Start("");

            K2DataObjects.SubmitRequest nos = new K2DataObjects.SubmitRequest();
            nos.Account = "TEST";
            nos.ClOrdID = DriverBase.Identities.Instance.getNextOrderID();
            
            nos.Mnemonic = "EAS";
            nos.OrderQty = 100;
            nos.OrdType = KaiTrade.Interfaces.OrderType.LIMIT;
            nos.Price = 11.99M;
            nos.Side = KaiTrade.Interfaces.Side.BUY;

            K2DataObjects.Message msg = new K2DataObjects.Message();
            msg.Label = "D";
            msg.Data = JsonConvert.SerializeObject(nos);

            _driver.OnMessage(msg);


            K2DataObjects.CancelOrderRequest cancel = new K2DataObjects.CancelOrderRequest();
            cancel.OrigClOrdID = nos.ClOrdID;
            cancel.ClOrdID = DriverBase.Identities.Instance.getNextOrderID();
            cancel.Mnemonic = nos.Mnemonic;

            msg = new K2DataObjects.Message();
            msg.Label = "F";
            msg.Data = JsonConvert.SerializeObject(cancel);

            _driver.OnMessage(msg);
            
            

            
            
        }
    }
}
