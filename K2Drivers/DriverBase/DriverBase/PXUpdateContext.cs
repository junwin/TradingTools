using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriverBase
{
    /// <summary>
    /// This class may be used by drivers to help determine what has changed
    /// in some given price update
    /// </summary>
    public class PXUpdateContext
    {
        private string m_Mnemonic;

        public string Mnemonic
        {
            get { return m_Mnemonic; }
            set { m_Mnemonic = value; }
        }

        private decimal? m_BidPrice=decimal.MaxValue;

        public decimal? BidPrice
        {
            get { return m_BidPrice; }
            set { m_BidPrice = value; }
        }

        private decimal? m_BidSize = decimal.MaxValue;

        public decimal? BidSize
        {
            get { return m_BidSize; }
            set { m_BidSize = value; }
        }

        private long? m_BidTime=0;

        public long? BidTime
        {
            get { return m_BidTime; }
            set { m_BidTime = value; }
        }

        private decimal? m_OfferPrice=decimal.MaxValue;

        public decimal? OfferPrice
        {
            get { return m_OfferPrice; }
            set { m_OfferPrice = value; }
        }

        private decimal? m_OfferSize = decimal.MaxValue;

        public decimal? OfferSize
        {
            get { return m_OfferSize; }
            set { m_OfferSize = value; }
        }
        private long? m_OfferTime=0;

        public long? OfferTime
        {
            get { return m_OfferTime; }
            set { m_OfferTime = value; }
        }

        private decimal? m_TradePrice = decimal.MaxValue;

        public decimal? TradePrice
        {
            get { return m_TradePrice; }
            set { m_TradePrice = value; }
        }
        private decimal? m_TradeSize = decimal.MaxValue;

        public decimal? TradeVolume
        {
            get { return m_TradeSize; }
            set { m_TradeSize = value; }
        }
        private long? m_TradeTime=0;

        public long? TradeTime
        {
            get { return m_TradeTime; }
            set { m_TradeTime = value; }
        }

        public PXUpdateContext(string mnemonic)
        {
            this.Mnemonic = mnemonic;
        }
        public bool IsUpdatedTrade(KaiTrade.Interfaces.IPXUpdate update)
        {
            bool IsUpdate = false;
            try
            {
                if (update.TradePrice.HasValue)
                {
                    if ((update.TradePrice != TradePrice) || (update.TradeVolume != TradeVolume) || (update.ServerTicks != TradeTime))
                    {
                        IsUpdate = true;
                        TradePrice = update.TradePrice;
                        TradeVolume = update.TradeVolume;
                        TradeTime = update.ServerTicks;
                    }
                }
            }
            catch
            {
            }
            return IsUpdate;
        }

        public bool IsUpdatedBid(KaiTrade.Interfaces.IPXUpdate update)
        {
            bool IsUpdate = false;
            try
            {
                if (update.BidPrice.HasValue)
                {
                    if ((update.BidPrice != BidPrice) || (update.BidSize != BidSize) || (update.ServerTicks != BidTime))
                    {
                        IsUpdate = true;
                        BidPrice = update.BidPrice;
                        BidSize = update.BidSize;
                        BidTime = update.ServerTicks;
                    }
                }
            }
            catch
            {
            }
            return IsUpdate;
        }

        public bool IsUpdatedOffer(KaiTrade.Interfaces.IPXUpdate update)
        {
            bool IsUpdate = false;
            try
            {
                if (update.OfferPrice.HasValue)
                {
                    if ((update.OfferPrice != OfferPrice) || (update.OfferSize != OfferSize) || (update.ServerTicks != OfferTime))
                    {
                        IsUpdate = true;
                        OfferPrice = update.OfferPrice;
                        OfferSize = update.OfferSize;
                        OfferTime = update.ServerTicks;
                    }
                }
            }
            catch
            {
            }
            return IsUpdate;
        }
    }
}
