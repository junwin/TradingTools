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
    public class TSQueryGroup : KaiTrade.Interfaces.ITSQueryGroup
    {
        private string m_Mnemonic = "";
        private string m_Name="";
        private string m_StrategyName = "";

        /// <summary>
        /// the list of TSData sets in this group
        /// </summary>
        List<KaiTrade.Interfaces.ITSSet> m_Items;

        // Create a logger for use in this class
        public log4net.ILog m_Log;

        public TSQueryGroup()
        {
            m_Log = log4net.LogManager.GetLogger("Kaitrade");
            m_Items = new List<KaiTrade.Interfaces.ITSSet>();
        }

        /// <summary>
        /// ID of the strategy that this set is related to (if any)
        /// </summary>
        public string StrategyName
        {
            get { return m_StrategyName; }
            set
            {
                m_StrategyName = value;
                setStrategyName(m_StrategyName);
            }
        }

        public override string ToString()
        {
            string myRet = "";
            myRet += this.Name + ":";
            myRet += this.Mnemonic;

            return myRet;

            //return string.Format("Customer = {0} ID = {1}", Customer, CustomerID);
        }

        #region TSQueryGroup Members

        public void FromXml(string myXML)
        {
            try
            {
                
            }
            catch (Exception myE)
            {
                m_Log.Error("FromXml", myE);
            }
        }

        
        public string GetXML()
        {
            string myXML = "";
            try
            {
                //myXML = GetXMLDB().ToXml();
            }
            catch (Exception myE)
            {
                m_Log.Error("FromXml", myE);
            }

            return myXML;
        }

        /// <summary>
        /// returns true if statusA is more severe than B
        /// </summary>
        /// <param name="myStatusA"></param>
        /// <param name="myStatusB"></param>
        /// <returns></returns>
        private bool isMoreSevere(KaiTrade.Interfaces.Status myStatusA, KaiTrade.Interfaces.Status myStatusB)
        {
            bool myRet = false;
            switch (myStatusB)
            {
                case KaiTrade.Interfaces.Status.closed:
                case KaiTrade.Interfaces.Status.closing:
                    if (myStatusA == KaiTrade.Interfaces.Status.error)
                    {
                        myRet = true;
                    }
                    else
                    {
                        myRet = false;
                    }
                    break;

                case KaiTrade.Interfaces.Status.error:
                    myRet = false;
                    break;

                case KaiTrade.Interfaces.Status.loaded:
                    if ((myStatusA != KaiTrade.Interfaces.Status.open) && (myStatusA != KaiTrade.Interfaces.Status.opening) && (myStatusA != KaiTrade.Interfaces.Status.loaded))
                    {
                        myRet = true;
                    }
                    else
                    {
                        myRet = false;
                    }
                    break;
                case KaiTrade.Interfaces.Status.open:
                case KaiTrade.Interfaces.Status.opening:
                    if (myStatusA != KaiTrade.Interfaces.Status.open)
                    {
                        myRet = true;
                    }
                    else
                    {
                        myRet = false;
                    }
                    break;
                default:
                    break;
            }
            return myRet;
        }

        /// <summary>
        /// Get the status of a group of set - this will show
        /// the most severe error state of any of the groups sets
        /// open implies all are open
        /// </summary>
        public KaiTrade.Interfaces.Status Status
        {
            get
            {
                KaiTrade.Interfaces.Status myStatus = KaiTrade.Interfaces.Status.open;
                foreach (KaiTrade.Interfaces.ITSSet myTSSet in m_Items)
                {
                    if (isMoreSevere(myTSSet.Status, myStatus))
                    {
                        myStatus = myTSSet.Status;
                    }
                }
                return myStatus;
            }
        }

        public string Mnemonic
        {
            get
            {
                return m_Mnemonic;
            }
            set
            {
                m_Mnemonic = value;
                setMnemonic(m_Mnemonic);
            }
        }

        /// <summary>
        /// This will override the intraday interval used in individual queries
        /// </summary>
        /// <param name="myInterval"></param>
        public void SetInterval(int myInterval)
        {
            try
            {
                foreach (KaiTrade.Interfaces.ITSSet mySet in m_Items)
                {
                    if (mySet != null)
                    {
                        mySet.IntraDayInterval = myInterval;
                    }
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("setMnemonic", myE);
            }
        }

        /// <summary>
        /// set the mnemonic in any sets
        /// </summary>
        /// <param name="myMnemonic"></param>
        private void setMnemonic(string myMnemonic)
        {
            try
            {
                foreach (KaiTrade.Interfaces.ITSSet mySet in m_Items)
                {
                    if (mySet != null)
                    {
                        mySet.Mnemonic = myMnemonic;
                    }
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("setMnemonic", myE);
            }
        }

        /// <summary>
        /// set the strategy ID in any sets
        /// </summary>
        /// <param name="myMnemonic"></param>
        private void setStrategyName(string myStrategyID)
        {
            try
            {
                foreach (KaiTrade.Interfaces.ITSSet mySet in m_Items)
                {
                    if (mySet != null)
                    {
                        mySet.StrategyName = myStrategyID;
                    }
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("setStrategyName", myE);
            }
        }

        
        

        /// <summary>
        /// Add a subscriber to the query group - any sets that have a publisher interface
        /// will be subscribed to
        /// </summary>
        /// <param name="mySubscriber"></param>
        public void addSubscriber(KaiTrade.Interfaces.ISubscriber mySubscriber)
        {
            try
            {
                foreach (KaiTrade.Interfaces.IPublisher myPub in m_Items)
                {
                    if (myPub != null)
                    {
                        myPub.Subscribe(mySubscriber);
                    }
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("addSubscriber", myE);
            }
        }

        /// <summary>
        /// Unsubscribe to the  to the query group - any sets that have a publisher interface
        /// will be unsubscribed
        /// </summary>
        /// <param name="mySubscriber"></param>
        public void UnSubscribe(KaiTrade.Interfaces.ISubscriber mySubscriber)
        {
            try
            {
                foreach (KaiTrade.Interfaces.IPublisher myPub in m_Items)
                {
                    if (myPub != null)
                    {
                        myPub.UnSubscribe(mySubscriber);
                    }
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("UnSubscribe", myE);
            }
        }

        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }

        public List<KaiTrade.Interfaces.ITSSet> Items
        {
            get
            {
                return m_Items;
            }
            set
            {
                m_Items = value;
            }
        }

        #endregion
    }
}
