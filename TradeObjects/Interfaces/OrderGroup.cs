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
    /// Provide a way to group a set of orders that must be processed
    /// by some algo as a group - note that a Strategy may have one or more
    /// order groups
    /// </summary>
    public interface OrderGroup
    {
        /// <summary>
        /// add an order to the group
        /// </summary>
        /// <param name="myOrder"></param>
        void AddOrder(KaiTrade.Interfaces.Order myOrder);

        /// <summary>
        /// get the unique id for the group
        /// </summary>
        string ID
        {
            get;
        }

        /// <summary>
        /// get a list of the groups orders
        /// </summary>
        List<KaiTrade.Interfaces.Order> Orders
        {
            get;
        }

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
        /// Get the LongWorking qty for the strategy
        /// </summary>
        double LongQty
        {
            get;
            set;
        }
        /// <summary>
        /// get the short pending qty
        /// </summary>
        double ShortQty
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
        /// Get/calculate the current PNL
        /// </summary>
        /// <returns></returns>
        double CalculateCurrentPNL();

        /// <summary>
        /// Get the PNL value based on last calculation
        /// </summary>
        double PNL
        {
            get;
        }

         /// <summary>
        /// return the average px of all the orders - note this
        /// only makes sense for the same product
        /// </summary>
        /// <returns></returns>
        double CalcAvgPx();

        /// <summary>
        /// get the long filled qty
        /// </summary>
        double LongFilledQty
        {
            get;
            set;
        }

        /// <summary>
        /// Get the user defined step numbers - typically used to apply a sequence of actions to the group
        /// </summary>
        int StepNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Time in ticks that the order group expires - used in some
        /// algos to expire groups that for example have not completed in a given period
        /// of time
        /// </summary>
        long Expiration
        { get; set;}

        /// <summary>
        /// get/set a user defined tag to the group
        /// </summary>
        string Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Detect if the ID is a member of the group
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        bool IsGroupMember(string ID);

        /// <summary>
        /// Returns whether or not a particular group of
        /// orders has completed i.e. that there are no more
        /// expected transactions for any of the individual orders
        /// </summary>
        /// <returns></returns>
        bool IsCompleted();

        /// <summary>
        /// Returns the number of orders in the group
        /// that are pending - i.e. not (or have not been)
        /// in the exchange book
        /// </summary>
        /// <returns></returns>
        int PendingOrderCount();

        /// <summary>
        /// Get the the total order qty for the group - i.e. the sum of
        /// all the individual order qtys
        /// </summary>
        double OrderQty
        { get;}

        /// <summary>
        /// Get the total working qty for the grouo i.e. the sum of all the working qtys (-ve => short)
        /// </summary>
        double WorkingQty
        { get;}

        /// <summary>
        /// Cancel any working orders in the group
        /// </summary>
        void Cancel();

        /// <summary>
        /// Edit the order for the ID specified
        /// </summary>
        /// <param name="newQty">new qty if specified</param>
        /// <param name="newPrice">new price if specified</param>
        /// <param name="newStopPrice">new stop price if specified</param>
        void ReplaceOrder(double? newQty, double? newPrice, double? newStopPx);

        /// <summary>
        /// Get a log of basic order information as array of strings
        /// </summary>
        /// <returns>Array of strings, each string is basic info of an order in the group</returns>
        string[] GetOrderLogInfo();
        
    }
}
