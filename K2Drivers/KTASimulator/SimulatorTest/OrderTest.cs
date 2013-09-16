using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using K2DataObjects;
using KaiTrade.Interfaces;

namespace SimulatorTest
{
    

    [TestClass]
    public class OrderTest
    {
        static KTASimulator.KTASimulator _driver = null;

        static private Dictionary<string, List<KaiTrade.Interfaces.IMessage>> _messages = null;

        private void recordMessage(KaiTrade.Interfaces.IMessage message)
        {
            if (_messages == null)
            {
                _messages = new Dictionary<string, List<KaiTrade.Interfaces.IMessage>>();
            }
            if (!_messages.ContainsKey(message.Label))
            {
                _messages.Add(message.Label, new List<KaiTrade.Interfaces.IMessage>());
            }
            _messages[message.Label].Add(message);
            
        }

        void OnMessage(KaiTrade.Interfaces.IMessage message)
        {
            recordMessage(message);
        }

        [TestMethod]
        public void CreateDriverInstance()
        {
            _driver = new KTASimulator.KTASimulator();
            Assert.IsNotNull(_driver);
            _driver.Message += new KaiTrade.Interfaces.Message(OnMessage); 
        }

        /// <summary>
        /// Test the most basic submit, modify and cancel
        /// </summary>
        [TestMethod]
        public void SubmitOrderSimInBook()
        {
            // EAS will just go into the simulators order book - you can
            // Delete or modify it
            _driver = new KTASimulator.KTASimulator();
            _driver.Message += new KaiTrade.Interfaces.Message(OnMessage); 
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

            // Try to modify the order
            ModifyOrderRequest mod = new ModifyOrderRequest();
            mod.OrigClOrdID = nos.ClOrdID;
            mod.ClOrdID = DriverBase.Identities.Instance.getNextOrderID();
            mod.Mnemonic = nos.Mnemonic;

            msg = new K2DataObjects.Message();
            msg.Label = "G";
            msg.Data = JsonConvert.SerializeObject(mod);

            _driver.OnMessage(msg);

            // Try to cancel the order
            K2DataObjects.CancelOrderRequest cancel = new K2DataObjects.CancelOrderRequest();
            cancel.OrigClOrdID = mod.ClOrdID;
            cancel.ClOrdID = DriverBase.Identities.Instance.getNextOrderID();
            cancel.Mnemonic = nos.Mnemonic;

            msg = new K2DataObjects.Message();
            msg.Label = "F";
            msg.Data = JsonConvert.SerializeObject(cancel);

            _driver.OnMessage(msg);


            System.Threading.Thread.Sleep(10000);

            if (_messages.Count > 0)
            {
                Assert.AreEqual(_messages["8"].Count, 3);

                Fill f1 = JsonConvert.DeserializeObject<Fill>(_messages["8"][0].Data);
                Fill fillCancel = JsonConvert.DeserializeObject<Fill>(_messages["8"][2].Data);
                Fill fillMod = JsonConvert.DeserializeObject<Fill>(_messages["8"][1].Data);
                Assert.AreEqual(f1.ClOrdID,nos.ClOrdID);
                Assert.AreEqual(f1.OrderStatus, OrderStatus.NEW);
                Assert.AreEqual(f1.ExecType, ExecType.ORDER_STATUS);

                Assert.AreEqual(fillMod.OrigClOrdID, nos.ClOrdID);
                Assert.AreEqual(fillMod.ClOrdID, mod.ClOrdID);
                Assert.AreEqual(fillMod.OrderStatus, OrderStatus.REPLACED);
                Assert.AreEqual(fillMod.ExecType, ExecType.ORDER_STATUS);

                Assert.AreEqual(fillCancel.ClOrdID, cancel.ClOrdID);
                Assert.AreEqual(fillCancel.OrigClOrdID, mod.ClOrdID);
                Assert.AreEqual(fillCancel.OrderStatus, OrderStatus.CANCELED);
                Assert.AreEqual(fillCancel.ExecType, ExecType.ORDER_STATUS);
            }
        }


