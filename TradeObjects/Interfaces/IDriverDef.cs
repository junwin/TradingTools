using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaiTrade.Interfaces
{

   
    public interface IUserCredential
    {
       string UserId {get; set;}
       string Pwd {get; set;}
    }

    public interface IIPEndpoint
    {
        /// <summary>
        /// Server address
        /// </summary>
        string Server { get; set; }

        /// <summary>
        /// Port 
        /// </summary>
        int Port { get; set; }
        /// <summary>
        /// Name of endpoint
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Path on the server
        /// </summary>
        string Path { get; set; }

    }
    /// <summary>
    /// Provides the data to define a driver/connection we may load
    /// </summary>
    public interface IDriverDef
    {
        string Name { get; set; }
        string Code { get; set; }
        string LoadPath { get; set; }
        bool Enabled { get; set; }

        /// <summary>
        /// Second driver code if additional routing is needed
        /// </summary>
        string RouteCode { get; set; }

        /// <summary>
        /// path to config files
        /// </summary>
        string ConfigPath { get; set; }

        /// <summary>
        /// If false the driver can auto start as opposed to 
        /// requiring a start message
        /// </summary>
        bool ManualStart { get; set; }

        /// <summary>
        /// True if the driver is being used on a live market
        /// </summary>
        bool LiveMarket { get; set; }

        /// <summary>
        /// Instruct the drive to hide any UI
        /// </summary>
        bool HideDriverUI { get; set; }

        /// <summary>
        /// Use async price handling
        /// </summary>
        bool AsyncPrices { get; set; }

        /// <summary>
        /// process replace and cancel messages in a queue
        /// </summary>
        bool QueueReplaceRequests { get; set; }
        /// <summary>
        /// List of additional user defined paramters that can
        /// be passed to the driver
        /// </summary>
        List<KaiTrade.Interfaces.IParameter> UserParameters{ get; set; }

        /// <summary>
        /// IPEndpoint of a server used by the driver
        /// </summary>
        IIPEndpoint IPEndPoint { get; set; }

        /// <summary>
        /// User credential required by the driver
        /// </summary>
        IUserCredential UserCredential { get; set; }

        /// <summary>
        /// Message queue exchanges - used by rabbit MQ for example
        /// </summary>
        List<IMQExchange> MQExchanges { get; set; }

        /// <summary>
        /// Keys used to route traffic from the driver
        /// </summary>
        List<IMQRoutingKey> MQRoutingKeys { get; set; }

        
    }
}
