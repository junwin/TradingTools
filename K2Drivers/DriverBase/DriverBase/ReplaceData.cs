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
using System.Threading;
using System.Collections;
using System.Linq;

namespace DriverBase
{
    /// <summary>
    /// Type a cancel or replace  - note that a cancel will
    /// override the previous replaces
    /// </summary>
    public enum crrType { 
        /// <summary>
        /// Replace request
        /// </summary>
        replace, 
        /// <summary>
        /// Cancel request
        /// </summary>
        cancel }

    public class RequestData
    {
        private string _mnemonic = "";
        private crrType _cRRType;
        private int _retryCount;
        private string _clOrdID = "";
        private string _origClOrdID = "";

        public string OrigClOrdID
        {
            get { return _origClOrdID; }
            set { _origClOrdID = value; }
        }

        public string ClOrdID
        {
            get { return _clOrdID; }
            set { _clOrdID = value; }
        }
        

        public virtual int RetryCount
        {
            get { return _retryCount; }
            set { _retryCount = value; }
        }

        public crrType CRRType
        {
            get { return _cRRType; }
            set { _cRRType = value; }
        }

        public string Mnemonic
        {
            get { return _mnemonic; }
            set { _mnemonic = value; }
        }

    }
    public class CancelRequestData : RequestData
    {
        private KaiTrade.Interfaces.ICancelOrderRequest _cancelRequest = null;
        protected long _lastChangeTicks;
        protected OrderContext _orderContext;
        protected crrType _cRRType;

        public CancelRequestData(crrType type, KaiTrade.Interfaces.ICancelOrderRequest cancelRequest)
        {
            _cancelRequest = cancelRequest;
            if(_cancelRequest == null)
            {
                Exception myE = new Exception("a non null cancel request must be supplied");
                throw myE;
            }

            Mnemonic = cancelRequest.Mnemonic;
            

            _lastChangeTicks = DateTime.Now.Ticks;


            if(_cancelRequest.ClOrdID.Length == 0 )
            {
                Exception myE = new Exception("a clordid must be specified on a cancelOrder");
                throw myE;
            }
            if (_cancelRequest.OrigClOrdID.Length == 0)
            {
                Exception myE = new Exception("a original clordid must be specified on a cancelOrder");
                throw myE;
            }
            ClOrdID = cancelRequest.ClOrdID;
            OrigClOrdID = cancelRequest.OrigClOrdID;
            CRRType = crrType.cancel;
   
        }

        public OrderContext OrderContext
        {
            get
            {
                return _orderContext;
            }
            set
            {
                _orderContext = value;
                _lastChangeTicks = DateTime.Now.Ticks;
            }
        }

        public override int  RetryCount
        {
            get
            {
                if (_cancelRequest.RetryCount.HasValue)
                {
                    return _cancelRequest.RetryCount.Value;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                _cancelRequest.RetryCount = value;
            }
        }

    }



    public class ModifyRequestData : RequestData
    {
        private KaiTrade.Interfaces.IModifyOrderRequst _modifyRequest = null;
        protected long _lastChangeTicks;
        protected OrderContext _orderContext;
        protected crrType _cRRType;
       

        public ModifyRequestData(crrType type, KaiTrade.Interfaces.IModifyOrderRequst modifyRequest)
        {
            _modifyRequest = modifyRequest;
            if (_modifyRequest == null)
            {
                Exception myE = new Exception("a non null modify request must be supplied");
                throw myE;
            }

            Mnemonic = modifyRequest.Mnemonic;
            

            _lastChangeTicks = DateTime.Now.Ticks;


            if (_modifyRequest.ClOrdID.Length == 0)
            {
                Exception myE = new Exception("a clordid must be specified on a modifyOrder");
                throw myE;
            }
            if (_modifyRequest.OrigClOrdID.Length == 0)
            {
                Exception myE = new Exception("a original clordid must be specified on a modifyOrder");
                throw myE;
            }
            ClOrdID = modifyRequest.ClOrdID;
            OrigClOrdID = modifyRequest.OrigClOrdID;
            CRRType = crrType.replace;

        }

        public OrderContext OrderContext
        {
            get
            {
                return _orderContext;
            }
            set
            {
                _orderContext = value;
                _lastChangeTicks = DateTime.Now.Ticks;
            }
        }

        public override int  RetryCount
        {
            get
            {
                if (_modifyRequest.RetryCount.HasValue)
                {
                    return _modifyRequest.RetryCount.Value;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                _modifyRequest.RetryCount = value;
            }
        }

        public double? Price
        {
            get { return _modifyRequest.Price; }
            set
            {
                _modifyRequest.Price = value;
                _lastChangeTicks = DateTime.Now.Ticks;
            }
        }

        public double? StopPrice
        {
            get { return _modifyRequest.StopPrice; }
            set
            {
                _modifyRequest.StopPrice = value;
                _lastChangeTicks = DateTime.Now.Ticks;
            }
        }

        public long? qty
        {
            get { return _modifyRequest.Qty; }
            set
            {
                _modifyRequest.Qty = value;
                _lastChangeTicks = DateTime.Now.Ticks;
            }
        }

        public void updateFrom(ModifyRequestData r)
        {
            if (r.Price.HasValue)
            {
                this.Price = r.Price;
            }
            if (r.StopPrice.HasValue)
            {
                this.StopPrice = r.StopPrice;
            }
            if (r.qty.HasValue)
            {
                this.qty = r.qty;
            }
            
            this.RetryCount = r.RetryCount;
        }


    }

    
}