        /// <summary>
        /// Test submitting a product that should auto fill
        /// </summary>
        [TestMethod]
        public void SubmitOrderSimAutoFill()
        {
            // HPQ will just fill over time an order of 5 will fill 2,1,2
            _driver = new KTASimulator.KTASimulator();
            _driver.Message += new KaiTrade.Interfaces.Message(OnMessage);
            _driver.Start("");

            K2DataObjects.SubmitRequest nos = new K2DataObjects.SubmitRequest();
            nos.Account = "TEST";
            nos.ClOrdID = DriverBase.Identities.Instance.getNextOrderID();

            nos.Mnemonic = "HPQ";
            nos.OrderQty = 5;
            nos.OrdType = KaiTrade.Interfaces.OrderType.LIMIT;
            nos.Price = 11.99M;
            nos.Side = KaiTrade.Interfaces.Side.BUY;

            K2DataObjects.Message msg = new K2DataObjects.Message();
            msg.Label = "D";
            msg.Data = JsonConvert.SerializeObject(nos);

            _driver.OnMessage(msg);

            

            System.Threading.Thread.Sleep(20000);

            if (_messages.Count > 0)
            {
                Assert.AreEqual(_messages["8"].Count, 4);

                Fill fillNew = JsonConvert.DeserializeObject<Fill>(_messages["8"][0].Data);
                Fill fillPF1 = JsonConvert.DeserializeObject<Fill>(_messages["8"][1].Data);
                Fill fillPF2 = JsonConvert.DeserializeObject<Fill>(_messages["8"][2].Data);
                Fill fillFilled = JsonConvert.DeserializeObject<Fill>(_messages["8"][3].Data);
                Assert.AreEqual(fillNew.ClOrdID, nos.ClOrdID);
                Assert.AreEqual(fillNew.OrderStatus, OrderStatus.NEW);
                Assert.AreEqual(fillNew.ExecType, ExecType.ORDER_STATUS);

                Assert.AreEqual(fillPF1.ClOrdID, nos.ClOrdID);
                Assert.AreEqual(fillPF1.OrderStatus, OrderStatus.PARTIALLY_FILLED);
                Assert.AreEqual(fillPF1.ExecType, ExecType.PARTIAL_FILL);
                Assert.AreEqual(fillPF1.LeavesQty, 3);
                Assert.AreEqual(fillPF1.FillQty, 2);
                Assert.AreEqual(fillPF1.CumQty, 2);

                Assert.AreEqual(fillPF2.ClOrdID, nos.ClOrdID);
                Assert.AreEqual(fillPF2.OrderStatus, OrderStatus.PARTIALLY_FILLED);
                Assert.AreEqual(fillPF2.ExecType, ExecType.PARTIAL_FILL);
                Assert.AreEqual(fillPF2.LeavesQty, 2);
                Assert.AreEqual(fillPF2.FillQty, 1);
                Assert.AreEqual(fillPF2.CumQty, 3);

                Assert.AreEqual(fillFilled.ClOrdID, nos.ClOrdID);
                Assert.AreEqual(fillFilled.OrderStatus, OrderStatus.FILLED);
                Assert.AreEqual(fillFilled.ExecType, ExecType.FILL);
                Assert.AreEqual(fillFilled.CumQty, 5);


/*
                Assert.AreEqual(fillMod.OrigClOrdID, nos.ClOrdID);
                Assert.AreEqual(fillMod.ClOrdID, mod.ClOrdID);
                Assert.AreEqual(fillMod.OrderStatus, OrderStatus.REPLACED);
                Assert.AreEqual(fillMod.ExecType, ExecType.ORDER_STATUS);

                Assert.AreEqual(fillCancel.ClOrdID, cancel.ClOrdID);
                Assert.AreEqual(fillCancel.OrigClOrdID, mod.ClOrdID);
                Assert.AreEqual(fillCancel.OrderStatus, OrderStatus.CANCELED);
                Assert.AreEqual(fillCancel.ExecType, ExecType.ORDER_STATUS);
 */
            }
        }


