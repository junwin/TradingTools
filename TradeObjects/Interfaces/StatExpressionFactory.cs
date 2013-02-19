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
    /// Interface that some factory that provides statistical caluclations (Averegaes, ATR etc)
    /// </summary>
    public interface StatExpressionFactory
    {
        /// <summary>
        /// Get an isntance of the nameed expression
        /// </summary>
        /// <param name="myName"></param>
        /// <returns></returns>
        KaiTrade.Interfaces.StatExpression GetStatExpression(string myName);

        /// <summary>
        /// Return a list of expression
        /// </summary>
        /// <returns></returns>
        List<string> GetStatExpressionNames();

        /// <summary>
        /// Get set the app facade that objects created by this factory can use
        /// </summary>
        KaiTrade.Interfaces.Facade AppFacade
        {
            get;
            set;
        }
    }
}
