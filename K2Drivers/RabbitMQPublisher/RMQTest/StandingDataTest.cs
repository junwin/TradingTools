using System;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RMQTest
{
    [TestClass]
    public class StandingDataTest
    {
        

        RabbitMQPublisher.RMQListner listner = null;

        public void OnRMQMessage(string propType, string messageData)
        {
        }

        [TestMethod]
        public void PublishProductTest()
        {
            RabbitMQPublisher.RMQFactory.Instance().HostName = "10.1.11.37";
            var channel = RabbitMQPublisher.RMQFactory.Instance().GetRMQChannel(KaiTrade.Interfaces.MQExchanges.DEFAULT);
            Assert.IsNotNull(channel);

            listner = new RabbitMQPublisher.RMQListner();
            listner.SubscribeInfo();
            listner.OnRMQMessage += OnRMQMessage;
            listner.SubscribeProductsRMQ("");
            //li
            RabbitMQPublisher.RMQ rmqHelper = new RabbitMQPublisher.RMQ();

            // Example #1 
            // Read the file as one string. 
            string productJSON = System.IO.File.ReadAllText(@"testdata\productJSON.txt");

            K2DataObjects.Product product = JsonConvert.DeserializeObject<K2DataObjects.Product>(productJSON);

            rmqHelper.Publish("", product);

            System.Threading.Thread.Sleep(10000);
        }

        [TestMethod]
        public void PublishAccountTest()
        {
            RabbitMQPublisher.RMQFactory.Instance().HostName = "10.1.11.37";
            var channel = RabbitMQPublisher.RMQFactory.Instance().GetRMQChannel(KaiTrade.Interfaces.MQExchanges.DEFAULT);
            Assert.IsNotNull(channel);

            listner = new RabbitMQPublisher.RMQListner();
            listner.SubscribeInfo();
            listner.OnRMQMessage += OnRMQMessage;
            listner.SubscribeAccountsRMQ("");
            //li
            RabbitMQPublisher.RMQ rmqHelper = new RabbitMQPublisher.RMQ();

            // Example #1 
            // Read the file as one string. 
            string accountJSON = System.IO.File.ReadAllText(@"testdata\accountJSON.txt");

            K2DataObjects.Account account = JsonConvert.DeserializeObject<K2DataObjects.Account>(accountJSON);


            rmqHelper.Publish("", account);



            System.Threading.Thread.Sleep(10000);
        }
    }
}
