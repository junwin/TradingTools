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

namespace DriverBase
{
    /// <summary>
    /// Generates the various ID needed by the system
    /// </summary>
    public sealed class Identities
    {
        private static readonly Identities instance = new Identities();

        private Int32 m_orderID=0;
       // private Int32 m_execID=0;
        //private Int32 m_ReqID=0;
        private long m_NextLong;
        private string m_PrePend = "";

        private Identities()
        {
            // note that this class is a singleton
            m_orderID = 0;
            //m_execID = 0;
            //m_ReqID = 0;
            m_NextLong = (System.DateTime.Now.Ticks % 10000000);
            m_PrePend = "K";
            m_PrePend += m_NextLong.ToString("X");
            //m_PrePend += "K";
        }

        // public property that can only get the single instance of this class.
        public static Identities Instance
        {
            get
            {
                return instance;
            }
        }

        public long GetNextNumber()
        {
            return m_NextLong++;
        }

        public string getNextOrderID()
        {
            string strRet = m_PrePend + (++m_orderID).ToString("X");
            return strRet;
        }
        public QuickFix.OrderID genOrderID()
        {
            return new QuickFix.OrderID(getNextOrderID());
        }

        public string getNextExecID()
        {
            string strRet = m_PrePend + (++m_orderID).ToString("X");
            return strRet;
        }
        public QuickFix.ExecID genExecID()
        {
            return new QuickFix.ExecID(getNextExecID());
        }

        public string genReqID()
        {
            string strRet = m_PrePend + (++m_orderID).ToString("X");
            return strRet;
        }
    }
}
