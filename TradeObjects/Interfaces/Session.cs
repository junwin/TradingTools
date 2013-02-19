/***************************************************************************
 *
 *      Copyright (c) 2009,2010 KaiTrade LLC (registered in Delaware)
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
    /// Interface of an object that represents a sessino between a user of
    /// the system and a remote instance of KaiTrade
    /// </summary>
    public interface Session
    {
        /// <summary>
        /// Get the unique identifer for the sesssion
        /// </summary>
        string Identity
        { get; set; }

        /// <summary>
        /// Get/Set the user associated with the session
        /// </summary>
        string UserID
        { get;set;}

        /// <summary>
        /// Set the session correlation ID
        /// </summary>
        string CorrelationID
        { get; set; }
    }
}
