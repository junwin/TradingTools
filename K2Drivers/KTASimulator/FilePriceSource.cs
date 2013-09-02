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
using System.IO;
using System.Threading;
using System.Timers;

namespace KTASimulator
{
    public enum SrcStatus { open, ended, failed, none };
    public delegate void PriceUpdate(KaiTrade.Interfaces.IPXUpdate pxUpdate);
    public delegate void PriceUpdateStatus(FilePriceSource source, SrcStatus status);

     

    /// <summary>
    /// This class will open a file, read prices and publish them in the simulator
    /// </summary>
    public class FilePriceSource
    {

        private PriceUpdate m_PriceUpdate;
        private PriceUpdateStatus m_PriceUpdateStatus;

        private string m_FilePath;
        private FileStream m_File;
        private StreamReader m_Sr;

        

        private Thread m_DataPlayback;
        private bool m_RunPlayBack = false;

        
        
        private long m_ReceivedUpdates = 0;
        private int m_Interval = 0;
        public int m_DepthLevels = 0;
        private bool m_RepeatOnEnd = false;
        private bool m_RunRealTime = true;
        private bool m_PlayOnSubscribe = true;

        private KTASimulator m_Simulator = null;

        public log4net.ILog m_Log = log4net.LogManager.GetLogger("PlugIn");


        public FilePriceSource(KTASimulator simulator)
        {
            m_Simulator = simulator;
        }

        public void Start(string filePath)
        {
            try
            {
                this.FilePath = filePath;
                // attempt to stop
                stopDataReplay();

                // start
                startDataReplay();
            }
            catch(Exception myE)
            {
                m_Log.Error("Start", myE);
            }
        }

        public void Stop()
        {
            try
            {
                stopDataReplay();
            }
            catch (Exception myE)
            {
                m_Log.Error("Stop", myE);
            }
        }

        /// <summary>
        /// Get/Set the file path
        /// </summary>
        public string FilePath
        {
            get { return m_FilePath; }
            set { m_FilePath = value; }
        }

        public int DepthLevels
        {
            get { return m_DepthLevels; }
            set { m_DepthLevels = value; }
        }

        public int  Interval
        {
            get { return m_Interval; }
            set { m_Interval = value; }
        }

        public bool RepeatOnEnd
        {
            get { return m_RepeatOnEnd; }
            set { m_RepeatOnEnd = value; }
        }

        public bool PlayOnSubscribe
        {
            get { return m_PlayOnSubscribe; }
            set { m_PlayOnSubscribe = value; }
        }

        public bool RunRealTime
        {
            get { return m_RunRealTime; }
            set { m_RunRealTime = value; }
        }

