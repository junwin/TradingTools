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
    /// This is used to manage a group of TS Data sets, an application can have many TSQueryGroups, each
    /// group will have 1 or more TS datasets. The group allows the user to access these at a higher level
    /// for example to get the data, load/store and so on
    /// </summary>
    public interface TSQueryGroup
    {
        /// <summary>
        /// Set up a TS Query group from XML
        /// </summary>
        /// <param name="myXML"></param>
        void FromXml(string myXML);

        

        /// <summary>
        /// Store a query group as XML
        /// </summary>
        /// <returns></returns>
        string GetXML();

       

        /// <summary>
        /// Get/Set the group name
        /// </summary>
        string Name
        { get; set;}

        /// <summary>
        /// ID of the strategy that this set is related to (if any)
        /// </summary>
        string StrategyName
        { get; set; }

        /// <summary>
        /// Get the status of a group of set - this will show
        /// the most severe error state of any of the groups sets
        /// open implies all are open
        /// </summary>
        KaiTrade.Interfaces.Status Status
        { get;}

        /// <summary>
        /// will set the mnemonic that used for all members of a query set
        /// </summary>
        string Mnemonic
        {
            get;
            set;
        }

        /// <summary>
        /// This will override the intraday interval used in individual queries
        /// </summary>
        /// <param name="myInterval"></param>
        void SetInterval(int myInterval);

        /// <summary>
        /// Add a subscriber to the query group - any sets that have a publisher interface
        /// will be subscribed to
        /// </summary>
        /// <param name="mySubscriber"></param>
        void addSubscriber(KaiTrade.Interfaces.ISubscriber mySubscriber);

        /// <summary>
        /// Unsubscribe to the  to the query group - any sets that have a publisher interface
        /// will be unsubscribed
        /// </summary>
        /// <param name="mySubscriber"></param>
        void UnSubscribe(KaiTrade.Interfaces.ISubscriber mySubscriber);

        

        /// <summary>
        /// get/set list of TS Sets
        /// </summary>
        List<KaiTrade.Interfaces.TSSet> Items
        { get; set;}
    }
}
