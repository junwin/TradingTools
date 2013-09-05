/***************************************************************************
 *
 *      Copyright (c) 2009,2010,2011,2012 KaiTrade LLC (registered in Delaware)
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
using System.IO;

namespace KTManagers
{
    public class OrderManager : PublisherManager, KaiTrade.Interfaces.OrderManager
    {
        /// <summary>
        /// Singleton OrderManager
        /// </summary>
        private static volatile OrderManager s_instance;

        private static object s_Token = new object();

        /// <summary>
        /// Collection of orders indexed by the order identity
        /// </summary>
        private System.Collections.Generic.Dictionary<string, KaiTrade.Interfaces.Order> m_Orders;

        /// <summary>
        /// Collection of orders in dexed by ClOrdID
        /// </summary>
        private System.Collections.Generic.Dictionary<string, KaiTrade.Interfaces.Order> m_ClOrdIDMap;

        /// <summary>
        /// Collection of execution reps in dexed by exec id
        /// </summary>
        private System.Collections.Generic.Dictionary<string, QuickFix.Message> m_ExecRepMap;

        public static OrderManager Instance()
        {
            // Uses "Lazy initialization" and double-checked locking
            if (s_instance == null)
            {
                lock (s_Token)
                {
                    if (s_instance == null)
                    {
                        s_instance = new OrderManager();
                    }
                }
            }
            return s_instance;
        }

        protected OrderManager()
        {
            //m_Subscribers = new List<KaiTrade.Interfaces.Subscriber>();
            m_Orders = new Dictionary<string, KaiTrade.Interfaces.Order>();
            m_ClOrdIDMap = new Dictionary<string, KaiTrade.Interfaces.Order>();
            m_ExecRepMap = new Dictionary<string, QuickFix.Message>();
            m_Name = "KTAOrderManager";
        }

        #region OrderManager Members

        /// <summary>
        /// Create an order and regsiter it in the manager
        /// </summary>
        /// <returns></returns>
        public KaiTrade.Interfaces.Order CreateOrder()
        {
            KaiTrade.Interfaces.Order myOrder = new KaiTrade.TradeObjects.Order();
            m_Orders.Add(myOrder.Identity, myOrder);
            DoUpdate("CREATE", myOrder.Identity);
            return myOrder;
        }

        

        /// <summary>
        /// Register an order with the manager - used when the order is created elsewhere
        /// e.g. when loaded from a file. You shoul normally use CreateOrder to get an Order
        /// </summary>
        /// <param name="?"></param>
        public void RegisterOrder(KaiTrade.Interfaces.Order myOrder)
        {
            try
            {
                m_Orders.Add(myOrder.Identity, myOrder);
                DoUpdate("REGISTER", myOrder.Identity);
            }
            catch (Exception myE)
            {
                m_Log.Error("RegisterOrder", myE);
            }
        }

        /// <summary>
        /// Invert the side passed in
        /// </summary>
        /// <param name="mySide"></param>
        /// <returns></returns>
        public string InvertSide(string mySide)
        {
            string myRet = "";
            switch (mySide.ToUpper())
            {
                case KaiTrade.Interfaces.Side.BUY:
                    myRet = KaiTrade.Interfaces.Side.SELL;
                    break;
                case KaiTrade.Interfaces.Side.SELL:
                    myRet = KaiTrade.Interfaces.Side.BUY;
                    break;
                default:
                    myRet = mySide;
                    break;
            }
            return myRet;
        }

        /// <summary>
        /// Associate a ClOrdID with a particular order
        /// </summary>
        /// <param name="myClOrdID"></param>
        /// <param name="myOrder"></param>
        public void AssociateClOrdID(string myClOrdID, KaiTrade.Interfaces.Order myOrder)
        {
            if (m_ClOrdIDMap.ContainsKey(myClOrdID))
            {
                m_ClOrdIDMap[myClOrdID] = myOrder;
            }
            else
            {
                m_ClOrdIDMap.Add(myClOrdID, myOrder);
            }
            // ensure the order is accessable by its ID
            if (!m_Orders.ContainsKey(myOrder.Identity))
            {
                m_Orders.Add(myOrder.Identity, myOrder);
            }
        }

        /// <summary>
        /// Delete an order from the manager
        /// </summary>
        /// <param name="myID"></param>
        public void DeleteOrder(string myID)
        {
            if(m_Orders.ContainsKey(myID))
            {
                KaiTrade.Interfaces.Order myOrder = m_Orders[myID];
                if(m_ClOrdIDMap.ContainsKey(myOrder.ClOrdID.getValue()))
                {
                    m_ClOrdIDMap.Remove(myOrder.ClOrdID.getValue());
                }
                m_Orders.Remove(myID);
                DoUpdate("DELETE", myID);
            }
        }

        /// <summary>
        /// Get an order using its ID
        /// </summary>
        /// <param name="myOrderID"></param>
        /// <returns></returns>
        public KaiTrade.Interfaces.Order GetOrder(string myOrderID)
        {
            KaiTrade.Interfaces.Order myOrder = null;
            if (m_Orders.ContainsKey(myOrderID))
            {
                myOrder = m_Orders[myOrderID];
            }
            return myOrder;
        }

        /// <summary>
        /// Get a list of all OrderIDs in the manager
        /// </summary>
        /// <returns></returns>
        public List<string> GetOrderIDS()
        {
            List<string> myList = new List<string>();
            foreach (string myID in m_Orders.Keys)
            {
                myList.Add(myID);
            }

            return myList;
        }

        /// <summary>
        /// Get a list of all OrderIDs in the manager
        /// </summary>
        /// <returns></returns>
        public List<string> GetOrderIDS(string user)
        {
            List<string> myList = new List<string>();
            foreach (KaiTrade.Interfaces.Order order in m_Orders.Values)
            {
                if (order.User == user)
                {
                    myList.Add(order.Identity);
                }
            }

            return myList;
        }

        /// <summary>
        /// Get an order using a CLOrdID
        /// </summary>
        /// <param name="myID"></param>
        /// <returns></returns>
        public KaiTrade.Interfaces.Order GetOrderWithClOrdIDID(string myID)
        {
            KaiTrade.Interfaces.Order myOrder = null;
            if (m_ClOrdIDMap.ContainsKey(myID))
            {
                myOrder = m_ClOrdIDMap[myID];
            }
            return myOrder;
        }

        /// <summary>
        /// Load a set of orders from a files of XML databindings and
        /// add them to the manager
        /// </summary>
        /// <param name="myFilePath"></param>
        public void LoadOrdersFromFile(string myFilePath)
        {
            try
            {
                KAI.kaitns.OrderSet mySet = new KAI.kaitns.OrderSet();
                mySet.FromXmlFile(myFilePath);
                KaiTrade.TradeObjects.Order myOrder;

                foreach (KAI.kaitns.Order myOrderDB in mySet.Order)
                {
                    myOrder = new KaiTrade.TradeObjects.Order();
                    myOrder.FromXMLDB(myOrderDB);
                    this.RegisterOrder(myOrder);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("LoadOrdersFromFile" , myE);
            }
        }

        /// <summary>
        /// Store orders using XML databindings to a file
        /// </summary>
        /// <param name="myFilePath"></param>
        public void StoreOrdersToFile(string myFilePath)
        {
            try
            {
                KAI.kaitns.OrderSet mySet = new KAI.kaitns.OrderSet();
                mySet.Name = "OrderManager";
                KAI.kaitns.Order myOrderDB;

                foreach (KaiTrade.Interfaces.Order myOrder in m_Orders.Values)
                {
                    myOrderDB = myOrder.ToXMLDB();
                    mySet.Order.Add(myOrderDB);
                }

                mySet.ToXmlFile(myFilePath);
            }
            catch (Exception myE)
            {
                m_Log.Error("StoreOrdersToFile", myE);
            }
        }


        public void ToFileJSON(string myOrderFilePath, string myFillsFilePath)
        {
            FileStream file;
            StreamWriter writer;

            FileStream fillsFile;
            StreamWriter fillsWriter;

            try
            {


                file = new FileStream(myOrderFilePath, FileMode.Create, FileAccess.Write);
                writer = new StreamWriter(file);

                fillsFile = new FileStream(myFillsFilePath, FileMode.Create, FileAccess.Write);
                fillsWriter = new StreamWriter(fillsFile);

                foreach (KaiTrade.Interfaces.Order order in m_Orders.Values)
                {

                    K2DataObjects.OrderData orderData = new K2DataObjects.OrderData();
                    orderData.From(order);
                    string orderJSON = Newtonsoft.Json.JsonConvert.SerializeObject(orderData);
                    writer.WriteLine(orderJSON);
                    writer.Flush();

                    try
                    {
                        foreach (KaiTrade.Interfaces.Fill fill in order.FillsList)
                        {
                            K2DataObjects.FillData fillData = fill as K2DataObjects.FillData;
                            if (fill != null)
                            {
                                string fillJSON = Newtonsoft.Json.JsonConvert.SerializeObject(fillData);
                                fillsWriter.WriteLine(fillJSON);
                                fillsWriter.Flush();
                            }

                        }
                    }
                    catch (Exception myE)
                    {
                        m_Log.Error("fill2File error", myE);

                    }

                }
                writer.Close();
                file.Close();
                fillsWriter.Close();
                fillsFile.Close();



            }
            catch (Exception myE)
            {
                m_Log.Error("ProductManager.ToFile:JSON", myE);
            }
            finally
            {
                writer = null;
                file = null;
                fillsWriter = null;
                fillsFile = null;
            }

        }

        /// <summary>
        /// Return an order as tab separated
        /// </summary>
        /// <returns></returns>
        public string ToTabSeparated()
        {
            string myOrders = "";
            try
            {
                foreach (KaiTrade.Interfaces.Order myOrder in m_Orders.Values)
                {
                    myOrders += myOrder.ToTabSeparated();
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("ToTabSeparated", myE);
            }
            return myOrders;
        }

        /// <summary>
        /// indicate that the order has changed
        /// </summary>
        /// <param name="myID"></param>
        public void SetChanged(string myID)
        {
            DoUpdate("UPDATE", myID);
        }

        /// <summary>
        /// Record an incomming execution report - will return false
        /// if the report has been processed already
        /// </summary>
        /// <param name="myID"></param>
        /// <param name="myExecRep"></param>
        /// <returns>false if report exists</returns>
        public bool RecordExecutionReport(string myID, QuickFix.Message myExecRep)
        {
            bool bRet = false;
            if (m_ExecRepMap.ContainsKey(myID))
            {
                // no action
            }
            else
            {
                m_ExecRepMap.Add(myID, myExecRep);
                bRet = true;
            }
            return bRet;
        }

        #endregion
    }
}
