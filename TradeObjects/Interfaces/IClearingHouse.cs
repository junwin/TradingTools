/***************************************************************************
 *
 *      Copyright (c) 2009,2010,2011 KaiTrade LLC (registered in Delaware)
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
using System.Linq;
using System.Text;

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Identifies a clearing house affiliated to some exchange/exchanges e.g. OCC, LCH etc
    /// </summary>
    public interface IClearingHouse
    {
        /// <summary>
        /// unique identifier for the clearing house
        /// </summary>
        string Identity
        { get; set; }

        /// <summary>
        /// Long Name of the clearing house
        /// </summary>
        string Name
        { get; set; }


        /// <summary>
        /// short code for the clearing house
        /// </summary>
        string Code
        { get; set; }

    }
}
