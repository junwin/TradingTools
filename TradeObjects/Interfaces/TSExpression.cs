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
    /// Defines the format of the expression - Native
    /// </summary>
    public enum TSFormat
    {
        KTA, DriverSpecific
    }

    /// <summary>
    /// Identifies some expression use to get
    /// time series data from a provider
    /// </summary>
    public interface TSExpression
    {
        /// <summary>
        /// Get/Set the format of this expression
        /// </summary>
        TSFormat Format
        { get; set;}

        /// <summary>
        /// Some expression to be evaluated by the driver or provider
        /// of TS data
        /// </summary>
        string Expression
        { get; set;}

        /// <summary>
        /// Allias used to identify the result of some expression - e.g. for RTD
        /// </summary>
        string Alias
        { get; set; }

        /// <summary>
        /// Name of the Base expression/alg/calc to be used
        /// </summary>
        string BaseExpression
        { get;}

        /// <summary>
        /// Expression alg or calculation used to evaluate this expression
        /// </summary>
        StatExpression StatExpression
        { get; set; }
    }
}
