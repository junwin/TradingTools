//-----------------------------------------------------------------------
// <copyright file="IUser.cs" company="KaiTrade LLC">
// Copyright (c) 2013, KaiTrade LLC.
//// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>// <author>John Unwin</author>
// <website>https://github.com/junwin/K2RTD.git</website>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Interface of an object representing a KaiTrade user
    /// </summary>
    public interface IUser
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
