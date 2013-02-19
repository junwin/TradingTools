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
    /// Interface of an object representing a KaiTrade user
    /// </summary>
    public interface User
    {
        /// <summary>
        /// DO NOT USE Get the unique identifer for the user allocated by the system
        /// </summary>
        string ID
        { get; set; }

        /// <summary>
        /// Get set some user id identity - do not use Identity
        /// </summary>
        string UserID
        { get; set; }

        /// <summary>
        /// User sign on name
        /// </summary>
        string UserName
        { get; set; }

        /// <summary>
        /// Users password
        /// </summary>
        string UserPwd
        { get; set; }

        /// <summary>
        /// Is the user enabled
        /// </summary>
        bool Enabled
        { get; set; }

        /// <summary>
        /// Get/Set the users K2 config, determines access, servcies, plugings and drivers
        /// </summary>
        string K2Config
        { get; set; }

        /// <summary>
        /// Date time the user last signed in
        /// </summary>
        DateTime LastSignIn
        { get; set; }

        /// <summary>
        /// IP address of last sign in
        /// </summary>
        string LastIP
        { get; set; }

        /// <summary>
        /// Are they signed in true=> signed in
        /// </summary>
        bool IsSignedIn
        { get; set; }

        /// <summary>
        /// User's email
        /// </summary>
        string Email
        { get; set; }
    }
}
