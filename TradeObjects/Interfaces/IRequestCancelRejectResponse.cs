using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaiTrade.Interfaces
{
    public enum CxlRejResponseTo { Cancel, Modify, None }
    public enum CxlRejReason { TooLateToCancel,InvalidPriceIncrement, UnknownOrder, BrokerCredit, OrderAlreadyInPendingStatus, 
                        UnableToProcessOrderMassCancelRequest, OrigOrdModTime, DuplicateClOrdID, PriceExceedsCurrentPrice,
                        PriceExceedsCurrentPriceBand,Other }

    public interface IRequestCancelRejectResponse
    {
        string OrderID { get; set; }
        string ClOrdID { get; set; }
        string OrigClOrdID { get; set; }
        string OrdStatus { get; set; }
        CxlRejResponseTo CxlRejResponseTo { get; set; }
        DateTime TransactTime { get; set; }
        CxlRejReason CxlRejReason { get; set; }
        string ReasonText { get; set; }

    }
}
