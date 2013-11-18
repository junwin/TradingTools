using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace CQGTestAppWinForm
{
    public partial class Form1 : Form
    {
        static KTACQG.KTACQG _driver = null;

        static private Dictionary<string, List<KaiTrade.Interfaces.IMessage>> _messages = null;

        private L1PriceSupport.MemoryPriceHandler _priceHandler = null;

        private RabbitMQPublisher.RMQ rmqPub;

       
        public Form1()
        {
            InitializeComponent();
            rmqPub = new RabbitMQPublisher.RMQ();

        }

        private void btnCQGStartTest_Click(object sender, EventArgs e)
        {
            StartCQG();
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

        public void StartCQG()
        {
            // reset the message cllection
            _messages = null;
            // EAS will just go into the simulators order book - you can
            // Delete or modify it
            _driver = new KTACQG.KTACQG();
            _driver.Message += new KaiTrade.Interfaces.Message(OnMessage);
            _driver.Start("");

            //System.Threading.Thread.Sleep(100000);
        }

        public  void OnBarUpdate(string requestID, KaiTrade.Interfaces.ITSItem[] bars)
        {
            if (bars.Length > 0)
            {
                rmqPub.Publish(KaiTrade.Interfaces.MQRoutingKeyPrefix.TSBAR+bars[0].Mnemonic, bars);
            }
        }

        private void btnReqBar_Click(object sender, EventArgs e)
        {
            try
            {
                //_driver = new KTACQG.KTACQG();
                // create reader & open file
                TextReader tr = new StreamReader(@"BarReq/SimpleReqJSON.txt");

                // read a line of text
                string jsonData = tr.ReadToEnd();

                var tsItems = JsonConvert.DeserializeObject<K2DataObjects.TSDataSetData[]>(jsonData);
                // close the stream
                tr.Close();

                _driver.BarUpdate += OnBarUpdate;

                var tsSet = new K2DataObjects.TSDataSetData();

                foreach (var item in tsItems)
                {
                    _driver.RequestTSData(item);
                }
                

            }
            catch
            {
            }
        }
    }
}
