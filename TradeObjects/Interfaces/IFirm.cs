//-----------------------------------------------------------------------
// <copyright file="IFirm.cs" company="KaiTrade LLC">
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
    /// defines the type of firm
    /// </summary>
    public enum FirmType { clearingMember, member, fcm, ib, none }

    /// <summary>
    /// Models some financial instritution - for example an FCM or broker
    /// </summary>
    public interface IFirm
    {
        /// <summary>
        /// Get set the unique identifier
        /// </summary>
        string ID
        {get; set;}

        /// <summary>
        /// Get set the unique identifier
        /// </summary>
        string FirmCode
        { get; set; }

        /// <summary>
        /// Get set the unique identifier
        /// </summary>
        string FirmName
        { get; set; }

        /// <summary>
        /// Set the type of firm
        /// </summary>
        FirmType FirmType
        { get; set; }

        /// <summary>
        /// defines if the firm is external - for example firm we give up a trade to
        /// </summary>
        bool External
        { get; set; }
    }
}
