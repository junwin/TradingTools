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
    /// Defines the interface that an object used to provide status message processing
    /// must impliment
    /// </summary>
    public interface MessageHelper
    {
        /// <summary>
        /// Get/Set the driver that we are helping - needed for access to the client list
        /// </summary>
        KaiTrade.Interfaces.Driver Driver
        {
            get;
            set;
        }
        /// <summary>
        /// Send a simple advisory message to all clients of the
        /// adapter
        /// </summary>
        /// <param name="myMessageText"></param>
        void SendAdvisoryMessage(string myMessageText);

        /// <summary>
        /// Send some message back to our clients
        /// </summary>
        /// <param name="myMessage"></param>
        void SendMessage(KaiTrade.Interfaces.Message myMessage);

         /// <summary>
        /// Send a FIX style response to our clients
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="myResponseMsg"></param>
        void sendResponse(string msgType, string myResponseMsg);

        /// <summary>
        /// Resend the last status message
        /// </summary>
        void SendLastStatusMessage();

        /// <summary>
        /// Send a status message to all of our clients
        /// </summary>
        /// <param name="myMessage"></param>
        void SendStatusMessage(KaiTrade.Interfaces.Message myMessage);

         /// <summary>
        /// Send a FIX style status message specifing all parameters
        /// </summary>
        /// <param name="myState"></param>
        /// <param name="myText"></param>
        /// <param name="myBegin"></param>
        /// <param name="mySID"></param>
        /// <param name="myTID"></param>
        /// <param name="myFixName"></param>
        void SendStatusMessage(KaiTrade.Interfaces.Status myState, string myText, string myBegin, string mySID, string myTID, string myFixName);

        /// <summary>
        /// send a stuts message
        /// </summary>
        /// <param name="myState"></param>
        /// <param name="myText"></param>
        void SendStatusMessage(KaiTrade.Interfaces.Status myState, string myText);
    }
}
