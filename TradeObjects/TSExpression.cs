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

namespace K2DataObjects
{
    public class TSExpression : KaiTrade.Interfaces.ITSExpression
    {
        private string m_Expression = "";
        private string m_Alias = "";
        private KaiTrade.Interfaces.TSFormat m_Format = KaiTrade.Interfaces.TSFormat.DriverSpecific;
        //private KaiTrade.Interfaces.StatExpression m_StatExpression = null;

        #region TSExpression Members

        /// <summary>
        /// Get/Set the format of this expression
        /// </summary>
        public KaiTrade.Interfaces.TSFormat Format
        {
            get { return m_Format; }
            set { m_Format = value; }
        }

        public string Alias
        {
            get
            {
                return m_Alias;
            }
            set
            {
                m_Alias = value;
            }
        }

        public string Expression
        {
            get
            {
                return m_Expression;
            }
            set
            {
                m_Expression = value;
            }
        }

        /// <summary>
        /// Name of the Base expression/alg/calc to be used
        /// </summary>
        public string BaseExpression
        {
            get
            {
                string[] myParm = m_Expression.Split(',');
                return myParm[0];
            }
        }


        #endregion
    }
}
