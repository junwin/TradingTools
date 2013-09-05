﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace KTASimulator
{
    public partial class SimulatorMainUI : Form
    {
        private KTASimulator m_Simulator;

        public SimulatorMainUI()
        {
            InitializeComponent();
        }

        public KTASimulator Simulator
        {
            get { return m_Simulator; }
            set { m_Simulator = value; }
        }

        private void loadPriceFile(string filename)
        {
            try
            {
            }
            catch (Exception myE)
            {
            }
        }


        private KaiTrade.Interfaces.IProduct addProduct(string cfi, string exchnage, string mnemonic, string symbol, string mmy, string currency, double? tickSize)
        {
            KaiTrade.Interfaces.IProduct prod =null;
            try
            {
                prod = m_Simulator.AddProductDirect(mnemonic, cfi, exchnage, "", "", symbol, mmy, "", currency, null, true);
                if(tickSize.HasValue)
                {
                    prod.TickSize = (decimal)tickSize.Value;
                }
                m_Simulator.CreateMarket(mnemonic);
                
            }
            catch (Exception myE)
            {
            }
            return prod;
        }

        private void addProductPriceFile(string filepath, KaiTrade.Interfaces.IProduct prod, int interval, bool runRealTime, bool repeatOnEnd, bool playOnSubscribe)
        {
            try
            {
                m_Simulator.AddPriceFile(prod.Mnemonic, filepath, interval, runRealTime, repeatOnEnd, playOnSubscribe);
            }
            catch (Exception myE)
            {
            }

        }

        private void btnFindFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtFilePath.Text = openFileDialog1.FileName;
            }
        }

        private void btnOpenPriceFile_Click(object sender, EventArgs e)
        {
            try
            {
                m_Simulator.AddPriceFile("XDELL", txtFilePath.Text, 5, true, true, false);
            }
            catch (Exception myE)
            {
            }
        }

        private void btnLoadFiles_Click(object sender, EventArgs e)
        {
            m_Simulator.GetPriceFiles();
        }

        private void btnRealTime_Click(object sender, EventArgs e)
        {
            try
            {
                m_Simulator.RunPxRealTime();
            }
            catch (Exception myE)
            {
            }
        }

        private void btnIntervalRun_Click(object sender, EventArgs e)
        {
            try
            {
                int interval = 10;
                try
                {
                    interval = int.Parse(txtInterval.Text);
                    if(interval <=0)
                    {
                        interval = 10;
                    }
                }
                catch(Exception mye)
                {
                }
                m_Simulator.RunPxInterval(interval);
                 
            }
            catch (Exception myE)
            {
            }
        }

        private void btnPxUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                publishPrices(txtMnemonic.Text, txtOfferSz.Text, txtOfferPx.Text, txtBidPx.Text, txtBidSz.Text, txtTradePx.Text, txtTradeSz.Text);

            }
            catch (Exception myE)
            {
            }
        }

        private void publishPrices(string myMnemonic, string myOfferSz, string myOfferPx, string myBidPx, string myBidSz, string myTradePx, string myTradeSz)
        {
            try
            {
                //KaiTrade.Interfaces.Facade myFacade = m_Simulator.Facade;

                // try get the product and create one if needed - this wont work for all Venues
                //myFacade.CheckCreateMnemonic(myMnemonic);

                // get a PX publisher for the mnemonic
                
                L1PriceSupport.PXPublisher myPub = m_Simulator.Facade.Factory.GetPXPublisher(myMnemonic) as L1PriceSupport.PXPublisher;
                if (myPub != null)
                {
                    myPub.SetField(KaiTrade.Interfaces.MDConstants.BID1_PRICE, myBidPx);
                    myPub.SetField(KaiTrade.Interfaces.MDConstants.BID1_SIZE, myBidSz);
                    myPub.SetField(KaiTrade.Interfaces.MDConstants.OFFER1_PRICE, myOfferPx);
                    myPub.SetField(KaiTrade.Interfaces.MDConstants.OFFER1_SIZE, myOfferSz);
                    myPub.SetField(KaiTrade.Interfaces.MDConstants.TRADE_PRICE, myTradePx);
                    myPub.SetField(KaiTrade.Interfaces.MDConstants.TRADE_SIZE, myTradeSz);
                    (myPub as KaiTrade.Interfaces.IPublisher).OnUpdate(null);
                }

            }
            catch (Exception myE)
            {
                 
            }
        }


    }
}