/***************************************************************************
 *
 *      Copyright (c) 2009,2010,2011 KaiTrade LLC (registered in Delaware)
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
    /// Basic state of a strategy
    /// </summary>
    public enum StrategyState
    {
        enter, exit, complete, init, error, cancelled
    }

   /// <summary>
    /// Delegate to indicate that the strategy has changed in some way
   /// </summary>
   /// <param name="strategy"></param>
    public delegate void StrategyChanged(KaiTrade.Interfaces.Strategy strategy);

    /// <summary>
    /// Models a strategy that we will run in the system - in gerneral some of the
    /// strategy properties act as a default, in terms of multi-leg products the
    /// leg values always override the strategy values. For example 2 legs can trade on
    /// different venues and thus have different accounts.
    /// </summary>
    public interface Strategy
    {
        /// <summary>
        /// Get the Identity of the strategy
        /// </summary>
        string Identity
        {
            get;
            set;
        }

        /// <summary>
        /// Get set the underlying dataobject - the data object is a data contract
        /// used for WCF services - can be null is this mechanism is not supported
        /// </summary>
        KaiTrade.Interfaces.StrategyData StrategyData
        { get; set; }


        /// <summary>
        /// Get/Set user's session ID that the order belongs to - note that a user can have N sessions over some time period
        /// </summary>
        string SessionID
        { get; set; }


        /// <summary>
        /// Get/Set user identity (a guid) that the strategy belongs to
        /// </summary>
        string User
        { get; set; }


        /// <summary>
        /// get/set the type of strategy
        /// </summary>
        StrategyType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Get the state of the strategy
        /// </summary>
        StrategyState State
        {
            get;
            set;
        }

        /// <summary>
        /// used to connect delegates for the strategy changed event
        /// </summary>
        StrategyChanged StrategyChanged
        {
            get;
            set;
        }

        /// <summary>
        /// get/set the algorithm used to process orders in the strategy
        /// </summary>
        ORStrategyAlgorithm ORAlgorithm
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the name of the OR Alg used in the strategy
        /// this is used for load/store of the strategy so that we can
        /// create an instance of the alg as required.
        /// </summary>
        string ORAlgorithmName
        {
            get;
            set;
        }

        /// <summary>
        /// get/set the algorithm used to handle price changes for the strategy
        /// </summary>
        KaiTrade.Interfaces.PriceAlg PXAlgorithm
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the name of the PXAlgorithm used in the strategy
        /// this is used for load/store of the strategy so that we can
        /// create an instance of the alg as required.
        /// </summary>
        string PXAlgorithmName
        {
            get;
            set;
        }

        /// <summary>
        /// Add a single paramter to the Strategy - these are used by the
        /// algo running in the strategy
        /// </summary>
        /// <param name="myID"></param>
        /// <param name="myValue"></param>
        void AddAlgParameter(string myID, string myValue);

        /// <summary>
        /// Replace the existing strategy parameters with the list specified - note 
        /// that this will replace existing values, add new parameters and leave any other 
        /// parameters as is
        /// </summary>
        /// <param name="myParameters"></param>
        void ReplaceAlgParameter(List<KaiTrade.Interfaces.K2Parameter> myParameters);

      /// <summary>
        /// Get the value of a parameter used by algos running in this strategy
      /// </summary>
      /// <param name="myID"></param>
      /// <returns></returns>
        string GetAlgParameterValue(string myID);

        /// <summary>
        /// Get the ATDL  string for the algo
        /// </summary>
        /// <returns></returns>
        string GetAtdl();

        /// <summary>
        /// Clear all algo parameters
        /// </summary>
        void ClearAlgParameters();

        /// <summary>
        /// get/set the list of parameters used with the alg
        /// </summary>
        List<KaiTrade.Interfaces.K2Parameter> AlgParameters
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set paramters as a string delimited bag of values
        /// </summary>
        string ParameterBag
        {
            get;
            set;
        }

        /// <summary>
        /// Set the algos parameters
        /// </summary>
        void SetAlgParameters();

        /// <summary>
        /// Set a strategy internal property
        /// </summary>
        /// <param name="myID">parameter name</param>
        /// <param name="myValue">parameter value</param>
        /// <param name="myRunIdentifier">the run identifier(excel row or id) - if empty applies in general to the strategy</param>
        void SetStrategyProperty(string myID, string myValue, string myRunIdentifier);
        void SetStrategyProperty(KaiTrade.Interfaces.K2Parameter parameter, string myRunIdentifier);

        /// <summary>
        /// Reset the list of changed parameters - all are set to not changed
        /// </summary>
        void ResetChangedParameters();

        /// <summary>
        /// Checks if all the parameter names on the lits have been changed/updated/added
        /// </summary>
        /// <param name="names">list of parameter names</param>
        /// <returns></returns>
        bool AllParametersUpdated(List<string> names);

        /// <summary>
        /// get/set the strategy name that the user gave to the strategy - more friendly than the strategy identity GUID ID
        /// </summary>
        string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Get a list of orders that are associated with this strategy
        /// </summary>
        /// <returns></returns>
        List<string> GetOrders();

        /// <summary>
        /// Add an order to the strategy
        /// </summary>
        /// <param name="myOrder"></param>
        void AddOrder(Order myOrder);

        /// <summary>
        /// get the identity of the last order submitted for this strategy
        /// </summary>
        string LastOrdIdentity
        {
            get;
            set;
        }
        /// <summary>
        /// Get the product ID associated with the strategy
        /// </summary>
        string ProductID
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the product mnemonic
        /// </summary>
        string Mnemonic
        {
            get;

            set;
        }

        /// <summary>
        /// Get/Set the data mnemonic, this is used when accessing data if specified
        /// if this is not use then the base Mnemonic is used for both. It helps when
        /// you want to get data from one venue and trade on another
        /// </summary>
        string DataMnemonic
        {
            get;

            set;
        }

        /// <summary>
        /// Get/Set the strategy product
        /// </summary>
        TradableProduct Product
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the account for the strategy
        /// </summary>
        string Account
        {
            get;
            set;
        }

        /// <summary>
        /// Get the default side of this strategy
        /// </summary>
        QuickFix.Side Side
        {
            get;
            set;
        }

        /// <summary>
        /// Get the default side of this strategy as a string
        /// </summary>
        string SideAsString
        {
            get;
        }
        /// <summary>
        /// get/set the default order type
        /// </summary>
        string OrdTypeAsString
        {
            get;
        }

        /// <summary>
        /// If the side is Short Sell this needs to be used
        /// to specify where the products are held
        /// </summary>
        string ShortSaleLocate
        {
            get;
            set;
        }

        /// <summary>
        /// get/set the default order type
        /// </summary>
        QuickFix.OrdType OrdType
        {
            get;
            set;
        }
        /// <summary>
        /// get/set the default TimeInForce
        /// </summary>
        string TimeInForce
        {
            get;
            set;
        }

        /// <summary>
        /// get/set the default date time for the time in force
        /// </summary>
        string TIFDateTime
        {
            get;
            set;
        }

        /// <summary>
        /// Set the default qty  for the strategy
        /// </summary>
        double Qty
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the max floor
        /// </summary>
        double? MaxFloor
        {
            get;
            set;
        }

        /// <summary>
        /// Set the default price  for the strategy
        /// </summary>
        double Price
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the stop PX for the strategy - note the behaviour
        /// depends on the type of strategy, in general this works only for
        /// single legs
        /// </summary>
        double StopPx
        {
            get;
            set;
        }

        /// <summary>
        /// Set the qty limit for the strategy
        /// </summary>
        double QtyLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the maximum number of iterations that are allowed in any
        /// run of the strategy - for example how many scalp orders sets
        /// can be placed
        /// </summary>
        int MaxIterations
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the maximum number of times the strategy may be entered - this is reset
        /// when a strategy is loaded or created, unlike max iterations which is the max number of
        /// runs/orders perminted in each entry, this is the max number of times you
        /// can enter the strategy. It defaults to -1 i.e. not limited
        /// </summary>
        int MaxEntries
        {
            get;
            set;
        }

        /// <summary>
        /// Max price for the strategy: max price allowed on any orders from the strategey - confirm that the algo used supports this
        /// </summary>
        double MaxPrice
        {
            get;
            set;
        }

        /// <summary>
        /// Min price for the strategy: min price allowed on any orders from the strategey - confirm that the algo used supports this
        /// </summary>
        double MinPrice
        {
            get;
            set;
        }

        /// <summary>
        /// Test if a price is in a valid range - NOT AVAILABLE FOR ALL ALGOS NOTE: DEFAULT IS TRUE
        /// You *MUST* set the MaxPrice and MinPrice first
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        bool PriceInValidRange(string orderType, double price);

        /// <summary>
        /// Get the short working qty of the strategy
        /// </summary>
        double ShortWorkingQty
        {
            get;
            set;
        }
        /// <summary>
        /// Get the LongWorking qty for the strategy
        /// </summary>
        double LongWorkingQty
        {
            get;
            set;
        }
        /// <summary>
        /// Get the Long potential how many *could* be filled) qty for the strategy
        /// </summary>
        double LongPotentialQty
        {
            get;
            set;
        }
        /// <summary>
        /// get the short potential (how many *could* be filled) qty
        /// </summary>
        double ShortPotentialQty
        {
            get;
            set;
        }
        /// <summary>
        /// get the short filled qty
        /// </summary>
        double ShortFilledQty
        {
            get;
            set;
        }
        /// <summary>
        /// return the position (long filled - short filled) +ve => long
        /// </summary>
        double Position
        {
            get;
        }
        /// <summary>
        /// return the potential position (long filled - short filled) +ve => long, this includes working and pending qty
        /// </summary>
        double PotentialPosition
        {
            get;
        }
        /// <summary>
        /// return the working qty (long working - short working) +ve => long
        /// </summary>
        double Working
        {
            get;
        }
        /// <summary>
        /// get the long filled qty
        /// </summary>
        double LongFilledQty
        {
            get;
            set;
        }

        /// <summary>
        /// get the PnL for the strategy - calculated when accessed
        /// not you need prices for this.
        /// </summary>
        double GetPNL();

        /// <summary>
        /// Get the average price for all the fills on the strategy
        /// NOTE - Should only be used for single leg strategies, else it is the  average
        /// over N products
        /// </summary>
        /// <returns></returns>
        double GetAveragePrice();

        /// <summary>
        /// Called to get the strategy to do time based processing, for example
        /// to exit strategies after a time period (end time)
        /// </summary>
        /// <param name="myTime">time to use in any operations e.g. from an API to override
        /// the time on the PC</param>
        void Tick(DateTime myTime);

        /// <summary>
        /// Enter the strategy - action depends on implimenting class, in general this is
        /// the prefered way to run or submit orders in a strategy
        /// </summary>
        /// <param name="myReEnter">if true - a strategy in the entered state can be renetered</param>
        void Enter(bool myReEnter);

        /// <summary>
        /// get the datetime of when the strategy was last entered
        /// </summary>
        DateTime LastEnterTime
        { get;}

        /// <summary>
        /// Exit the strategy - action depends on implimenting class
        /// </summary>
        void Exit();

        /// <summary>
        /// Process a Trade signal
        /// </summary>
        /// <param name="mySignal"></param>
        void HandleTradeSignal(KaiTrade.Interfaces.TradeSignal mySignal);

        /// <summary>
        /// Determines if we flatten position on Exit
        /// </summary>
        bool FlattenOnExit
        { get; set;}

        /// <summary>
        /// Determines if we cancels working orders  on Exit
        /// </summary>
        bool CancelOnExit
        { get; set;}

        /// <summary>
        /// determine if we use the strategies start and end times
        /// </summary>
        bool UseStrategyTimes
        { get; set;}

        /// <summary>
        /// Time of day the strategy can run from(start time) - if specified not time limits
        /// </summary>
        DateTime StartTime
        { get; set;}

        /// <summary>
        /// Time of day the strategy can run to(end time) - if specified not time limits
        /// if the strategy is an enter state when the end time passes the strategy will
        /// Exit and obey any exit rules in force
        /// </summary>
        DateTime EndTime
        { get; set;}

        /// <summary>
        /// Submit the strategy based on its default values
        /// </summary>
        /// <returns></returns>
        string Submit();

        /// <summary>
        /// Submit an order for this strategy
        /// </summary>
        /// <param name="myQty">applied to all the oustanding orders according to ratios in product legs</param>
        /// <param name="myQtyLimit">may qty bought or sold for the strategy</param>
        /// <param name="myPrice">applied to all the oustanding orders according to ratios in product legs</param>
        /// <param name="myStopPx">stop price if applicable</param>
        /// <param name="mySide"></param>
        /// <param name="myOrderType"></param>
        /// <param name="myTimeType"></param>
        /// <param name="myDateTime"></param>
        /// <param name="myAccount">account id will override venue and product account</param>
        /// <returns></returns>
        string Submit(double myQty, double myQtyLimit, double myPrice, double myStopPx, string mySide, string myOrderType, string myTimeType, string myDateTime, string myAccount, string extendedOrderType, List<KaiTrade.Interfaces.K2Parameter> parms);

        /// <summary>
        /// Cancel any outstanding orders associated with this strategy
        /// </summary>
        void Cancel();

        /// <summary>
        /// Cancel any buy orders
        /// </summary>
        void CancelBuy();

        /// <summary>
        /// Cancel any sell order
        /// </summary>
        void CancelSell();

        /// <summary>
        /// Reset - the strategy to its new condition, reset any internal collections and states
        /// </summary>
        void Reset();

        /// <summary>
        /// Flatten positions and cancel working
        /// </summary>
        void Flatten();

        /// <summary>
                /// Modify any oustanding orders
        /// </summary>
        /// <param name="newQty"></param>
        /// <param name="newPrice"></param>
        /// <returns>number of orders affected</returns>
        int Replace(double? newQty, double? newPrice, double? newStopPx);

        /// <summary>
        /// Handle an Exec report for an order belonging to a strategy
        /// the primitive gandling of the exec report on the order itself will have been done
        /// exec reports always relate to an actual order in the market
        /// </summary>
        /// <param name="myExec">fix execution report</param>
        /// <param name="myOrd">order the execution report applies to </param>
        void HandleExecReport(QuickFix.Message myExec, KaiTrade.Interfaces.Order myOrd);

        /// <summary>
        /// Handle an reject report for an order belonging to a strategy
        /// the primitive gandling of the exec report on the order itself will have been done
        /// reject reports always relate to an actual order in the market
        /// </summary>
        /// <param name="myExec">fix execution report</param>
        /// <param name="myOrd">order the execution report applies to </param>
        void HandleReject(QuickFix.Message myReject, KaiTrade.Interfaces.Order myOrd);

       /// <summary>
        /// Refresh the position of the Strategy, will iterate the orders to
        /// total position
        /// </summary>
        void RefreshPosition();

        /// <summary>
        /// Subscribe to the Enter and Exit Triggers - given the names provided, these are items
        /// in the Trigger publisher
        /// </summary>
        /// <param name="myEntryTrgname">name of entry trigger field</param>
        /// <param name="myExitTrgName">name of exit  trigger field</param>
        void SubscribeTriggers(KaiTrade.Interfaces.Facade myFacade, string myEntryTrgname, string myExitTrgName);
        void SubscribeTriggers(KaiTrade.Interfaces.Facade myFacade);

        /// <summary>
        /// Get the trigger name used for this strategy - THIS IS Depricated
        /// </summary>
        string TriggerName
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set entry trigger name - this will be depricated to support multiple trigger names
        /// </summary>
        string EntryTriggerName
        {
            get;
            set;
        }
        /// <summary>
        /// Get/Set exit trigger name - this will be depricated to support multiple trigger names
        /// </summary>
        string ExitTriggerName
        {
            get;
            set;
        }

        

        /// <summary>
        /// A signal name to the list of names we support - this
        /// can be used to control what signals are processed by the strategy
        /// </summary>
        /// <returns></returns>
        List<string> SignalNames 
        {
            get;
            set;
        }

        /// <summary>
        /// List of all trade signals this strategy has received
        /// </summary>
        List<TradeSignal> TradeSignalsReceived
        { get; set; }

        /// <summary>
        /// Add an exit trigger
        /// </summary>
        /// <param name="name">name of trigger</param>
        /// <param name="isSet">state of trigger</param>
        void XAddExitTriggerName(string name, bool isSet);

        /// <summary>
        /// Add an entry trigger
        /// </summary>
        /// <param name="name">name of trigger</param>
        /// <param name="isSet">state of trigger</param>
        void XAddEntryTriggerName(string name, bool isSet);

        string TriggerValues
        { get;}

        /// <summary>
        /// get/set whether the strategy will attempt to auto connect the enter and exit
        /// trigger names to constions in the trade venue
        /// </summary>
        bool AutoConnectTrg
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the interval used on conditions
        /// </summary>
        int ConditionInterval
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the enabled flag for the strategy
        /// </summary>
        bool Enabled
        {
            get;
            set;
        }
        /// <summary>
        /// convert the strategy type enum to a string
        /// </summary>
        /// <param name="myType"></param>
        /// <returns></returns>
        string EncodeType(KaiTrade.Interfaces.StrategyType myType);

        /// <summary>
        /// Convert the string representation of strategy type to the enum
        /// </summary>
        /// <param name="myType"></param>
        /// <returns></returns>
        KaiTrade.Interfaces.StrategyType DecodeType(string myType);

        /// <summary>
        /// Set up strategy from an XML data binding
        /// </summary>
        /// <param name="myOrder"></param>
        void FromXMLDB(KAI.kaitns.Strategy myStrategy);

        /// <summary>
        /// write strategy onto an XML data bining
        /// </summary>
        /// <returns></returns>
        KAI.kaitns.Strategy ToXMLDB();

        /// <summary>
        /// render the strategy in ATDL (FIX Algorithmic Trading Definition Language)
        /// </summary>
        /// <returns></returns>
        string ToAtdl();

        /// <summary>
        /// number of complete runs - e.g. a fully sequence buy-sell etc is done
        /// </summary>
        int Compeleted
        { get;}

        /// <summary>
        /// how many time we have run
        /// </summary>
        int RunCount
        { get;}

        /// <summary>
        /// The identifier for a particuar run/entry  of the strategy
        /// can be used to publish status and information from a particular run
        /// for example fills for a particular run.
        /// </summary>
        string RunIdentifier
        { get; set; }

        /// <summary>
        /// Identifier used to track a trade system use of orders, strategeies and algos against some
        /// ID
        /// </summary>
        string CorrelationID
        {
            get;
            set;
        }

        /// <summary>
        /// Add an order group to the strategy
        /// </summary>
        /// <param name="myGrp"></param>
        void AddOrderGroup(KaiTrade.Interfaces.OrderGroup myGrp);

        /// <summary>
        /// Get an order group with its ID
        /// </summary>
        /// <param name="myID"></param>
        /// <returns></returns>
        KaiTrade.Interfaces.OrderGroup GetOrderGroup(string myID);

        /// <summary>
        /// Add an order group for a particular Leg ID
        /// </summary>
        /// <param name="myLegID">unique idenitfier for the leg</param>
        /// <param name="myGrp">order group</param>
        void AddLegOrderGroup(string myLegID, KaiTrade.Interfaces.OrderGroup myGrp);

        /// <summary>
        /// Get an order group for the Leg ID specified - this will create the group if required
        /// </summary>
        /// <param name="myID">Leg idebtifier</param>
        /// <returns></returns>
        KaiTrade.Interfaces.OrderGroup GetLegOrderGroup(string myID);

        /// <summary>
        /// Get all of the strategy order groups - excluding the LEG Groups
        /// </summary>
        /// <returns></returns>
        List<KaiTrade.Interfaces.OrderGroup> GetOrderGroups();

        /// <summary>
        /// Get/Set the path of a TS Query group that may be used in the
        /// strategy
        /// </summary>
        string TSQueryGroupPath
        {
            get;
            set;
        }

        /// <summary>
        /// Free format info about the strategy - for example output from an Algo
        /// </summary>
        string Info
        {
            get;
            set;
        }

        /// <summary>
        /// Has the strategy been initialized?
        /// </summary>
        bool Initialized
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents the base data in a strategy
    /// </summary>
    public interface StrategyData
    {
        /// <summary>
        /// Get the Identity of the strategy
        /// </summary>
        string Identity
        {
            get;
            set;
        }

        /// <summary>
        /// Get set AutoCreate, if true then the system will create the strategy if needed without
        /// the user needed to do an explicit add
        /// </summary>
        bool AutoCreate
        { get; set; }

        /// <summary>
        /// Get/Set user's session ID that the order belongs to - note that a user can have N sessions over some time period
        /// </summary>
        string SessionID
        { get; set; }


        /// <summary>
        /// Get/Set user identity (a guid) that the strategy belongs to
        /// </summary>
        string User
        { get; set; }



        /// <summary>
        /// The identifier for a particuar run/entry  of the strategy
        /// can be used to publish status and information from a particular run
        /// for example fills for a particular run.
        /// </summary>
        string RunIdentifier
        { get; set; }


        /// <summary>
        /// Identifier used to track a trade system use of orders, strategeies and algos against some
        /// ID
        /// </summary>
        string CorrelationID
        {
            get;
            set;
        }

        /// <summary>
        /// get/set the type of strategy
        /// </summary>
        StrategyType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Get the state of the strategy
        /// </summary>
        StrategyState State
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the name of the OR Alg used in the strategy
        /// this is used for load/store of the strategy so that we can
        /// create an instance of the alg as required.
        /// </summary>
        string ORAlgorithmName
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the name of the PXAlgorithm used in the strategy
        /// this is used for load/store of the strategy so that we can
        /// create an instance of the alg as required.
        /// </summary>
        string PXAlgorithmName
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set paramters as a string delimited bag of values - used to persist K2Parameters
        /// </summary>
        string ParameterBag
        {
            get;
            set;
        }

        /// <summary>
        /// Get set the list of parameters associated with the strategy
        /// </summary>
        List<K2Parameter> K2Parameters
        {
            get;
            set;
        }

        /// <summary>
        /// get/set the users namer for the strategy - more friendly than the GUID ID
        /// </summary>
        string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// get the identity of the last order submitted for this strategy
        /// </summary>
        string LastOrdIdentity
        {
            get;
            set;
        }
        /// <summary>
        /// Get the product ID associated with the strategy
        /// </summary>
        string ProductID
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the product mnemonic
        /// </summary>
        string Mnemonic
        {
            get;

            set;
        }

        /// <summary>
        /// Get/Set the data mnemonic, this is used when accessing data if specified
        /// if this is not use then the base Mnemonic is used for both. It helps when
        /// you want to get data from one venue and trade on another
        /// </summary>
        string DataMnemonic
        {
            get;

            set;
        }

        /// <summary>
        /// Get/Set the account for the strategy
        /// </summary>
        string Account
        {
            get;
            set;
        }

        /// <summary>
        /// Get the default side of this strategy
        /// </summary>
        string Side
        {
            get;
            set;
        }

        /// <summary>
        /// If the side is Short Sell this needs to be used
        /// to specify where the products are held
        /// </summary>
        string ShortSaleLocate
        {
            get;
            set;
        }

        /// <summary>
        /// get/set the default order type
        /// </summary>
        string OrdType
        {
            get;
            set;
        }
        /// <summary>
        /// get/set the default TimeInForce
        /// </summary>
        string TimeInForce
        {
            get;
            set;
        }

        /// <summary>
        /// get/set the default date time for the time in force
        /// </summary>
        string TIFDateTime
        {
            get;
            set;
        }

        /// <summary>
        /// Set the default qty  for the strategy
        /// </summary>
        double Qty
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the max floor
        /// </summary>
        double? MaxFloor
        {
            get;
            set;
        }

        /// <summary>
        /// Set the default price  for the strategy
        /// </summary>
        double Price
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the stop PX for the strategy - note the behaviour
        /// depends on the type of strategy, in general this works only for
        /// single legs
        /// </summary>
        double StopPx
        {
            get;
            set;
        }

        /// <summary>
        /// Set the qty limit for the strategy
        /// </summary>
        double QtyLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the maximum number of iterations that are allowed in any
        /// run of the strategy - for example how many scalp orders sets
        /// can be placed
        /// </summary>
        int MaxIterations
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the maximum number of times the strategy may be entered - this is reset
        /// when a strategy is loaded or created, unlike max iterations which is the max number of
        /// runs/orders perminted in each entry, this is the max number of times you
        /// can enter the strategy. It defaults to -1 i.e. not limited
        /// </summary>
        int MaxEntries
        {
            get;
            set;
        }

        /// <summary>
        /// Max price for the strategy: max price allowed on any orders from the strategey - confirm that the algo used supports this
        /// </summary>
        double  MaxPrice
        {
            get;
            set;
        }

        /// <summary>
        /// Min price for the strategy: min price allowed on any orders from the strategey - confirm that the algo used supports this
        /// </summary>
        double  MinPrice
        {
            get;
            set;
        }

        /// <summary>
        /// Determines if we flatten position on Exit
        /// </summary>
        bool FlattenOnExit
        { get; set; }

        /// <summary>
        /// Determines if we cancels working orders  on Exit
        /// </summary>
        bool CancelOnExit
        { get; set; }

        /// <summary>
        /// determine if we use the strategies start and end times
        /// </summary>
        bool UseStrategyTimes
        { get; set; }

        /// <summary>
        /// Time of day the strategy can run from(start time) - if specified not time limits
        /// </summary>
        DateTime StartTime
        { get; set; }

        /// <summary>
        /// Time of day the strategy can run to(end time) - if specified not time limits
        /// if the strategy is an enter state when the end time passes the strategy will
        /// Exit and obey any exit rules in force
        /// </summary>
        DateTime EndTime
        { get; set; }

        /// <summary>
        /// A signal name to the list of names we support - this
        /// can be used to control what signals are processed by the strategy
        /// </summary>
        /// <returns></returns>
        List<string> SignalNames
        {
            get;
            set;
        }

        string EntryTriggerName
        {
            get;
            set;
        }
        string ExitTriggerName
        {
            get;
            set;
        }

        /// <summary>
        /// get/set whether the strategy will attempt to auto connect the enter and exit
        /// trigger names to constions in the trade venue
        /// </summary>
        bool AutoConnectTrg
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the interval used on conditions
        /// </summary>
        int ConditionInterval
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the enabled flag for the strategy
        /// </summary>
        bool Enabled
        {
            get;
            set;
        }

        /// <summary>
        /// Free format info about the strategy - for example output from an Algo
        /// </summary>
        string Info
        {
            get;
            set;
        }

        /// <summary>
        /// Has the strategy been initialized?
        /// </summary>
        bool Initialized
        {
            get;
            set;
        }
    }
}