        [TestMethod]
        public void SubmitOrderSimMarket()
        {
            // AAB will just be put into the simulators internal book
            _driver = new KTASimulator.KTASimulator();
            _driver.Message += new KaiTrade.Interfaces.Message(OnMessage);
            _driver.Start("");


            _driver.AddProductDirect(@"Data\SimProduct.txt");
            List<IProduct> products = _driver.Facade.GetProductManager().GetProducts("KTASIM", "", "");

            Assert.AreEqual(products.Count, 3);

            foreach (IProduct product in products)
            {
                IPublisher pub = _driver.Facade.CreatePxPub(product);
                if (pub != null)
                {
                    _driver.Register(pub,0,DateTime.Now.Ticks.ToString());
                }

            }

            System.Threading.Thread.Sleep(10000);

            K2ServiceInterface.IL1PX pxPub = _driver.Facade.GetL1Prices(products[1]);
            Assert.IsNotNull(pxPub);
            Assert.IsNotNull(pxPub.BidPrice);





            K2DataObjects.SubmitRequest nos = new K2DataObjects.SubmitRequest();
            nos.Account = "TEST";
            nos.ClOrdID = DriverBase.Identities.Instance.getNextOrderID();

            nos.Mnemonic = "AAB";
            nos.SecurityID = "AAB";
            nos.OrderQty = 5;
            nos.OrdType = KaiTrade.Interfaces.OrderType.LIMIT;
            nos.Price = 11.99M;
            nos.Side = KaiTrade.Interfaces.Side.BUY;

            K2DataObjects.Message msg = new K2DataObjects.Message();
            msg.Label = "D";
            msg.Data = JsonConvert.SerializeObject(nos);

            _driver.OnMessage(msg);

            System.Threading.Thread.Sleep(1000);

            K2DataObjects.SubmitRequest nos1 = new K2DataObjects.SubmitRequest();
            nos1.Account = "TEST";
            nos1.ClOrdID = DriverBase.Identities.Instance.getNextOrderID();

            nos1.Mnemonic = "AAB";
            nos1.SecurityID = "AAB";
            nos1.OrderQty = 5;
            nos1.OrdType = KaiTrade.Interfaces.OrderType.LIMIT;
            nos1.Price = 11.99M;
            nos1.Side = KaiTrade.Interfaces.Side.SELL;

            msg = new K2DataObjects.Message();
            msg.Label = "D";
            msg.Data = JsonConvert.SerializeObject(nos1);

            _driver.OnMessage(msg);



            System.Threading.Thread.Sleep(30000);

            if (_messages.Count > 0)
            {
                Assert.AreEqual(_messages["8"].Count, 4);

                Fill fillNew = JsonConvert.DeserializeObject<Fill>(_messages["8"][0].Data);
                Fill fillPF1 = JsonConvert.DeserializeObject<Fill>(_messages["8"][1].Data);
                Fill fillPF2 = JsonConvert.DeserializeObject<Fill>(_messages["8"][2].Data);
                Fill fillFilled = JsonConvert.DeserializeObject<Fill>(_messages["8"][3].Data);
                Assert.AreEqual(fillNew.ClOrdID, nos.ClOrdID);
                Assert.AreEqual(fillNew.OrderStatus, OrderStatus.NEW);
                Assert.AreEqual(fillNew.ExecType, ExecType.ORDER_STATUS);

                Assert.AreEqual(fillPF1.ClOrdID, nos.ClOrdID);
                Assert.AreEqual(fillPF1.OrderStatus, OrderStatus.PARTIALLY_FILLED);
                Assert.AreEqual(fillPF1.ExecType, ExecType.PARTIAL_FILL);
                Assert.AreEqual(fillPF1.LeavesQty, 3);
                Assert.AreEqual(fillPF1.FillQty, 2);
                Assert.AreEqual(fillPF1.CumQty, 2);

                Assert.AreEqual(fillPF2.ClOrdID, nos.ClOrdID);
                Assert.AreEqual(fillPF2.OrderStatus, OrderStatus.PARTIALLY_FILLED);
                Assert.AreEqual(fillPF2.ExecType, ExecType.PARTIAL_FILL);
                Assert.AreEqual(fillPF2.LeavesQty, 2);
                Assert.AreEqual(fillPF2.FillQty, 1);
                Assert.AreEqual(fillPF2.CumQty, 3);

                Assert.AreEqual(fillFilled.ClOrdID, nos.ClOrdID);
                Assert.AreEqual(fillFilled.OrderStatus, OrderStatus.FILLED);
                Assert.AreEqual(fillFilled.ExecType, ExecType.FILL);
                Assert.AreEqual(fillFilled.CumQty, 5);

            }
        }
    }
}
