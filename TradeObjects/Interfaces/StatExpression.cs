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
using System.Text;

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Defines the interface that a class performing statistical/numerical
    /// analysis on the set of TS data will provide
    /// </summary>
    public interface StatExpression
    {
        /// <summary>
        /// Get/Set the TSSet the statistic function will work on
        /// </summary>
        KaiTrade.Interfaces.TSSet TSSet
        {
            get;
            set;
        }

        /// <summary>
        /// Evaluate the expression on the current data, normally should use
        /// the last complete item/bar - setting bUseCurrent bar will cause
        /// the evaluation to include the bar being added to, that has yet to
        /// end.
        /// </summary>
        void EvaluateAdded(bool bUseCurrentBar);
        void EvaluateUpdate(bool bUseCurrentBar);

        /// <summary>
        /// Get Current Value
        /// </summary>
        double Value
        {
            get;
        }

        /// <summary>
        /// Expression name
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Paramters requred for the expression
        /// </summary>
        List<KaiTrade.Interfaces.K2Parameter> Parameters
        { get; set; }

        /// <summary>
        /// Get the Atdl that corresponds to this algo - older algos may not support this.
        /// </summary>
        /// <returns></returns>
        string GetAtdl();

        /// <summary>
        /// WILL BE DEPRICATED - pared for paramters
        /// </summary>
        string Expression
        {
            get;
            set;
        }

        /// <summary>
        /// Return the XML rendering of the expression
        /// </summary>
        /// <returns></returns>
        KAI.kaitns.StatExpression ToXMLDB();

        /// <summary>
        /// Render the expression in XML
        /// </summary>
        /// <param name="myMatcher"></param>
        void FromXMLDB(KAI.kaitns.StatExpression myExpression );
    }
}
