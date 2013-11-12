using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RMQTest
{
    [TestClass]
    public class ConnectionFactoryTest
    {
        [TestMethod]
        public void GetConnectionFactoryTest()
        {
            RabbitMQPublisher.RMQFactory.Instance().HostName="10.1.11.14";

            var connectionfactory = RabbitMQPublisher.RMQFactory.Instance().GetConnectionFactory();
            Assert.IsNotNull(connectionfactory);
        }

        [TestMethod]
        public void GetRMQChannelTest()
        {
            RabbitMQPublisher.RMQFactory.Instance().HostName = "10.1.11.14";
            var channel = RabbitMQPublisher.RMQFactory.Instance().GetRMQChannel(KaiTrade.Interfaces.MQExchanges.DEFAULT);
            Assert.IsNotNull(channel);

        }
    }
}
