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
    /// Defines the types of extended managers we support
    /// </summary>
    public enum extendedManagerType { order, product, user, account, allocation };

    /// <summary>
    /// Defines the interface(what functions) a factory object must provide
    /// for the Kaitrade system
    /// </summary>
    public interface Factory
    {
        /// <summary>
        /// get the main message processpr used by the app - this will
        /// handle all messages sent from the Drivers
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.Client GetMainMessageHandler();

        /// <summary>
        /// Set an extended manager, this will replace the default KaiTrade manager, for example
        /// to allow a server based product manager.
        /// Note that only one extended manager per type is allowed, if you add multiple the
        /// last one will be used
        /// </summary>
        /// <param name="managerType">type of manager</param>
        /// <param name="newManager">new manager to be used</param>
        void SetExtendedManager(extendedManagerType managerType, object newManager);

        /// <summary>
        /// Get the Driver manager
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.DriverManager GetDriverManager();

        /// <summary>
        /// Get the metadata manager - defines grids and ranges
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.MetaDataManager GetMetaDataManager();

        /// <summary>
        /// Get the venue manager
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.VenueManager GetVenueManager();

        /// <summary>
        /// Get the publisher Manager
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.PublisherManager GetPublisherManager();

        /// <summary>
        /// Get the Product Manager
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.ProductManager GetProductManager();

        /// <summary>
        /// Get the Order Manager
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.OrderManager GetOrderManager();

        /// <summary>
        /// Get the User Manager
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.UserManager GetUserManager();

        /// <summary>
        /// Get the Session Manager
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.SessionManager GetSessionManager();

        /// <summary>
        /// Get the PlugIn Manager
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.PlugInManager GetPlugInManager();

        /// <summary>
        /// Get the OCO Order manager
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.OCOManager GetOCOManager();

        /// <summary>
        /// Get the Triggered Order Manager
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.TriggeredOrderManager GetTriggeredOrderManager();

        /// <summary>
        /// Get the spread/pair trade manager
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.SpreadManager GetSpreadManager();

        /// <summary>
        /// Get the limit checker used on order submit and modify
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.LimitChecker GetLimitChecker();

        /// <summary>
        /// Get the limit checker used on order submit and modify
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.StrategyManager GetStrategyManager();

        /// <summary>
        /// Get the manager used to provide account information
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.AccountManager GetAccountManager();

        /// <summary>
        /// Get the manager used to manage allocations
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.AllocationManager GetAllocationManager();

        /// <summary>
        /// Get the manager for user's settings and options
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.UserOptionsManager GetUserOptionsManager();

        /// <summary>
        /// Get the position publisher
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.PositionPublisher GetPositionPublisher();

        /// <summary>
        /// Get the logger used to record system and other messages
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.Logger GetLogger();

        /// <summary>
        /// Get the Time series data manager
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.TSSetManager GetTSSetManager();

        /// <summary>
        /// get the trade signal manager
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.TradeSignalManager GetTradeSignalManager();

        /// <summary>
        /// get the trade system manager
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.ITradeSystemManager GetTradeSystemManager();

        /// <summary>
        /// create an order group
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.OrderGroup CreateOrderGroup();

        /// <summary>
        /// Return the algo manager - provides access to a range of algo in KaiTrade
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.AlgoManager GetAlgoManager();
    }
}
