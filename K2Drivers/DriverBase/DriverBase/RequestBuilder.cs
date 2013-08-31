using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DriverBase
{
    public class RequestBuilder
    {
        public static string DoRejectCxlReq(KaiTrade.Interfaces.ICancelOrderRequest cancelReq, string orderID, KaiTrade.Interfaces.CxlRejReason myReason, string myReasonText)
        {
            KaiTrade.Interfaces.IRequestCancelRejectResponse cxl = new K2DataObjects.RequestCancelRejectResponse();
            cxl.OrderID = orderID;
            cxl.ClOrdID = cancelReq.ClOrdID;
            cxl.OrdStatus = KaiTrade.Interfaces.OrderStatus.REJECTED;
            cxl.OrigClOrdID = cancelReq.OrigClOrdID;
            cxl.CxlRejResponseTo = KaiTrade.Interfaces.CxlRejResponseTo.Cancel;
            cxl.ReasonText = myReasonText;
            cxl.TransactTime = DateTime.Now;
            cxl.CxlRejReason = myReason;

            // send our response message back to the clients of the adapter
            return JsonConvert.SerializeObject(cxl);
        }

        /// <summary>
        /// Reject a cancel request
        /// </summary>
        /// <param name="myReq"></param>
        public static string DoRejectModReq(KaiTrade.Interfaces.IModifyOrderRequst mod, string orderID, KaiTrade.Interfaces.CxlRejReason myReason, string myReasonText)
        {
            KaiTrade.Interfaces.IRequestCancelRejectResponse cxl = new K2DataObjects.RequestCancelRejectResponse();
            cxl.OrderID = orderID;
            cxl.ClOrdID = mod.ClOrdID;
            cxl.OrdStatus = KaiTrade.Interfaces.OrderStatus.REJECTED;
            cxl.OrigClOrdID = mod.OrigClOrdID;
            cxl.CxlRejResponseTo = KaiTrade.Interfaces.CxlRejResponseTo.Modify;
            cxl.ReasonText = myReasonText;
            cxl.TransactTime = DateTime.Now;
            cxl.CxlRejReason = myReason;

            // send our response message back to the clients of the adapter
            return JsonConvert.SerializeObject(cxl);
           
        }

    }
}
