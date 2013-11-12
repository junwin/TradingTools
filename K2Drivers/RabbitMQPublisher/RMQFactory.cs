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
    public  class RMQFactory
    {
        /// <summary>
        /// Singleton OrderManager
        /// </summary>
        private static volatile RMQFactory s_instance;

        /// <summary>
        /// used to lock the class during instantiation
        /// </summary>
        private static object s_Token = new object();

        /// <summary>
        /// Rabbit connection Factory
        /// </summary>
        private ConnectionFactory m_RMQConnectionFactory = null;
        private IConnection m_Conn = null;

        /// <summary>
        /// RMQ channel used to publish general info - initially used to support charts
        /// </summary>
        private IModel m_InfoChannel = null;

        private string hostName = "10.1.11.19";

        // Create a logger for use in this class
        private log4net.ILog m_Log;


        public static RMQFactory Instance()
        {
            // Uses "Lazy initialization" and double-checked locking
            if (s_instance == null)
            {
                lock (s_Token)
                {
                    if (s_instance == null)
                    {
                        s_instance = new RMQFactory();
                    }
                }
            }
            return s_instance;
        }

        protected  RMQFactory()
        {

            m_Log = log4net.LogManager.GetLogger("KTARemote");
        }

        public string HostName
        {
            get { return hostName; }
            set { hostName = value; }
        }

        public ConnectionFactory GetConnectionFactory()
        {
            try
            {
                if (m_RMQConnectionFactory == null)
                {
                    m_RMQConnectionFactory = new ConnectionFactory();


                    m_RMQConnectionFactory.Protocol = Protocols.FromEnvironment();


                    m_RMQConnectionFactory.HostName = HostName;

                    m_Conn = m_RMQConnectionFactory.CreateConnection();
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("GetConnectionFactory", myE);
            }
            return m_RMQConnectionFactory;
        }

        public IModel GetRMQChannel(string exchangeName)
        {
            try
            {
                if (m_InfoChannel == null)
                {
                    GetConnectionFactory();
                    m_InfoChannel = m_Conn.CreateModel();
                    m_InfoChannel.ExchangeDeclare(exchangeName, ExchangeType.Topic);

                }
            }
            catch (Exception myE)
            {
                m_Log.Error("GetRMQInfoChannel", myE);
            }
            return m_InfoChannel;
        }

        public IModel CloseRMQChannel(string exchangeName)
        {
            try
            {
                if (m_InfoChannel != null)
                {
                    m_InfoChannel.Close(Constants.ReplySuccess, "Closing the channel");
                    m_Conn.Close(Constants.ReplySuccess, "Closing the connection");
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("GetRMQInfoChannel", myE);
            }
            return m_InfoChannel;
        }
    }
}
