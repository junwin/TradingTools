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
using System.ServiceModel;
//using System.Linq;
using System.Runtime.Serialization;

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Defines some data field
    /// </summary>
    public interface Field
    {
        /// <summary>
        /// Current value of the field
        /// </summary>
        string Value
        {
            get;
            set;
        }

        /// <summary>
        /// Field name/ID
        /// </summary>
        string ID
        {
            get;
            set;
        }

        /// <summary>
        /// Has the field been changed
        /// </summary>
        bool Changed
        {
            get;
            set;
        }

        /// <summary>
        /// Is the field in a valid state
        /// </summary>
        bool IsValid
        {
            get;
        }
    }

}
