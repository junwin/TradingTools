//-----------------------------------------------------------------------
// <copyright file="IMessage.cs" company="KaiTrade LLC">
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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Defines an internal message used
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// unique ID for this message
        /// </summary>
        string Identity
        { get; set; }

        /// <summary>
        /// Message data JSON, FIX, FIXML
        /// </summary>
        string Data
        {
            get;
            set;
        }

        /// <summary>
        /// CorrelationID used for tiing messages together - not that same as the Trade/Algo CorrelationID
        /// </summary>
        string CorrelationID
        {
            get;
            set;
        }

        /// <summary>
        /// Message label - NewOrder,...
        /// </summary>
        string Label
        {
            get;
            set;
        }

        /// <summary>
        /// get/set TargetID (destination) for the message  
        /// </summary>
        string TargetID
        {
            get;
            set;
        }

        /// <summary>
        /// get/set Client SubID
        /// </summary>
        string ClientSubID
        {
            get;
            set;
        }

        /// <summary>
        /// get/set target subID
        /// </summary>
        string TargetSubID
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set format - JSON, FIX, FIXML other
        /// </summary>
        string Format
        {
            get;
            set;
        }
        /// <summary>
        /// Get/Set AppSpecific  
        /// </summary>
        long AppSpecific
        {
            get;
            set;
        }
        /// <summary>
        /// Get/Set AppState  
        /// </summary>

        int AppState
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set App type  
        /// </summary>
        string AppType
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set Tag - user tag
        /// </summary>
        string Tag
        {
            get;
            set;
        }
        /// <summary>
        /// get/Set ClientID
        /// </summary>
        string ClientID
        {
            get;
            set;
        }
        /// <summary>
        /// get/Set user ID  
        /// </summary>
        string UserID
        {
            get;
            set;
        }

        /// <summary>
        /// get set the Venue Code that the message is intended for
        /// </summary>
        string VenueCode
        {
            get;
            set;
        }

        /// <summary>
        /// get set creation time  
        /// </summary>
        string CreationTime
        {
            get;
            set;
        }
    }
}
