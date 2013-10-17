
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using CQG;
using K2ServiceInterface;

namespace KTACQG
{
    public partial class KTACQG : DriverBase.DriverBase
    {
        private string getK2Side(eOrderSide side)
        {
            string strSide = "";
            switch (side)
            {
                case eOrderSide.osdBuy:
                    strSide = KaiTrade.Interfaces.Side.BUY;
                    break;
                case eOrderSide.osdSell:
                    strSide = KaiTrade.Interfaces.Side.SELL;
                    break;
                default:

                    //log.
                    //Exception myE = new Exception("CQG:Invalid order side:" + side.ToString());
                    //throw myE;
                    break;

            }
            return strSide;
        }


        /*
        /// <summary>
        /// Get the CQG order side from a QuickFix side
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        private eOrderSide getOrderSide(QuickFix.Side side)
        {
            eOrderSide myRet = eOrderSide.osdUndefined;
            switch (side.getValue())
            {
                case QuickFix.Side.BUY:
                    myRet = eOrderSide.osdBuy;
                    break;
                case QuickFix.Side.SELL:
                    myRet = eOrderSide.osdSell;
                    break;
                default:
                    Exception myE = new Exception("CQG:Invalid order side:" + side.ToString());
                    throw myE;

                    break;
            }
            return myRet;
        }
        
        /// <summary>
        /// Get the CQG order duration from a quickfix TimeInForce
        /// </summary>
        /// <param name="tif"></param>
        /// <returns></returns>
        private eOrderDuration getOrderDuration(QuickFix.TimeInForce tif)
        {
            eOrderDuration myRet = eOrderDuration.odUndefined;
            switch (tif.getValue())
            {
                case QuickFix.TimeInForce.DAY:
                    myRet = eOrderDuration.odDay;
                    break;
                case QuickFix.TimeInForce.GOOD_TILL_DATE:
                    myRet = eOrderDuration.odGoodTillDate;
                    break;
                case QuickFix.TimeInForce.GOOD_TILL_CANCEL:
                    myRet = eOrderDuration.odGoodTillCanceled;
                    break;
                case QuickFix.TimeInForce.FILL_OR_KILL:
                    myRet = eOrderDuration.odFOK;
                    break;

                default:
                    Exception myE = new Exception("CQG:Invalid order duration:" + tif.ToString());
                    throw myE;

                    break;
            }
            return myRet;
        }
         
         /// <summary>
        /// Get the CQG order duration from a quickfix TimeInForce
        /// </summary>
        /// <param name="tif"></param>
        /// <returns></returns>
        private eOrderDuration getOrderDuration(QuickFix.TimeInForce tif)
        {
            eOrderDuration myRet = eOrderDuration.odUndefined;
            switch (tif.getValue())
            {
                case QuickFix.TimeInForce.DAY:
                    myRet = eOrderDuration.odDay;
                    break;
                case QuickFix.TimeInForce.GOOD_TILL_DATE:
                    myRet = eOrderDuration.odGoodTillDate;
                    break;
                case QuickFix.TimeInForce.GOOD_TILL_CANCEL:
                    myRet = eOrderDuration.odGoodTillCanceled;
                    break;
                case QuickFix.TimeInForce.FILL_OR_KILL:
                    myRet = eOrderDuration.odFOK;
                    break;

                default:
                    Exception myE = new Exception("CQG:Invalid order duration:" + tif.ToString());
                    throw myE;

                    break;
            }
            return myRet;
        }

         
         */

    }
}
