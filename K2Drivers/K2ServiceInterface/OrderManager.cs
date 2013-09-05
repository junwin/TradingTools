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
    /// Defines an interface that any object manageing a set of
    /// orders for KaiTrade must impliment
    /// </summary>
    public interface IOrderManager
    {
        /// <summary>
        /// Create a new order and register it in the manager
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.IOrder CreateOrder();

        /// <summary>
        /// Register an order with the manager - used when the order is created elsewhere
        /// e.g. when loaded from a file. You should normally use CreateOrder to get an Order
        /// </summary>
        /// <param name="?"></param>
        void RegisterOrder(KaiTrade.Interfaces.IOrder myOrder);

        /// <summary>
        /// Invert the side passed in
        /// </summary>
        /// <param name="mySide"></param>
        /// <returns></returns>
        string InvertSide(string mySide);

        /// <summary>
        /// Associate a ClOrdID with a particular order
        /// </summary>
        /// <param name="myClOrdID"></param>
        /// <param name="myOrder"></param>
        void AssociateClOrdID(string clOrdID,KaiTrade.Interfaces.IOrder order);

        /// <summary>
        /// Get an order for the ID passed in
        /// </summary>
        /// <param name="myOrderID"></param>
        /// <returns></returns>
        KaiTrade.Interfaces.IOrder GetOrder(string myOrderID);

        /// <summary>
        /// Get an order using a CLOrdID
        /// </summary>
        /// <param name="myID"></param>
        /// <returns></returns>
        KaiTrade.Interfaces.IOrder GetOrderWithClOrdIDID(string myID);

        /// <summary>
        /// Delete the order from manager
        /// </summary>
        /// <param name="myID"></param>
        void DeleteOrder(string myID);

        /// <summary>
        /// Get a list of order ID's
        /// </summary>
        /// <returns></returns>
        List<string> GetOrderIDS();

        /// <summary>
        /// Get list of order IDs for the specified user
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        List<string> GetOrderIDS(string UserID);

        /// <summary>
        /// Load orders from a file
        /// </summary>
        /// <param name="myFilePath"></param>
        void LoadOrdersFromFile(string myFilePath);

        /// <summary>
        /// Store Orders in a file
        /// </summary>
        /// <param name="myFilePath"></param>
        void StoreOrdersToFile(string myFilePath);

        /// <summary>
        /// Store orders to a file of JSON
        /// </summary>
        /// <param name="myOrderFilePath">file to write orders into</param>
        /// <param name="myFillsFilePath">file to write fill data into</param>
        void ToFileJSON(string myOrderFilePath, string myFillsFilePath);

        /// <summary>
        /// Return an order as tab separated
        /// </summary>
        /// <returns></returns>
        string ToTabSeparated();

        /// <summary>
        /// indicate that the order has changed
        /// </summary>
        /// <param name="myID"></param>
        void SetChanged(string myID);

        /// <summary>
        /// Record an incomming execution report - will return false
        /// if the report has been processed already
        /// </summary>
        /// <param name="myID"></param>
        /// <param name="myExecRep"></param>
        /// <returns>false if report exists</returns>
        bool RecordExecutionReport(string myID, KaiTrade.Interfaces.IFill fill);
    }
}
