/***************************************************************************
 *
 *      Copyright (c) 2009,2010,2011,2012 KaiTrade LLC (registered in Delaware)
 *                     All Rights Reserved Worldwide
 *
 * STRICTLY PROPRIETARY and CONFIDENTIAL
 *
 * WARNING:  This file is the confidential property of KaiTrade LLC For
 * use only by those with the express written permission and license from
 * KaiTrade LLC.  Unauthorized reproduction, distribution, use or disclosure
 * of this file or any program (or document) is prohibited.
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace K2ServiceInterface
{

    /// <summary>
    /// Results from a replace request - used for collaping replace queues
    /// </summary>
    public enum orderReplaceResult
    {
        /// <summary>
        /// Replace succeded
        /// </summary>
        success,
        /// <summary>
        /// Error during replace
        /// </summary>
        error,
        /// <summary>
        /// Could not replace at this time
        /// </summary>
        replacePending,
        /// <summary>
        /// Cannot cancel at this time
        /// </summary>
        cancelPending
    };

    /// <summary>
    /// Used to get price updates from the driver
    /// </summary>
    /// <param name="pxUpdate"></param>
    public delegate void PriceUpdate(KaiTrade.Interfaces.IPXUpdate pxUpdate);
 

    /// <summary>
    /// Defines the interface a drive(broker connection) must impliment to be loaded and used from
    /// KaiTrade
    /// </summary>
    public interface IDriver
    {
       
        /// <summary>
        /// Send a message - the driver needs to process this message
        /// the Message usually wraps a FIX message for trading
        /// other drivers will use the relevant API
        /// </summary>
        /// <param name="myMsg"></param>
        void Send(KaiTrade.Interfaces.IMessage myMsg);

        /// <summary>
        /// Register some client to receive messages - note use a subject for prices
        /// </summary>
        /// <param name="myClient"> object that will receieve messages</param>
        void Register(string myTag, KaiTrade.Interfaces.IClient myClient);

        /// <summary>
        /// Unregister some client
        /// </summary>
        /// <param name="myClient">client that will be unregistered</param>
        void UnRegister(KaiTrade.Interfaces.IClient myClient);

        /// <summary>
        /// Register a delgate to get price updates - this is complementary
        /// to register a specific publisher
        /// </summary>
        PriceUpdate PriceUpdate
        { get; set; }

       
        /// <summary>
        /// Open prices and depth for the given product
        /// </summary>
        /// <param name="product">product to open prices on</param>
        /// <param name="depthLevels">Depth levels to request</param>
        /// <param name="requestID">identifier for this request</param>
        void OpenPrices(KaiTrade.Interfaces.IProduct product, int depthLevels, string requestID);
       

        /// <summary>
        /// Unregister some subject
        /// </summary>
        /// <param name="myClient">subject that will be unregistered</param>
        void UnRegister(KaiTrade.Interfaces.IPublisher myPublisher);

        
        /// <summary>
        /// Request the product details, get the driver to access the product and fill in
        /// product details in the kaitrade product object.
        /// Note that not all drivers support this and that the call may take some
        /// time to set the values i.e. it is not syncronous
        /// </summary>
        /// <param name="myProduct"></param>
        void RequestProductDetails(KaiTrade.Interfaces.IProduct myProduct);

        /// <summary>
        /// Get a list of the available trade destinations for the cficode specified
        /// note that a market code represents some tradable market/exchange supported by the driver
        /// </summary>
        /// <param name="venueCode"> Cficode - the asset class that the market supports(futures, options, fx etc), if empty all markets </param>
        /// <returns>a list of trade destinations supported by the driver</returns>
        List<KaiTrade.Interfaces.IVenueTradeDestination> GetTradeDestinations(string cfiCode);

        /// <summary>
        /// Set the APP Facade in the plugin - this lets the driver access the functions provided
        /// by KaiTrade using the KaiTrade Facade
        /// </summary>
        /// <param name="myFacade"></param>
        void SetFacade(IFacade myFacade);

        /// <summary>
        /// Start an adapter passing in any state needed - in this release
        /// the state is the working directory so an adapter can locate
        /// any other resources
        /// </summary>
        /// <param name="myState">string state data - this depends on the type od adapter</param>
        void Start(string myState);

        /// <summary>
        /// Stop the adapter
        /// </summary>
        void Stop();

        /// <summary>
        /// If set to true then run on the live market - not all drivers will obey this
        /// </summary>
        bool LiveMarket
        { get; set; }

        /// <summary>
        /// Set the parent of the adapter - this will be some
        /// type of toolkit. This enables a particular adapter to
        /// access that same set of facilities as the toolkit
        /// </summary>
        /// <param name="myParent"></param>
        void SetParent(IDriverManager myParent);



        /// <summary>
        /// Request that all adapters report their status - the adapter returns the
        /// state through the status message that the client interface must support
        /// </summary>
        void StatusRequest();

        /// <summary>
        /// Get the name of the adapter
        /// </summary>
        /// <returns></returns>
        string Name
        {
            get;
        }

        /// <summary>
        ///  ID of the Adapter
        /// </summary>
        string ID
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the tag - this is then returned on any message from the adapter
        /// </summary>
        string Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Get a list of current clients - note an adapter may choose to
        /// return an empty list
        /// </summary>
        System.Collections.Generic.List<KaiTrade.Interfaces.IClient> Clients
        {
            get;
        }

        /// <summary>
        /// Get the state data for the driver - usually passed to the driver
        /// at start up from the provisioning
        /// </summary>
        KaiTrade.Interfaces.Status Status { get; }

        /// <summary>
        /// Get a list of sessions supported by the driver
        /// </summary>
        System.Collections.Generic.List<IDriverSession> Sessions
        {
            get;
        }

        /// <summary>
        /// Request a set of time series data - if supported
        /// </summary>
        /// <param name="myTSSet"></param>
        void RequestTSData( KaiTrade.Interfaces.ITSSet myTSSet);

        /// <summary>
        /// Will request any trade systems that the driver supports - note that this
        /// is asyncronous the driver will add any trading systems using the Facade
        /// </summary>
        void RequestTradeSystems();

        /// <summary>
        /// Request any conditions that the driver supports- note that this
        /// is asyncronous the driver will add any conditions using the Facade
        /// </summary>
        void RequestConditions();

        /// <summary>
        /// Get a set of time series data - if supported
        /// </summary>
        /// <param name="myTSSet"></param>
        void DisconnectTSData( KaiTrade.Interfaces.ITSSet myTSSet);

        /// <summary>
        /// Display or Hide any UI the driver has
        /// </summary>
        /// <param name="uiVisible">true => show UI, False => hide UI</param>
        void ShowUI(bool uiVisible);

        /// <summary>
        /// Get the running status of some driver
        /// compliments the StatusRequest();
        /// </summary>
        IDriverStatus GetRunningStatus();
    }

    /// <summary>
    /// Defines the coarse status of some driver attribute
    /// </summary>
    public enum StatusConditon { good, error, alert, none };

    /// <summary>
    /// Defines an interface to provide a snapshot of
    /// a drivers status
    /// </summary>
    public interface IDriverStatus
    {
        /// <summary>
        /// Status of the order routing connection
        /// </summary>
        StatusConditon OrderRouting
        { get; set; }

        /// <summary>
        /// Status of the prices connection
        /// </summary>
        StatusConditon Prices
        { get; set; }

        /// <summary>
        /// Status of the historic data connection
        /// </summary>
        StatusConditon HistoricData
        { get; set; }
    }
}
