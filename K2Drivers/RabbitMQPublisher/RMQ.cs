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

        /// <summary>
        /// Rabbit connection Factory
        /// </summary>
        private ConnectionFactory m_RMQConnectionFactory = null;
        private IConnection m_Conn = null;

        /// <summary>
        /// RMQ channel used to publish general info - initially used to support charts
        /// </summary>
        private IModel m_InfoChannel = null;

        public const string CHARTINFO = "ChartInfo";
        public const string TRADESIGNAL = "TradeSignal";
        public const string PRICES = "Prices";
        public const string BARDATA = "BarData";
        public const string ORDER = "Order";


        public  RMQ()
        {
            m_Log = log4net.LogManager.GetLogger("KTARemote");
        }

        public ConnectionFactory GetConnectionFactory()
        {
            try
            {
                if (m_RMQConnectionFactory == null)
                {
                    m_RMQConnectionFactory = new ConnectionFactory();


                    m_RMQConnectionFactory.Protocol = Protocols.FromEnvironment();


                    m_RMQConnectionFactory.HostName = "10.1.11.19";

                    m_Conn = m_RMQConnectionFactory.CreateConnection();
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("GetConnectionFactory", myE);
            }
            return m_RMQConnectionFactory;
        }

        public IModel GetRMQInfoChannel()
        {
            try
            {
                if (m_InfoChannel == null)
                {
                    GetConnectionFactory();
                    m_InfoChannel = m_Conn.CreateModel();
                    m_InfoChannel.ExchangeDeclare(CHARTINFO, ExchangeType.Topic);

                }
            }
            catch (Exception myE)
            {
                m_Log.Error("GetRMQInfoChannel", myE);
            }
            return m_InfoChannel;
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
                routingKey = "PX." + routingKey;

                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(px);

                byte[] messageBody = Encoding.UTF8.GetBytes(jsonData);
                IBasicProperties props = GetRMQInfoChannel().CreateBasicProperties();
                props.ContentType = "text/plain";
                props.DeliveryMode = 2;
                props.Type = "Price";

                GetRMQInfoChannel().BasicPublish(CHARTINFO, routingKey, props, messageBody);
            }
            catch (Exception myE)
            {
            }
        }
    }
}
