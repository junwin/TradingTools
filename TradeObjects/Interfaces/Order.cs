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
    public enum LastOrderCommand { neworder, cancel, replace, none };
    /// <summary>
    /// Defines the behaviour of orders used in KaiTrade
    /// </summary>
    public interface Order
    {
        /// <summary>
        /// Gets the identity of the order.
        /// </summary>
        /// <value>The identity.</value>
        string Identity
        {
            get;
        }

        /// <summary>
        /// Get/Set the orders product
        /// </summary>
        TradableProduct Product
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set user's session ID that the order belongs to - note that a user can have N sessions over some time period
        /// </summary>
        string SessionID
        { get; set; }


        /// <summary>
        /// Get/Set user identity (a guid) that the order belongs to
        /// </summary>
        string User
        { get; set; }


        /// <summary>
        /// Get/Set the orders parent, will be the identity of some strategy
        /// </summary>
        string ParentIdentity
        {
            get;
            set;
        }

        /// <summary>
        /// ID of order trigger - see triggeredOrders
        /// </summary>
        string TriggerOrderID
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the group(if any) that the order belongs to, this is used when orders are
        /// related for example in pairs trading - note that a Strategy can contain
        /// one of more OrderGroups
        /// </summary>
        KaiTrade.Interfaces.OrderGroup OrderGroup
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the name of the group of OCO orders that this order
        /// belongs to.
        /// </summary>
        string OCAGroupName
        {
            get;
            set;
        }

        /// <summary>
        /// Used to link a set of orders that are executed under an OCO group, this is used to allow one of the
        /// initial orders in the group to spawn other orders, provided these have the same LinkName they will
        /// be treated like the inital order, an example of use would be a Profit Taking order, that places the total
        /// qty desired in N tranches, each of the orders it submits can be treated like the original order
        /// </summary>
        string OCAOrderLinkName
        { get; set; }

        /// <summary>
        /// Gets or sets user defined tag
        /// </summary>
        /// <value>The tag text.</value>
        string Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Time in ticks that the order expires - used in some
        /// algos to expire groups that for example have not completed in a given period
        /// of time
        /// </summary>
        long Expiration
        { get; set;}

        /// <summary>
        /// Specifies if the order is an AutoTradeOrder
        /// </summary>
        bool IsAutoTrade
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the autotradecount
        /// </summary>
        int AutoTradeProcessCount
        {
            get;

            set;
        }

        /// <summary>
        /// Gets or sets the side.
        /// </summary>
        /// <value>The side.</value>
        QuickFix.Side Side
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
        /// Required for short sell orders
        /// </summary>
        QuickFix.LocateReqd LocateReqd
        {
            get;
            set;
        }
        /// <summary>
        /// Identification of a Market Maker’s location
        /// </summary>
        QuickFix.LocationID LocationID
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the type of the order.
        /// </summary>
        /// <value>The type of the order.</value>
        QuickFix.OrdType OrdType
        {
            get;
            set;
        }

        /// <summary>
        /// get set the extended order type (KTRLSTOP etc..)
        /// </summary>
        string ExtendedOrdType
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set any parameters asscociated with the extended order type
        /// the order and usage of the parameters depends on the type
        /// </summary>
        string[] ExtendedOrdTypeParameters
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the exchange order id.
        /// </summary>
        /// <value>The exchange order id.</value>
        QuickFix.OrderID OrderID
        {
            get;

            set;
        }

        /// <summary>
        /// Gets or sets the fix client order id - this is the ID allocated by the K2Accelerator
        /// it is not a user defined order ID, for an ID set by the end user see K2ClientAssignedID
        /// </summary>
        /// <value>The client order id.</value>
        QuickFix.ClOrdID ClOrdID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the original fix client order id.
        /// </summary>
        /// <value>The original client order id.</value>
        QuickFix.OrigClOrdID OrigClOrdID
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or Sets an identity assigned by the user (trader, app) to identify
        /// an  order
        /// </summary>
        /// <value>The user assigned  order id.</value>
        string  ClientAssignedID
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the NOS extra field bag - these are added to any outbound NOS fix message
        /// </summary>
        List<K2Parameter> NOSBag
        {
            get;

            set;
        }
        /// <summary>
        /// Get/Set the Cancel extra field bag - these are added to any outbound NOS fix message
        /// </summary>
        List<K2Parameter> CancelBag
        {
            get;

            set;
        }

        /// <summary>
        /// Get/Set the Cancel extra field bag - these are added to any outbound NOS fix message
        /// </summary>
        List<K2Parameter> ReplaceBag
        {
            get;

            set;
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the trade venue.
        /// </summary>
        /// <value>The venue.</value>
        string TradeVenue
        {
            get;
            set;
        }

        /// <summary>
        /// High limit for the autotrade object
        /// </summary>
        double HighLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Low limit for the autotrade object
        /// </summary>
        double LowLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Quantity limit for the autotrade object
        /// </summary>
        double QuantityLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Quantity Delta for the autotrade object
        /// </summary>
        double QuantityDelta
        {
            get;
            set;
        }

        /// <summary>
        /// Price Delta for the autotrade object
        /// </summary>
        double PriceDelta
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the fills list.
        /// </summary>
        /// <value>The fills list.</value>
        List<Fill> FillsList
        {
            get;
        }

        /// <summary>
        /// Get/set the currently executed quantity for chain of orders.
        /// </summary>
        QuickFix.CumQty CumQty
        {
            get;

            set;
        }

        /// <summary>
        /// The amount filled in the last execution
        /// </summary>
        QuickFix.LastQty LastQty
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the LastPX from the last fill/execution report
        /// </summary>
        QuickFix.LastPx LastPx
        {
            get;

            set;
        }

        /// <summary>
        /// Get/Set the AvgPx for the fills so far
        /// </summary>
        QuickFix.AvgPx AvgPx
        {
            get;

            set;
        }

        /// <summary>
        /// Consideration AvgPx*amount filled
        /// </summary>
        double Consideration
        {
            get;
        }

        /// <summary>
        /// the quantity open for further execution (order qty - cum qty)
        /// </summary>
        QuickFix.LeavesQty LeavesQty
        {
            get;

            set;
        }

        /// <summary>
        /// Gets or sets the account code
        /// </summary>
        /// <value>The account number.</value>
        QuickFix.Account Account
        {
            get;

            set;
        }

        /// <summary>
        /// get/set the hanlding instructions
        /// </summary>
        QuickFix.HandlInst HandlInst
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Mnemonic.
        /// </summary>
        /// <value>The monika.</value>
        string Mnemonic
        {
            get;

            set;
        }

        /// <summary>
        /// Gets or sets the time in force.
        /// </summary>
        /// <value>The time in force.</value>
        QuickFix.TimeInForce TimeInForce
        {
            get;
            set;
        }

        /// <summary>
        /// Required for GTD orders
        /// </summary>
        QuickFix.ExpireDate ExpireDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        /// <value>The price.</value>
        QuickFix.Price Price
        {
            get;

            set;
        }

        /// <summary>
        /// Gets or sets the stop price.
        /// </summary>
        /// <value>The price.</value>
        QuickFix.StopPx StopPx
        {
            get;

            set;
        }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>The quantity.</value>
        QuickFix.OrderQty OrderQty
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets Maximum quantity (e.g. number of shares) within an order to be shown
        /// on the exchange floor at any given time.
        /// </summary>
        /// <value>The quantity target.</value>
        QuickFix.MaxFloor MaxFloor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the order transact time.
        /// </summary>
        /// <value>The order time.</value>
        QuickFix.TransactTime TransactTime
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the list of strategy parameters for the order - based on FIX Protocal ADTL
        /// </summary>
        List<KaiTrade.Interfaces.K2Parameter> K2Parameters
        { get; set; }

        /// <summary>
        /// Gets or sets the name of the strategy.
        /// </summary>
        /// <value>The name of the strategy.</value>
        string StrategyName
        {
            get;
            set;
        }

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
        /// Gets or sets the state of the order.
        /// </summary>
        /// <value>The state of the order.</value>
        QuickFix.OrdStatus OrdStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Is the order pending a replace request
        /// </summary>
        LastOrderCommand LastCommand
        { get; set; }

        /// <summary>
        /// Get/Set freeform text associated with the order
        /// </summary>
        string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Set up order from an XML data binding
        /// </summary>
        /// <param name="myOrder"></param>
        void FromXMLDB(KAI.kaitns.Order myOrder);

        /// <summary>
        /// Set up an order from order data and fills
        /// </summary>
        /// <param name="myOrdData">ordder data</param>
        /// <param name="fills">list of fills associated with the order</param>
        void From(KaiTrade.Interfaces.OrderData myOrdData, List<KaiTrade.Interfaces.Fill> fills);

        /// <summary>
        /// write order onto an XML data bining
        /// </summary>
        /// <returns></returns>
        KAI.kaitns.Order ToXMLDB();

        /// <summary>
        /// Return an order as tab separated
        /// </summary>
        /// <returns></returns>
        string ToTabSeparated();

        /// <summary>
        /// Returns if the order is working i.e. it can be canceled or modified
        /// </summary>
        /// <returns></returns>
        bool IsWorking();

        /// <summary>
        /// Get the PnL for the order provided at this time(current market prices) if the orders
        /// product is subscribed - will throw an exception if the product is not valid.
        /// Note this is not realtime, its calculated when called
        /// </summary>
        double GetCurrentPNL();

        /// <summary>
        /// Get the PnL for the order provided at this time(current market prices) using the
        /// fill prices passed in.- will throw an exception if the product is not valid.
        /// This is used to calc the PNL implications of trading working qty at market price
        /// Note this is not realtime, its calculated when called
        /// </summary>
        /// <param name="AvgPx">Assumed fill prices</param>
        /// <returns></returns>
        double GetCurrentPNL(double AvgPx);
    }

    /// <summary>
    /// Defines the interface a data object must support - this is used in WCF and serializing
    /// </summary>
    public interface OrderData
    {
        /// <summary>
        /// Gets the identity of the order.
        /// </summary>
        /// <value>The identity.</value>
        string Identity
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the orders parent, will be the identity of some strategy
        /// </summary>
        string ParentIdentity
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set user identity that the order belongs to
        /// </summary>
        string User
        { get; set; }

        /// <summary>
        /// Get/Set user's session ID that the order belongs to - note that a user can have N sessions over some time period
        /// </summary>
        string SessionID
        { get; set; }


       


        /// <summary>
        /// ID of order trigger - see triggeredOrders
        /// </summary>
        string TriggerOrderID
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the name of the group of OCO orders that this order
        /// belongs to.
        /// </summary>
        string OCAGroupName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets user defined tag
        /// </summary>
        /// <value>The tag text.</value>
        string Tag
        {
            get;
            set;
        }

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
        /// Time in ticks that the order expires - used in some
        /// algos to expire groups that for example have not completed in a given period
        /// of time
        /// </summary>
        long Expiration
        { get; set; }

        /// <summary>
        /// Specifies if the order is an AutoTradeOrder
        /// </summary>
        bool IsAutoTrade
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the autotradecount
        /// </summary>
        int AutoTradeProcessCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the side - this is the long string version (not the FIX code) e.g. BUY SELL
        /// </summary>
        /// <value>The side.</value>
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
        /// Required for short sell orders
        /// </summary>
        string LocateReqd
        {
            get;
            set;
        }
        /// <summary>
        /// Identification of a Market Maker’s location
        /// </summary>
        string LocationID
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the type of the order- this is the long string version (not the FIX code) e.g. LIMIT MARKET
        /// </summary>
        /// <value>The type of the order.</value>
        string OrdType
        {
            get;
            set;
        }

        /// <summary>
        /// get set the extended order type (KTRLSTOP etc..)
        /// </summary>
        string ExtendedOrdType
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set any parameters asscociated with the extended order type
        /// the order and usage of the parameters depends on the type
        /// </summary>
        string[] ExtendedOrdTypeParameters
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the exchange order id.
        /// </summary>
        /// <value>The exchange order id.</value>
        string OrderID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the client order id.
        /// </summary>
        /// <value>The client order id.</value>
        string ClOrdID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the original fix client order id this is assigned by KaiTrade - see ClientAssignedID for
        /// a user assigned ID.
        /// </summary>
        /// <value>The original fix client order id.</value>
        string OrigClOrdID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets an identity assigned by the user (trader, app) to identify
        /// an  order
        /// </summary>
        /// <value>The user assigned  order id.</value>
        string ClientAssignedID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the trade venue.
        /// </summary>
        /// <value>The venue.</value>
        string TradeVenue
        {
            get;
            set;
        }

        /// <summary>
        /// Quantity limit for the autotrade object
        /// </summary>
        double QuantityLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Get/set the currently executed quantity for chain of orders.
        /// </summary>
        double CumQty
        {
            get;

            set;
        }

        /// <summary>
        /// The amount filled in the last execution
        /// </summary>
        double LastQty
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the LastPX from the last fill/execution report
        /// </summary>
        double LastPx
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the AvgPx for the fills so far
        /// </summary>
        double AvgPx
        {
            get;
            set;
        }

        /// <summary>
        /// the quantity open for further execution (order qty - cum qty)
        /// </summary>
        double LeavesQty
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the account code
        /// </summary>
        /// <value>The account number.</value>
        string Account
        {
            get;
            set;
        }

        /// <summary>
        /// get/set the hanlding instructions
        /// </summary>
        string HandlInst
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Mnemonic.
        /// </summary>
        /// <value>The monika.</value>
        string Mnemonic
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time in force.
        /// </summary>
        /// <value>The time in force.</value>
        string TimeInForce
        {
            get;
            set;
        }

        /// <summary>
        /// Required for GTD orders
        /// </summary>
        string ExpireDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        /// <value>The price.</value>
        double Price
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the stop price.
        /// </summary>
        /// <value>The price.</value>
        double StopPx
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>The quantity.</value>
        long OrderQty
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets Maximum quantity (e.g. number of shares) within an order to be shown
        /// on the exchange floor at any given time.
        /// </summary>
        /// <value>The quantity target.</value>
        long MaxFloor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the order transact time.
        /// </summary>
        /// <value>The order time.</value>
        string TransactTime
        {
            get;
            set;
        }
        /// <summary>
        /// Date time processed/stored in DB
        /// </summary>
        DateTime SystemDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the strategy.
        /// </summary>
        /// <value>The name of the strategy.</value>
        string StrategyName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the algo of the strategy.
        /// </summary>
        /// <value>The name of the strategy.</value>
        string AlgoName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the state of the order.
        /// </summary>
        /// <value>The state of the order.</value>
        string OrdStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set freeform text associated with the order
        /// </summary>
        string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the list of strategy parameters for the order - based on FIX Protocal ADTL
        /// </summary>
        List<KaiTrade.Interfaces.K2Parameter> K2Parameters
        { get; set; }
    }
}
