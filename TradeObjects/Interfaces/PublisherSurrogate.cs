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
using System.Linq;
using System.Text;

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// This represents a place holder for a regular publisher
    /// </summary>
    public interface PublisherSurrogate
    {
        KaiTrade.Interfaces.Publisher Publisher
        { get; set; }

        System.Collections.Generic.List<KaiTrade.Interfaces.Subscriber> Subscribers
        { get; set; }
    }
}
