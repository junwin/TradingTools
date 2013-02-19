/***************************************************************************
 *
 *      Copyright (c) 2009,2010,2011,2012 KaiTrade LLC (registered in Delaware)
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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Defines an internal message used
    /// </summary>
    public interface Message
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
