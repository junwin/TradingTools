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

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Describes the type of message that will be flashed to the user
    /// </summary>
    public enum FlashMessageType { warning, error, info, terminate, none };
    public enum ErrorLevel { none, recoverable, noneRecoverable, fatal };
    public enum FacadeState { none, loaded, starting, started, stopping, stopped }

    /// <summary>
    /// Represents the facade used to provide access to functions
    /// in KTA as a whole
    /// </summary>
    public interface Facade
    {
        /// <summary>
        /// Get the isStarted property true => the app is correctly started
        /// </summary>
        bool Started
        {
            get;
        }

        /// <summary>
        /// Get the main factory
        /// </summary>
        KaiTrade.Interfaces.Factory Factory
        {
            get;
        }

        /// <summary>
        /// Get the instance name for this running facade, needs to be unique the default
        /// facade will return K2+MachineName+":"+ instance count e.g.
        /// K2JUPC:1
        /// </summary>
        /// <returns></returns>
        string GetInstanceName();

        /// <summary>
        /// Get the current session correlation ID for a particular user - the correlation id is used
        /// to send messages back to a client over some session
        /// </summary>
        /// <param name="userIdentity"></param>
        /// <returns></returns>
        string GetCurrentSessionCorrelationID(string userIdentity);

        /// <summary>
        /// Associate a user id with a correlation ID
        /// </summary>
        /// <param name="userIdentity"></param>
        /// <param name="correlationID"></param>
        void AssociateUserSession(string userIdentity, string correlationID);

        /// <summary>
        /// Get the user that corrsponds to a given correlationID
        /// </summary>
        /// <param name="correlationID"></param>
        /// <returns></returns>
        KaiTrade.Interfaces.User GetuserWithCorrelationID(string correlationID);

        /// <summary>
        /// This is the users identifier for the
        /// current session - not their name or pwd
        /// </summary>
        string UserIdentifier
        { get; set;}

         /// <summary>
        /// Start KT App components
        /// </summary>
        /// <param name="myPath"></param>
        void Start();

        /// <summary>
        /// Load a set of plugins given a data binding colection of plugins
        /// </summary>
        /// <param name="plugins"></param>
        /// <returns>number loaded</returns>
        int LoadPlugins(string uid, KAI.kaitns.KTAPlugInCol plugins);
        /// <summary>
        /// Start all plugins for a specified user
        /// </summary>
        /// <param name="uid"></param>
        void StartAllPlugins(string uid);

        /// <summary>
        /// stop all plugins for a specified user
        /// </summary>
        /// <param name="uid"></param>
        void StopAllPlugins(string uid);

        /// <summary>
        /// Stop KT App components
        /// </summary>
        void Stop();

        /// <summary>
        /// Get/Set the current APP path that the facade is being run
        /// </summary>
        string AppPath
        { get;set;}

        /// <summary>
        /// Process a config data binding
        /// </summary>
        /// <param name="myConfig"></param>
        void ProcessConfigXMLDB(KAI.kaitns.KTAppConfig myConfig);

        /// <summary>
        /// Get or set the string of config data, this will be XML
        /// used by the facade to initialize itself - exisitng app
        /// use XML defined by KTAPPConfig schema
        /// </summary>
        string ConfigData
        {
            get;
            set;
        }

        /// <summary>
        /// Dynamically load some plugin (visible or non visible)
        /// </summary>
        /// <param name="uid">user id </param>
        /// <param name="path">path to the plugin</param>
        /// <returns></returns>
        KaiTrade.Interfaces.KTAPlugin DynamicLoad(string uid, string path, string xmlConfig);

        /// <summary>
        /// Get set the list of available plugins
        /// </summary>
        List<KaiTrade.Interfaces.KTAPlugin> PlugIns
        {
            get;
            set;
        }

        /// <summary>
        /// Save manager data - based on paths in the config file
        /// </summary>
        void SaveManagerData();

        /// <summary>
        /// Is external trading allowed for the app i.e. for Excel or Other source
        /// </summary>
        bool IsExternalTradingEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Send an order routing message to the relevant driver
        /// </summary>
        /// <param name="myMsg"></param>
        void SendOR(KaiTrade.Interfaces.Message myMsg);

        /// <summary>
        /// Process a message from a driver - used optionally by clients of the facade
        /// </summary>
        /// <param name="myMsg"></param>
        void OnDriverMessage(KaiTrade.Interfaces.Message myMsg);

        /// <summary>
        /// Register some client to receive messages - note use a subject for prices
        /// </summary>
        /// <param name="myClient"> object that will receieve messages</param>
        void Register(string myTag, Client myClient);

        /// <summary>
        /// Unregister some client
        /// </summary>
        /// <param name="myClient">client that will be unregistered</param>
        void UnRegister(Client myClient);

        /// <summary>
        /// Add a strategy to the strategy manager
        /// </summary>
        /// <param name="myMnemonic"></param>
        /// <param name="myName"></param>
        /// <param name="mySide">as the long BUY, SELL i.e. not the FIX code</param>
        /// <param name="myOrdType">as the long type LIMIT MARKET etc not the FIX Code</param>
        /// <param name="myQty"></param>
        /// <param name="myAccount"></param>
        /// <param name="mtStratType"></param>
        /// <param name="myParms">list of parameters</param>
        /// <returns></returns>
        KaiTrade.Interfaces.Strategy AddStrategy(string myMnemonic, string myName, string mySide, string myOrdType, double myQty, string myAccount, string myAlgoType, List<KaiTrade.Interfaces.K2Parameter> myParms);

        /// <summary>
        /// Add a triggered order the triggered orders maanger based on the order passed in
        /// </summary>
        /// <param name="order">order to be triggered</param>
        /// <param name="orderType">type of order e.g. KTARGET</param>
        /// <param name="parms">extra parameters - depends on order type</param>
        void AddTriggerOrder(KaiTrade.Interfaces.Order order, string orderType, List<KaiTrade.Interfaces.K2Parameter> parms);

        /// <summary>
        /// Add a triggered order the triggered orders maanger based on the order passed in
        /// </summary>
        /// <param name="order">order to be triggered</param>
        /// <param name="orderType">type of order e.g. KTARGET</param>
        /// <param name="targetOrderType">Type of order to submit when triggered</param>
        /// <param name="parms">extra parameters - depends on order type</param>
        void AddTriggerOrder(KaiTrade.Interfaces.Order order, string orderType, string targetOrderType, List<KaiTrade.Interfaces.K2Parameter> parms);

        

        /// <summary>
        /// Place a simple order - returns the order ID
        /// NOTE THIS WILL BE DEPRICATED YOU SHOULD USE ORDERDATA
        /// </summary>
        /// <param name="myMnemonic">string Mnemonic for the product we want to order</param>
        /// <param name="myQty">order QTY</param>
        /// <param name="myPrice">price </param>
        /// <param name="mySide">the long BUY, SELL code i.e. not the FIX code</param>
        /// <param name="myOrderType">type of order  Limit or Market</param>
        /// <param name="myTimeType">time type</param>
        /// <param name="DateTime">date if GTD or GTT</param>
        /// <returns></returns>
        string SubmitOrder(string myStrategyID, string myMnemonic, double myQty, double myPrice, string mySide, string myOrderType, double myStopPx, string myTimeType, string myDateTime, string myAccount, string myShortSellLocate, double? maxFloor, string extendedOrderType, List<KaiTrade.Interfaces.K2Parameter> parms);

        /// <summary>
        /// Place an order based on the order data passed in- this will create a strategey if needed
        /// </summary>
        /// <param name="o"></param>
        /// <returns>order ID - this will be the ID on the order data object passed in</returns>
        string SubmitOrder(KaiTrade.Interfaces.OrderData o);

        /// <summary>
        /// Place an order based on the order data passed in- this will create a strategey if needed
        /// </summary>
        /// <param name="o"></param>
        /// <returns>order ID</returns>
        string SubmitOrder(KaiTrade.Interfaces.StrategyData s, KaiTrade.Interfaces.OrderData o);

        /// <summary>
        /// Execute the strategy for the ID specified
        /// </summary>
        /// <param name="myID"></param>
        /// <returns></returns>
        string ExecuteStrategy(string myID);

        /// <summary>
        /// submit a complete order for processing, note the order must have a
        /// valid product and trade venue
        /// </summary>
        /// <param name="myOrder"></param>
        /// <returns>the order ID or empty string if failed</returns>
        string SubmitOrder(KaiTrade.Interfaces.Order myOrder);

        /// <summary>
        /// Render the given order into FIX
        /// </summary>
        /// <param name="myNOS"></param>
        /// <param name="myMsg"></param>
        /// <param name="myDriverCode"></param>
        /// <param name="myOrder"></param>
        void RenderOrderAsFix(out QuickFix.Message myNOS, out KaiTrade.Interfaces.Message myMsg, out string myDriverCode, KaiTrade.Interfaces.Order myOrder);

        /// <summary>
        /// Cancel the order for the ID specified
        /// </summary>
        /// <param name="myID"></param>
        void CancelOrder(string myID);

        /// <summary>
        /// Cancel all orders
        /// </summary>
        void CancelAllOrders();

       /// <summary>
        /// Edit the order for the ID specified
       /// </summary>
       /// <param name="myID"></param>
       /// <param name="newQty">new qty if specified</param>
       /// <param name="newPrice">new price if specified</param>
       void ReplaceOrder(string myID, double? newQty, double? newPrice, double? newStopPx);

        /// <summary>
        /// Add or replace a trading system - will store this in the tradesystem manager
        /// </summary>
        /// <param name="tradeSystem"></param>
       void AddReplaceTradeSystem(TradeSystem tradeSystem);

        /// <summary>
        /// Get a list of the trading systems avalable for some trade venue
        /// </summary>
        /// <param name="venue">if empty will return all systems</param>
        /// <returns></returns>
       List<TradeSystem> GetAvailableTradeSystems(string venue);

       /// <summary>
       /// Will request any trade systems that the system supports - note that this
       /// is asyncronous the drivers will add any trading systems using the Facade
       /// </summary>
       void RequestTradeSystems();

       /// <summary>
       /// Request any conditions that the system supports- note that this
       /// is asyncronous the drivers will add any conditions using the Facade
       /// </summary>
       void RequestConditions();


        /// <summary>
        /// Sets the state of a condition
        /// </summary>
        /// <param name="name">condition name</param>
        /// <param name="state">true or False</param>
       void SetCondition(string name, bool state);

        /// <summary>
        /// Get the state of named condition - will throw an
        /// excpetion if the condition does not exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
       bool GetCondition(string name);


       /// <summary>
       /// Process a condition that has been monitored by some
        /// time series set
       /// </summary>
       /// <param name="tsSet">set that was used to access the condition from broker/driver</param>
       /// <param name="conditionName">name of the condition</param>
       /// <param name="set">is the condition true or false</param>
       void ProcessCondition(KaiTrade.Interfaces.TSSet tsSet, string conditionName, DateTime timeStamp, bool set);

       /// <summary>
       /// Process a condition that has been monitored by some
       /// time series set
       /// </summary>
       /// <param name="tsSet"></param>
       /// <param name="signal"></param>
       void ProcessTradeSignal(KaiTrade.Interfaces.TSSet tsSet, KaiTrade.Interfaces.TradeSignal signal);

       /// <summary>
       /// Process a trade signal using (or creating) the strategy name given in conjunction with the
       /// AlgoName specified. If the strategy exists (identidied by the ID or UserName) then
       /// the existing strategy is updated using the information in strategyData
       /// </summary>
       /// <param name="strategyData">strategy data that is used to create (or update) a straegy that handles the signal</param>
       /// <param name="signal"></param>
        void ProcessTradeSignal(KaiTrade.Interfaces.StrategyData strategyData, KaiTrade.Interfaces.TradeSignal signal);

        /// <summary>
        /// Process a postion update - 
        /// </summary>
        /// <param name="position"></param>
        void ProcessPositionUpdate(KaiTrade.Interfaces.IPosition position);

        /// <summary>
       /// Request a set of data and conditions using a TSSet - the mnemonic
        /// in the set will be used to select the relevant driver
        /// </summary>
        /// <param name="myTSSet">TS Set - this defines what will be retrieved</param>
       void RequestTSData(KaiTrade.Interfaces.TSSet myTSSet);

       /// <summary>
       /// Simple Request for TS data - not all drivers support this
       /// </summary>
       /// <param name="uid">userid</param>
       /// <param name="requestID">user defined request ID</param>
       /// <param name="mnemonic">mnemonic used to select bar data</param>
       /// <param name="updatesRequired">if true will return updates</param>
       /// <param name="period">lenght  bars</param>
       /// <param name="start">start time - not used if number of bars LT 0</param>
       /// <param name="barCount">number of bars/intervals to return - if 0 all bar from start to current time, id -ve => last N bars</param>
       /// <returns>a TSData set </returns>
       KaiTrade.Interfaces.TSSet RequestTSData(string uid, string requestID, string mnemonic, bool updatesRequired, KaiTrade.Interfaces.TSPeriod period, DateTime start, long barCount);

       /// <summary>
       /// Disconnect from data and conditions
       /// </summary>
       /// <param name="myTSSet"></param>
       void DisconnectTSData(KaiTrade.Interfaces.TSSet myTSSet);

        /// <summary>
        /// Get set the window manager for the system
        /// </summary>
        WindowManager WindowManager
        {
            get;
            set;
        }

        /// <summary>
        /// Get the drivers running status
        /// </summary>
        /// <param name="driverID">the drivers code e.g. KTSIM</param>
        /// <returns></returns>
        KaiTrade.Interfaces.IDriverStatus GetDriverRunningState(string driverCode);

        /// <summary>
        /// get the driver that processes the specified product
        /// </summary>
        /// <param name="myProduct"></param>
        /// <returns></returns>
        KaiTrade.Interfaces.Driver GetDriver(KaiTrade.Interfaces.TradableProduct myProduct);

        /// <summary>
        /// Create and register a publisher with some driver
        /// </summary>
        /// <param name="publisherType">general or prices</param>
        /// <param name="driverCode">valid driver code</param>
        /// <param name="topicName">topic (mnemonic name)</param>
        /// <returns></returns>
        KaiTrade.Interfaces.Publisher CreateRegisterPublisherWithDriver(KaiTrade.Interfaces.PublisherType publisherType, string driverCode, string topicName);

        /// <summary>
        /// Create and register a publisher with some driver
        /// </summary>
        /// <param name="publisherType">name of type of publisher</param>
        /// <param name="driverCode">valid driver code</param>
        /// <param name="topicName">topic (mnemonic name)</param>
        /// <param name="depthLevels">number of depthLevels 0=> none, note not all drivers can do this</param>
        /// <param name="requestID">user defined ID</param>
        /// <returns></returns>
        KaiTrade.Interfaces.Publisher CreateRegisterPublisherWithDriver(string publisherType, string driverCode, string topicName, int depthLevels, string requestID);

        /// <summary>
        /// Get and subscribe to prices for the mnemonic specified, this will
        /// create a price publisher and open the instrument or return an existing
        /// publisher
        /// </summary>
        /// <param name="myMnemonic"></param>
        /// <returns></returns>
        KaiTrade.Interfaces.Publisher GetPXPublisher(string myMnemonic, int depthLevels);

        /// <summary>
        /// Return an instance of a publisher surrogate - will create the publisher
        /// if needed and register it in the publisher manager
        /// </summary>
        /// <param name="name">Name of the publisher</param>
        /// <returns></returns>
        KaiTrade.Interfaces.PublisherSurrogate GetSurrogatePublisher(string name);


        /// <summary>
        /// Get the accounts associated with some product
        /// </summary>
        /// <param name="productID">product ID</param>
        /// <returns>list of account codes - note these are the code NOT the ID</returns>
        List<string> GetMnemonicAccountCode(string mnemonic);
         


        /// <summary>
        /// Start prices foe the mnemonic specified - this will
        /// get the system to susbscribe to prices and update the
        /// product with prices
        /// </summary>
        /// <param name="myMnemonic"></param>
        void StartPrices(string myMnemonic, KaiTrade.Interfaces.Subscriber mySubscriber, int depthLevels);
        void StartPrices(KaiTrade.Interfaces.TradableProduct myProduct, KaiTrade.Interfaces.Subscriber mySubscriber, int depthLevels);

        /// <summary>
        /// This will stop price updates for the product concenerd
        /// unless - other susbcriptions are open for the
        /// product
        /// </summary>
        /// <param name="myMnemonic"></param>
        void StopPrices(string myMnemonic, KaiTrade.Interfaces.Subscriber mySubscriber);
        void StopPrices(KaiTrade.Interfaces.TradableProduct myProduct, KaiTrade.Interfaces.Subscriber mySubscriber);

        /// <summary>
        /// Get a snapshot of the current prices for some product
        /// </summary>
        /// <param name="mnemonic"></param>
        /// <returns></returns>
        KaiTrade.Interfaces.PXUpdate GetPriceSnapshot(string mnemonic);

        /// <summary>
        /// Get the publisher used for triggers
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.Publisher GetTriggerPublisher();

        /// <summary>
        /// Connect the enter and exit fields in the publisher provided
        /// to conditons or signals in the driver specified - this will then
        /// allow the driver to update these conditions.
        /// Note that the names used must correlate to names used in the driver, e.g. the name of
        /// a valid CQG condition
        /// </summary>
        /// <param name="myPub">publisher to get condition updates</param>
        /// <param name="myID">ID for query - allias in CQG</param>
        /// <param name="myEnterName">name of condition or tirgger (cqg condition) for enter</param>
        /// <param name="myExitName">name of condition or tirgger (cqg condition) for exit</param>
        void ConnectTriggers2Driver(string myMnemonic, string myExpression, KaiTrade.Interfaces.Publisher myPub, string myID, string myEnterName, string myExitName, int ConditionInterval);

        /// <summary>
        /// Publish a trigger update
        /// </summary>
        /// <param name="myPubName"></param>
        /// <param name="myTriggerName"></param>
        /// <param name="myValue"></param>
        KaiTrade.Interfaces.Publisher PublishTriggerValue(string myPubName, string myTriggerName, bool myValue);

        /// <summary>
        /// Get/Set the default venue name
        /// </summary>
        /// <returns></returns>
        string DefaultVenueName
        { get; set; }

        /// <summary>
        /// Check if a mnemonic exists and create it if not
        /// </summary>
        /// <param name="myMnemonic">a mnemonic </param>
        void CheckCreateMnemonic(string myMnemonic);

       /// <summary>
        /// Check if a mnemonic exists and create it if not
       /// </summary>
       /// <param name="myMnemonic"></param>
       /// <param name="myVenue"> venue that trades the product</param>
       /// <param name="mySecID">a security id for the product</param>
       /// <param name="myExchange">exchange listing the product</param>
       /// <param name="ExDestination">exchnage that we will trade the product on e.g. IB SMART</param>
       /// <param name="myCFI">CFI code of the product</param>
        /// <param name="mmy">cantract expiration of empty string</param>
        /// <param name="strikePx">option strike price or empty string</param>
        void CheckCreateMnemonic(string myMnemonic, string myVenue, string mySecID, string myExchange, string ExDestination, string myCFI, string Currency, string mmy, string strikePx);

        /// <summary>
        /// Add a product
        /// </summary>
        /// <param name="myUserName">users mnemonic - the product is assigned this mnemonic if specified</param>
        /// <param name="myVenueName">Product venue</param>
        /// <param name="myID">product id - broker specific</param>
        /// <param name="myExchange">Exchnage listing the product(part of the securoty definition) - if empty the default for the venue will be used</param>
        /// <param name="ExDestination">Exchange or venue traidng the product e.g. IB SMART</param>
        /// <param name="myCFI">CFI or empty</param>
        /// <param name="mmy">expiration YYYYMM  or YYYYMMDD depending on broker and CFI</param>
        /// <param name="myCurrency">Currency code or empty - takes venue default currency</param>
        /// <param name="strikePx">strike px or empty currency</param>
        /// <param name="doEvent">true => that we event client of change, false no events raised saves aa lot of processing</param>
        KaiTrade.Interfaces.TradableProduct AddProduct(string myUserName, string myVenueName, string myID, string myExchange, string ExDestination, string myCFI, string mmy, string myCurrency, double? strikePx, bool doEvent);

        /// <summary>
        /// Request the product details, get the driver to access the product and fill in
        /// product details in the kaitrade product object.
        /// Note that not all drivers support this and that the call may take some
        /// time to set the values.
        /// </summary>
        /// <param name="myProduct"></param>
        void RequestProductDetails(KaiTrade.Interfaces.TradableProduct myProduct);

        /// <summary>
        /// Get a list of the available trade destinations for the user and venue specified
        /// note that a trade destination represents some tradable market/exchange supported by the venue
        /// </summary>
        /// <param name="uid"> System assigned user id </param>
        /// <param name="venueCode"> Venue code </param>
        /// <param name="cfiCode"> Cficode - the asset class that the market supports(futures, options, fx etc), if empty all markets </param>
        /// <returns></returns>
        List<KaiTrade.Interfaces.IVenueTradeDestination> GetTradeDestinations(string uid, string venueCode, string cfiCode);

        /// <summary>
        /// Add an order to a group of orders that comprise a set of OCO Orders
        /// </summary>
        /// <param name="?"></param>
        /// <param name="OCOGroupName"></param>
        void AddOrderOCO(KaiTrade.Interfaces.Order order, string OCOGroupName);

        /// <summary>
        /// Get the list of orders that comprise an OCO scheme by thier oco group name
        /// </summary>
        /// <param name="OCOGroupName"></param>
        /// <returns></returns>
        List<KaiTrade.Interfaces.Order> GetOCOOrders(string OCOGroupName);

       
        /// <summary>
        /// Add and register an order based on the order data provided, note that this
        /// will not execute the order - it simply adds it to the order manager.
        /// </summary>
        /// <param name="orderData">order data used to register an order</param>
        /// <param name="fills">list of any fills for the order</param>
        /// <returns></returns>
        KaiTrade.Interfaces.Order RegisterOrder(KaiTrade.Interfaces.OrderData orderData, List<KaiTrade.Interfaces.Fill> fills);


        /// <summary>
        /// Update an existing order using the order data passed amd any fills
        /// NOTE THAT THIS WILL NOT UPDATE ORDERS IN THE MARKET!!
        /// </summary>
        /// <param name="orderData">order data - NOTE THE orderData identity must match an existing order</param>
        /// <param name="fills"></param>
        /// <returns></returns>
        KaiTrade.Interfaces.Order UpdateOrderInformation(KaiTrade.Interfaces.OrderData orderData, List<KaiTrade.Interfaces.Fill> fills);

        /// <summary>
        /// Get the actual product ID for some generic id, for example
        /// if the GenericID is EP, then this will return F.US.EPU2 or similar
        /// </summary>
        /// <param name="genericID">Generic ID (as used in CQG)</param>
        /// <returns>Mnemonic of the current product for the Generic</returns>
        string GetCurrentProductIDWithGenericID(string genericID);

        /// <summary>
        /// Add a product to a venue using its generic name - for example EP in CQG, note
        /// that this usually asyncronous - the product will probably be populated
        /// some time later.
        /// </summary>
        /// <param name="myGenericName"></param>
        /// <param name="myVenueName"></param>
        /// <returns></returns>
        KaiTrade.Interfaces.TradableProduct AddProduct(string myGenericName, string myVenueName);

        /// <summary>
        /// Update or Add a leg to the named ML product
        /// </summary>
        /// <param name="myMLName"></param>
        /// <param name="myMnemonic"></param>
        /// <param name="myQty"></param>
        /// <param name="mySide"></param>
        /// <param name="myOrdType"></param>
        /// <param name="myAccount"></param>
        /// <param name="myPriceOffset"></param>
        /// <param name="myMultiplier"></param>
        /// <param name="myDamper"></param>
        /// <param name="myQuoted"></param>
        void UpdateLeg(string myMLName, string myMnemonic, int myQty, string mySide, string myOrdType, string myAccount, double myPriceOffset, double myTickSize, double myMultiplier, double myDamper, bool myQuoted, int myPosition);

        /// <summary>
        /// Set a field bag from a delimited string
        /// </summary>
        /// <param name="myFieldBag"></param>
        /// <param name="myFields"></param>
        /// <param name="myDelmiter"></param>
        void SetBag(out List<KaiTrade.Interfaces.Field> myFieldBag, string myFields, string myDelmiter);
        void SetBag(out List<KaiTrade.Interfaces.K2Parameter> myFieldBag, string myFields, string myDelmiter);

        /// <summary>
        /// Get a delimeted string containing the bag of field data
        /// </summary>
        /// <param name="myFieldBag"></param>
        /// <param name="myDelimiter"></param>
        /// <returns></returns>
        string BagAsString(List<KaiTrade.Interfaces.Field> myFieldBag, string myDelimiter);
        string BagAsString(List<KaiTrade.Interfaces.K2Parameter> myFieldBag, string myDelimiter);

        /// <summary>
        /// Get the user property set from storage - the storage method
        /// can vary
        /// </summary>
        /// <returns></returns>
        string GetUserPropertySetXML();

        /// <summary>
        /// return the next kaitrade ID - unique in the run of KaiTrade
        /// </summary>
        /// <returns></returns>
        string GetNextKTID();

        /// <summary>
        /// Store the user property set into storage - the storage method
        /// can vary
        /// </summary>
        /// <param name="myUPSet"></param>
        void StoreUserPropertySetXML(string myXML);

        /// <summary>
        /// Store a proptery in the users property bag, this is store in
        /// the users folder and is not affected by rmoving the system
        /// </summary>
        /// <param name="myName">property name</param>
        /// <param name="myValue">property value</param>
        void SaveUserProperty(string myName, string myValue);

        /// <summary>
        /// Get a stroed user parameter, this returns a stroed value from
        /// the users property bag or an empty string if the bag or the
        /// property is not found
        /// </summary>
        /// <param name="myName"></param>
        /// <returns></returns>
        string GetUserProperty(string myName);
        

        /// <summary>
        /// Get a property that was set in the user profile - note that this will
        /// read a property from the profile, you cant edit this. There is SaveUserProperty
        /// and GetUserProperty() to get local properties
        /// </summary>
        /// <param name="myName"></param>
        /// <returns></returns>
        string GetUserProfileProperty(string uid, string myName);

        /// <summary>
        /// Get a list of the available profile properties
        /// </summary>
        /// <returns></returns>
        List<string> GetUserProfilePropertyList(string uid);

        /// <summary>
        /// Flash a warning or error message to the user
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="type"></param>
        void FlashMessage(string msg, FlashMessageType type);

        /// <summary>
        /// Raise and alert
        /// </summary>
        /// <param name="origin">Origin of the alert</param>
        /// <param name="msg">ALert message</param>
        /// <param name="appSpecific">error, message or other ID related to the origin</param>
        /// <param name="errorLevel">Recoverable/NoneRecoverable</param>
        /// <param name="type">error, warn</param>
        void RaiseAlert(string origin, string msg, int appSpecific, ErrorLevel errorLevel, FlashMessageType type);
        

        /// <summary>
        /// Sign in an external user of the system (app running over WCF other than ExcelRTD)
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="pwd"></param>
        /// <returns>System assigned ID - used on requests</returns>
        string SignInExternalUser(string userID, string pwd);

        /// <summary>
        /// SignOut an external user
        /// </summary>
        /// <param name="userID"></param>

        void SignOutExternalUser(string sessionID);

        /// <summary>
        /// Get the users XML profile from the server
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        string GetUserProfile(string sessoinID);

        /// <summary>
        /// Returns the current GUI user  
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.User CurrentGUIUser
        {get; set;}

        /// <summary>
        /// Get the current facade state
        /// </summary>
        FacadeState State { get; }

    }
}
