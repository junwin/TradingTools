using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing.v0_8;
using RabbitMQ.Client.Events;

namespace RMQTest
{
    [TestClass]
    public class RMQTSDataPublishTest
    {
        RabbitMQPublisher.RMQListner listner = null;

        public void OnRMQMessage(string propType, string messageData)
        {
        }

        [TestMethod]
        public void PublishBarListenTest()
        {
            RabbitMQPublisher.RMQFactory.Instance().HostName = "10.1.11.14";
            var channel = RabbitMQPublisher.RMQFactory.Instance().GetRMQChannel(KaiTrade.Interfaces.MQExchanges.DEFAULT);
            Assert.IsNotNull(channel);

            listner = new RabbitMQPublisher.RMQListner();
            listner.SubscribeInfo();
            listner.OnRMQMessage += OnRMQMessage;
            listner.SubscribeTSBarsRMQ("IBM");
            //li
            RabbitMQPublisher.RMQ rmqHelper = new RabbitMQPublisher.RMQ();

            KaiTrade.Interfaces.ITSItem[] tsi = new K2DataObjects.TSItem[2];
            
            tsi[0] = new  K2DataObjects.TSItem();
            tsi[0].Mnemonic = "IBM";
            tsi[0].Open = 10;
            tsi[0].Close = 12;
            tsi[0].High = 13;
            tsi[0].Low = 9;
            tsi[1] = new K2DataObjects.TSItem();
            tsi[1].Mnemonic = "IBM";
            tsi[1].Open = 12;
            tsi[1].Close = 14;
            tsi[1].High = 15;
            tsi[1].Low = 12;

            rmqHelper.Publish("IBM", tsi);
 
         

            System.Threading.Thread.Sleep(10000);
        }
    }
}
