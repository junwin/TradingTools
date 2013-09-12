//-----------------------------------------------------------------------
// <copyright file="IOrder.cs" company="KaiTrade LLC">
// Copyright (c) 2013, KaiTrade LLC.
//// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>// <author>John Unwin</author>
// <website>https://github.com/junwin/K2RTD.git</website>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace KaiTrade.Interfaces
{
    public enum LastOrderCommand { neworder, cancel, replace, none };
    
    /// <summary>
    /// Defines the interface a data object must support - this is used in WCF and serializing
    /// </summary>
    public interface IOrder
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
        /// The last commad processed for this order
        /// </summary>
        KaiTrade.Interfaces.LastOrderCommand LastOrderCommand
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
        List<KaiTrade.Interfaces.IParameter> K2Parameters
        { get; set; }
    }
}
