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
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace K2DataObjects
{
    public class DataContext : System.Data.Linq.DataContext
    {
        public System.Data.Linq.Table<Trade> Trades;
        public System.Data.Linq.Table<Order> Orders;
        public System.Data.Linq.Table<Fill> Fills;
        public System.Data.Linq.Table<TradeVenue> Venues;
        public System.Data.Linq.Table<User> Users;
        public System.Data.Linq.Table<Session> Sessions;
        public System.Data.Linq.Table<Account> Accounts;

        public System.Data.Linq.Table<Firm> Firms;
        public System.Data.Linq.Table<Exchange> Exchanges;

        public System.Data.Linq.Table<Product> Products;

        //public System.Data.Linq.Table<AccountAllocation> AccountAllocations;

        //public System.Data.Linq.Table<Allocation> Allocations;
        //public System.Data.Linq.Table<StrategyData> Strategies;
        public System.Data.Linq.Table<Server> Servers;
        public System.Data.Linq.Table<TradeVenueDestination> VenueDestinations;

        public System.Data.Linq.Table<UserTrial> UserTrials;
        public System.Data.Linq.Table<ClearingHouse> ClearingHouses;
        public System.Data.Linq.Table<OrderTrade> OrderTrades;

        public System.Data.Linq.Table<MQExchange> MQExchanges;
        public System.Data.Linq.Table<MQRoutingKey> MQRoutingKeys;
        public System.Data.Linq.Table<TradeSignal> TradeSignals;
        public System.Data.Linq.Table<Message> Messages;

        //public System.Data.Linq.Table<PlugInData> PlugIns;
        //public System.Data.Linq.Table<DriverData> Drivers;
        //public System.Data.Linq.Table<Extension> Extensions;
        

        public DataContext(string connection) : base(connection) { }
    }

    public class PriceBarDataContext : System.Data.Linq.DataContext
    {
        public System.Data.Linq.Table<PriceBar> PriceBars;
        //public System.Data.Linq.Table<CurveValue> CurveValues;
        


        public PriceBarDataContext(string connection) : base(connection) { }
    }
}
