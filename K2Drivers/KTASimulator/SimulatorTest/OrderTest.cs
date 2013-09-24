using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using K2DataObjects;
using KaiTrade.Interfaces;

namespace SimulatorTest
{
    public struct ReceivedfillStates
    {
        public bool osNew;
        public bool osPartFill;
        public bool odFilled;      
    }

    [TestClass]
    public class OrderTest
    {
        static KTASimulator.KTASimulator _driver = null;

        static private Dictionary<string, List<KaiTrade.Interfaces.IMessage>> _messages = null;

        private L1PriceSupport.MemoryPriceHandler _priceHandler = null;

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
            // reset the message cllection
            _messages = null;
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
            // reset the message cllection
            _messages = null;

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

            

            System.Threading.Thread.Sleep(50000);

            if (_messages.Count > 0)
            {
                Assert.AreEqual(4, _messages["8"].Count);

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

        [TestMethod]
        public void SubmitOrderBadAccountReject()
        {
            // reset the message cllection
            _messages = null;

            // HPQ will just fill over time an order of 5 will fill 2,1,2
            _driver = new KTASimulator.KTASimulator();
            _driver.Message += new KaiTrade.Interfaces.Message(OnMessage);
            _driver.Start("");

            K2DataObjects.SubmitRequest nos = new K2DataObjects.SubmitRequest();
            nos.Account = "TestBadAccount";
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



            System.Threading.Thread.Sleep(10000);

            if (_messages.Count > 0)
            {
                Assert.AreEqual(_messages["8"].Count, 1);

                Fill fillNew = JsonConvert.DeserializeObject<Fill>(_messages["8"][0].Data);
                Assert.AreEqual(fillNew.OrderStatus, OrderStatus.REJECTED);
                Assert.AreEqual(fillNew.ClOrdID, nos.ClOrdID);

            }
        }

        public void PriceUpdate(KaiTrade.Interfaces.IPXUpdate pxUpdate)
        {
            if (_priceHandler != null)
            {
                _priceHandler.ApplyPriceUpdate(pxUpdate);
            }
        }

        [TestMethod]
        public void SubmitOrderSimMarket()
        {
            // reset the message cllection
            _messages = null;

            // AAB will just be put into the simulators internal book
            _driver = new KTASimulator.KTASimulator();
            _priceHandler = new L1PriceSupport.MemoryPriceHandler();
            _driver.Facade.PriceHandler = _priceHandler;

            // Wire up a message hander for the executin messages
            _driver.Message += new KaiTrade.Interfaces.Message(OnMessage);
            (_driver as DriverBase.DriverBase).PriceUpdate += new K2ServiceInterface.PriceUpdate(this.PriceUpdate);

            _driver.Start("");


            // Load a file of products to the simulator
            _driver.AddProductDirect(@"Data\SimProduct.txt");
            List<IProduct> products = _driver.Facade.GetProductManager().GetProducts("KTASIM", "", "");

            // test that these are available as products
            Assert.AreEqual(products.Count, 3);

            // Create and register a price publisher for each product
            KaiTrade.Interfaces.IProduct aabProduct = null;
            foreach (IProduct product in products)
            {
                if (product.Mnemonic == "AAB")
                {
                    aabProduct = product;
                    _driver.Facade.PriceHandler.CreatePxPub(product);
                    _driver.OpenPrices(product,0,DateTime.Now.Ticks.ToString());
                }
                
            }

            // Wait for prices in the publisher
            System.Threading.Thread.Sleep(20000);

            // test that some prices are there
            K2ServiceInterface.IL1PX pxPub = _driver.Facade.PriceHandler.GetPXPublisher(aabProduct) as K2ServiceInterface.IL1PX;
            Assert.IsNotNull(pxPub);
            Assert.IsNotNull(pxPub.BidPrice);


            // Test orders in the market - these are filled when the price is
            // matched and there is enough volume
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

            System.Threading.Thread.Sleep(5000);

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

            // wait for the orders to fill - test that the right fills 
            // were received
            System.Threading.Thread.Sleep(180000);

            if (_messages.Count > 0)
            {
                Assert.IsTrue(_messages["8"].Count >= 4);

                ReceivedfillStates ord1States = new ReceivedfillStates();
                ord1States.osNew = false;
                ord1States.osPartFill = false;
                ord1States.odFilled = false;
                ReceivedfillStates ord2States;
                ord2States.osNew = false;
                ord2States.osPartFill = false;
                ord2States.odFilled = false;

                // Both orders should be fully filled 3 reports each
                // New, Partial, Fill
                foreach (IMessage recvMsg in _messages["8"])
                {
                    Fill fill = JsonConvert.DeserializeObject<Fill>(recvMsg.Data);
                    if (fill.ClOrdID == nos.ClOrdID)
                    {
                        // check we have new, partfill, filled
                        switch (fill.OrderStatus)
                        {
                            case OrderStatus.NEW:
                                ord1States.osNew = true;
                                break;
                            case OrderStatus.PARTIALLY_FILLED:
                                ord1States.osPartFill = true;
                                break;
                            case OrderStatus.FILLED:
                                ord1States.odFilled = true;
                                break;
                        }
                    }
                    if (fill.ClOrdID == nos1.ClOrdID)
                    {
                        // check we have new, partfill, filled
                        switch (fill.OrderStatus)
                        {
                            case OrderStatus.NEW:
                                ord2States.osNew = true;
                                break;
                            case OrderStatus.PARTIALLY_FILLED:
                                ord2States.osPartFill = true;
                                break;
                            case OrderStatus.FILLED:
                                ord2States.odFilled = true;
                                break;
                        }
                    }

                }

                Assert.IsTrue(ord1States.osNew && ord1States.osPartFill && ord1States.odFilled);
                Assert.IsTrue(ord2States.osNew && ord2States.osPartFill && ord2States.odFilled);
               

            }
        }
    }
}
