/***************************************************************************
 *
 *      Copyright (c) 2009 KaiTrade LLC (registered in Delaware)
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
    /// Defines an calculation algo that will act on the legs of a
    /// synthetic product to calculate prices for the product
    /// </summary>
    public interface SyntheticPriceCalc
    {
        /// <summary>
        /// return the calc name
        /// </summary>
        string Name
        { get;}

        /// <summary>
        /// Get/Set the product that this algo will calculate prices for
        /// </summary>
        KaiTrade.Interfaces.TradableProduct Product
        { get; set;}

        /// <summary>
        /// Recalculate prices for all legs
        /// </summary>
        void ReCalculate();

        /// <summary>
        /// Apply a leg price update
        /// </summary>
        /// <param name="myLeg"></param>
        void LegUpdate(KaiTrade.Interfaces.Leg myLeg);
    }
}