        /// <summary>
        /// update any clients of the alg that changes have occured
        /// </summary>
        private void updateClients(KaiTrade.Interfaces.IPXUpdate pxUpdate)
        {
            try
            {
                if (m_PriceUpdate != null)
                {
                    m_PriceUpdate(pxUpdate);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("updateClients", myE);
            }
        }

        public PriceUpdate PriceUpdate
        {
            get { return m_PriceUpdate; }
            set { m_PriceUpdate = value; }
        }
        private void updateClients(SrcStatus status)
        {
            try
            {
                if (m_PriceUpdateStatus != null)
                {
                    m_PriceUpdateStatus(this, status);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("updateClients:status", myE);
            }
        }

        public PriceUpdateStatus PriceUpdateStatus
        {
            get { return m_PriceUpdateStatus; }
            set { m_PriceUpdateStatus = value; }
        }

        private void stopDataReplay()
        {
            try
            {
                m_RunPlayBack = false;
                if (m_DataPlayback != null)
                {
                    try
                    {
                        if (Thread.CurrentThread != m_DataPlayback)
                        {
                            m_DataPlayback.Abort();
                        }
                    }
                    catch (Exception myE)
                    {
                    }
                }
                if (m_Sr != null)
                {
                    m_Sr.Close();
                    m_Sr = null;
                }
                if (m_File != null)
                {
                    m_File.Close();
                    m_File = null;
                }

               
            }
            catch (Exception myE)
            {
                m_Log.Error("stopDataDump", myE);
            }
        }

        private void startDataReplay()
        {
            try
            {
                m_File = new FileStream(m_FilePath, FileMode.Open, FileAccess.Read);
                 
                m_Sr = new StreamReader(m_File);
                m_RunPlayBack = true;

                m_DataPlayback = new Thread(new ThreadStart(this.runPlayBack));
                m_DataPlayback.SetApartmentState(ApartmentState.MTA);
                m_DataPlayback.Start();
            }
            catch (Exception myE)
            {
                m_Log.Error("startDataReplay", myE);
            }
        }


        KaiTrade.Interfaces.IProduct GetProductFromXML(string xmlData)
        {
            K2DataObjects.Product product = null;
            try
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(xmlData);

                product = new K2DataObjects.Product();

                foreach (System.Xml.XmlAttribute attribute in doc.Attributes)
                {
                    SetProductValue(product, attribute);

                }
                
            }
            catch (Exception myE)
            {
                m_Log.Error("runPlayBack", myE);
            }
            return product;
        }

        private void SetProductValue(KaiTrade.Interfaces.IProduct product, System.Xml.XmlAttribute attribute)
        {
            switch (attribute.Name)
            {
                case "Src":
                    product.SecurityID = attribute.Value;
                    break;
                case "IDSrc":
                    product.IDSource = attribute.Value;
                    break;
                case "Exch":
                    product.Exchange = attribute.Value;
                    break;
                case "CFI":
                    product.CFICode = attribute.Value;
                    break;
                case "TradeVenue":
                    product.TradeVenue = attribute.Value;
                    break;
                case "LongName":
                    product.LongName = attribute.Value;
                    break;
                case "Mnemonic":
                    product.Mnemonic = attribute.Value;
                    break;
                case "ExDestination":
                    product.ExDestination = attribute.Value;
                    break;
                case "Currency":
                    product.Currency = attribute.Value;
                    break;
                case "TickSize":
                    product.TickSize = decimal.Parse(attribute.Value.ToString());
                    break;
                case "ContractSize":
                    product.ContractSize = decimal.Parse(attribute.Value.ToString());
                    break;
                case "NumberDecimalPlaces":
                    product.NumberDecimalPlaces = int.Parse(attribute.Value.ToString());
                    break;
                case "Sym":
                    product.Symbol = attribute.Value;
                    break;
                case "PriceFeedQuantityMultiplier":
                    product.PriceFeedQuantityMultiplier = int.Parse(attribute.Value.ToString());
                    break;
                case "TickValue":
                    product.TickValue = decimal.Parse(attribute.Value.ToString());
                    break;
                case "Commodity":
                    product.Commodity = attribute.Value;
                    break;
                case "GenericName":
                    product.GenericName = attribute.Value;
                    break;
                default:
                    break;

            }
        }

        /*
        private void setupProduct(string line)
        {
            try
            {
                string xmlData = line.Substring(5);

                KAI.kaitns.Product prodDB = new KAI.kaitns.Product();
                prodDB.FromXml(xmlData);

                prodDB.TradeVenue = "KTSIM";
                prodDB.Mnemonic = "S." + prodDB.Mnemonic;
                if (prodDB.IsValidSym)
                {
                    prodDB.Sym = "S." + prodDB.Sym;
                }
                if (prodDB.IsValidSrc)
                {
                    prodDB.Src = "S." + prodDB.Src;
                }

                KaiTrade.Interfaces.IProduct prod = m_Simulator.Facade.Factory.GetProductManager().GetProductMnemonic(prodDB.Mnemonic);
                if (prod == null)
                {
                    prod = m_Simulator.Facade.Factory.GetProductManager().CreateProduct(prodDB.Mnemonic);
                    prod.From(prodDB, false);
                    m_Simulator.CreateMarket(prodDB.Mnemonic);
                }
                
            }
            catch (Exception myE)
            {
                m_Log.Error("runPlayBack", myE);
            }
        }
         */

        public void SetUpProductNoRun(string filePath)
        {
            try
            {
                m_File = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                m_Sr = new StreamReader(m_File);

                string myLine = m_Sr.ReadLine();
                if (myLine[0] == '#')
                {
                    m_Log.Info(myLine);
                    if (myLine.IndexOf("#XML") >= 0)
                    {
                        //setupProduct(myLine);
                    }
                }

                m_Sr.Close();
                m_Sr = null;
                m_File.Close();
                m_File = null;

            }
            catch
            {
            }
        }

        private void runPlayBack()
        {
            try
            {
                string myLine = "";
                m_RunPlayBack = true;
                m_ReceivedUpdates = 0;
                K2DataObjects.PXUpdateBase pxUpdate;
                long startTicks = 0;
                int sleepTime = 50;
                while ((!m_Sr.EndOfStream) && (m_RunPlayBack))
                {
                    myLine = m_Sr.ReadLine();
                    if (myLine[0] == '#')
                    {
                        m_Log.Info(myLine);
                        if (myLine.IndexOf("#XML") >= 0)
                        {
                            //setupProduct(myLine);
                        }
                    }
                    else
                    {
                        pxUpdate = new K2DataObjects.PXUpdateBase("FILE");
                        pxUpdate.From(myLine, ',');

                        pxUpdate.Mnemonic = "S." + pxUpdate.Mnemonic;
                        if (startTicks > 0)
                        {
                            sleepTime = (int)(pxUpdate.Ticks - startTicks)/10000;
                            startTicks = pxUpdate.Ticks;
                             
                        }
                        else
                        {
                            sleepTime = this.Interval;
                            startTicks = pxUpdate.Ticks;
                        }

                        if (this.m_RunRealTime)
                        {
                            if (sleepTime < 0)
                            {
                                sleepTime = 1;
                            }
                            Thread.Sleep(sleepTime);
                        }
                        else
                        {
                            Thread.Sleep(this.Interval);
                        }
                        this.updateClients(pxUpdate);

                        m_ReceivedUpdates++;
                    }
                    
                }
                if (m_Sr.EndOfStream)
                {
                    updateClients(SrcStatus.ended);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("runPlayBack", myE);
            }
        }
    }
}
