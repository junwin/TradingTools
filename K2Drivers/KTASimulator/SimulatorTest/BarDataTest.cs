using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KaiTrade.Interfaces;
using K2ServiceInterface;
using System.IO;
using Newtonsoft.Json;

namespace SimulatorTest
{
    /// <summary>
    /// Summary description for BarDataTest
    /// </summary>
    [TestClass]
    public class BarDataTest
    {
        static KTASimulator.KTASimulator _driver = null;

        static private Dictionary<string, List<KaiTrade.Interfaces.IMessage>> _messages = null;

        private L1PriceSupport.MemoryPriceHandler _priceHandler = null;

        public BarDataTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

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

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void LoadTSGroupTest()
        {
            //
            // TODO: Add test logic here
            //
            K2DataObjects.TSDataSetData[] tsSet;
            string jsonData = System.IO.File.ReadAllText(@"TestData/TSGrpJSON.txt");

            tsSet = JsonConvert.DeserializeObject<K2DataObjects.TSDataSetData[]>(jsonData);
            Assert.AreEqual(11, tsSet.Length);

        }

        [TestMethod]
        public void RequestTSSetDataTest()
        {

            // reset the message cllection
            _messages = null;

            _driver = new KTASimulator.KTASimulator();
            _driver.Message += new KaiTrade.Interfaces.Message(OnMessage);
            _driver.Start("");


            K2DataObjects.TSDataSetData[] tsSet;
            string jsonData = System.IO.File.ReadAllText(@"TestData/TSGrpJSON.txt");

            tsSet = JsonConvert.DeserializeObject<K2DataObjects.TSDataSetData[]>(jsonData);
            Assert.AreEqual(11, tsSet.Length);

            _driver.RequestTSData(tsSet);
        }
    }
}
