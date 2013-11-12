using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RMQTest
{
    [TestClass]
    public class RMMQPricePublishTest
    {
        RabbitMQPublisher.RMQListner listner = null;

        [TestMethod]
        public void PublishPriceTest()
        {
            RabbitMQPublisher.RMQFactory.Instance().HostName = "10.1.11.14";
            var channel = RabbitMQPublisher.RMQFactory.Instance().GetRMQChannel(KaiTrade.Interfaces.MQExchanges.DEFAULT);
            Assert.IsNotNull(channel);

            RabbitMQPublisher.RMQ rmqHelper = new RabbitMQPublisher.RMQ();

            KaiTrade.Interfaces.IPXUpdate pxu = new L1PriceSupport.PXUpdateBase();
            pxu.Mnemonic = "DELL";
            pxu.BidPrice = 22M;
            pxu.BidSize = 101;
            pxu.OfferPrice = 23M;
            pxu.OfferSize = 99;
            rmqHelper.PublishPrice("DELL", pxu);
        }

        public void OnRMQMessage(string propType, string messageData)
        {
        }

        [TestMethod]
        public void PublishPriceListenTest()
        {
            RabbitMQPublisher.RMQFactory.Instance().HostName = "10.1.11.14";
            var channel = RabbitMQPublisher.RMQFactory.Instance().GetRMQChannel(KaiTrade.Interfaces.MQExchanges.DEFAULT);
            Assert.IsNotNull(channel);

            listner = new RabbitMQPublisher.RMQListner();
            listner.SubscribeInfo();
            listner.OnRMQMessage += OnRMQMessage;
            listner.SubscribePricesRMQ("DELL");

            RabbitMQPublisher.RMQ rmqHelper = new RabbitMQPublisher.RMQ();

            KaiTrade.Interfaces.IPXUpdate pxu = new L1PriceSupport.PXUpdateBase();
            pxu.Mnemonic = "DELL";
            pxu.BidPrice = 22M;
            pxu.BidSize = 101;
            pxu.OfferPrice = 23M;
            pxu.OfferSize = 99;
            rmqHelper.PublishPrice("DELL", pxu);

            System.Threading.Thread.Sleep(10000);
        }
    }
}
