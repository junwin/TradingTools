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
    /// Interface of a on object that manages a set of sessions
    /// </summary>
    public interface SessionManager
    {
        /// <summary>
        /// Create an empty session and register it in the manager
        /// </summary>
        /// <param name="correlationID">Identifier to the physical session in
        /// some service or adapter</param>
        /// <returns></returns>
        KaiTrade.Interfaces.Session CreateSession(string correlationID);

        /// <summary>
        /// Get a list of session IDs
        /// </summary>
        /// <returns></returns>
        List<string> GetSessionIDs();

        /// <summary>
        /// Get a session given its unique ID
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.Session GetSession(string sessionID);

        /// <summary>
        /// Get the session for some correlation ID
        /// </summary>
        /// <param name="correlationID"></param>
        /// <returns></returns>
        KaiTrade.Interfaces.Session GetSessionWithCorrelationID(string correlationID);

        /// <summary>
        /// Get the current session for some user identity
        /// </summary>
        /// <param name="correlationID"></param>
        /// <returns></returns>
        KaiTrade.Interfaces.Session GetUserCurrentSession(string userIdentity);

        /// <summary>
        /// Associate a user and correlationID into a session - will create the session if needed
        /// </summary>
        /// <param name="userIdentity"></param>
        /// <param name="correlationID"></param>
        /// <returns>session that maps the user to a correlationID</returns>
        KaiTrade.Interfaces.Session AssociateUserCorrelation(string userIdentity, string correlationID);

        /// <summary>
        /// Associate a session with a user
        /// </summary>
        /// <param name="sessionID">session id of the session to associate - must exist</param>
        /// <param name="userIdentity">user identity to associate with</param>
        void AssociateSessionWithUser(string sessionID, string userIdentity);
    }
}
