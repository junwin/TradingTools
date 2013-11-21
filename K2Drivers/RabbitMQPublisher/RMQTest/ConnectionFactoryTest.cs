using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RMQTest
{
    [TestClass]
    public class ConnectionFactoryTest
    {
        private string hostName;

        public ConnectionFactoryTest()
        {
            hostName = RMQTest.Properties.Settings.Default.RMQBrokerAddress;
        }

        [TestMethod]
        public void GetConnectionFactoryTest()
        {           
            RabbitMQPublisher.RMQFactory.Instance().HostName=hostName;
            var connectionfactory = RabbitMQPublisher.RMQFactory.Instance().GetConnectionFactory();
            Assert.IsNotNull(connectionfactory);
        }

        [TestMethod]
        public void GetRMQChannelTest()
        {
            RabbitMQPublisher.RMQFactory.Instance().HostName = hostName;
            var channel = RabbitMQPublisher.RMQFactory.Instance().GetRMQChannel(KaiTrade.Interfaces.MQExchanges.DEFAULT);
            Assert.IsNotNull(channel);

        }
    }
}
