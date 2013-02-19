/***************************************************************************
 *
 *      Copyright (c) 2009 KaiTrade LLC (registered in Delaware)
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
    /// Defines trade limits applied within KaiTrade when routing orders
    /// </summary>
    public interface TradeLimits
    {
        /// <summary>
        /// Max size allowed on an individual order
        /// </summary>
        double MaxOrderSize
        { get; set;}

        /// <summary>
        /// Max consideration (qty*price) allowed on an individual order
        /// </summary>
        double MaxOrderConsideration
        { get; set;}

        /// <summary>
        /// Max size allowed to be submitted in a given KaiTrade session
        /// </summary>
        double MaxSize
        { get; set;}

        /// <summary>
        /// Max Consideration in a given given KaiTrade session
        /// </summary>
        double MaxConsideration
        { get; set;}

        /// <summary>
        /// Max number of messages/iterations in a particular run of an Algo
        /// </summary>
        int MaxMessages
        { get; set;}

        /// <summary>
        /// Set values from a string of XML
        /// </summary>
        /// <param name="myXML"></param>
        void FromXML(string myXML);

        /// <summary>
        /// Return values as a string of XML
        /// </summary>
        /// <returns></returns>
        string ToXML();
    }
}
