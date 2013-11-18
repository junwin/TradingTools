using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing.v0_8;
using RabbitMQ.Client.Events;

namespace RabbitMQPublisher
{
    public class RMQ
    {
        // Create a logger for use in this class
        private log4net.ILog m_Log;

       


        public  RMQ()
        {
            m_Log = log4net.LogManager.GetLogger("KTARemote");
        }


        /// <summary>
        /// Publish a price
        /// </summary>
        /// <param name="routingKey"></param>
        /// <param name="?"></param>
        public void Publish(string exchangeName, string messageType, string routingKey, byte[] messageBody)
        {
            try
            {

                IBasicProperties props = RMQFactory.Instance().GetRMQChannel(exchangeName).CreateBasicProperties();
                props.ContentType = "text/plain";
                props.DeliveryMode = 2;
                props.Type = messageType;

                RMQFactory.Instance().GetRMQChannel(exchangeName).BasicPublish(exchangeName, routingKey, props, messageBody);
            }
            catch (Exception myE)
            {
            }
        }

        /// <summary>
        /// Publish a price
        /// </summary>
        /// <param name="routingKey"></param>
        /// <param name="?"></param>
        public void PublishPrice(string routingKey, KaiTrade.Interfaces.IPXUpdate px)
        {
            try
            {
                routingKey = KaiTrade.Interfaces.MQRoutingKeyPrefix.PRICES + routingKey;
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(px);
                byte[] messageBody = Encoding.UTF8.GetBytes(jsonData);
                Publish(KaiTrade.Interfaces.MQExchanges.DEFAULT, KaiTrade.Interfaces.MQType.PRICES, routingKey, messageBody); 
            }
            catch (Exception myE)
            {
            }
        }
        public void Publish(string routingKey, KaiTrade.Interfaces.ITSItem[] tsBars)
        {
            try
            {
                routingKey = "TS." + routingKey;
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(tsBars);
                byte[] messageBody = Encoding.UTF8.GetBytes(jsonData);
                Publish(KaiTrade.Interfaces.MQExchanges.DEFAULT, KaiTrade.Interfaces.MQType.TSBAR, routingKey, messageBody); 

            }
            catch (Exception myE)
            {
            }
        }

        
    }
}
