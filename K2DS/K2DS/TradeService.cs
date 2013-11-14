using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Transactions;
using NLog;

namespace K2DS
{
    public class TradeService
    {

        private Logger m_log = LogManager.GetLogger("TBSLog");
        #region TradeService Members
        public IEnumerable<XElement> LoadXML(String xmlFile)
        {
            XDocument doc = null;
            XDocument xmlDoc = null;
            XNamespace ns = null;
            try
            {
                doc = XDocument.Load(xmlFile);
                ns = "http://www.kaitrade.com/KA";
                xmlDoc = XDocument.Load(xmlFile);
            }
            catch (Exception myE)
            {
                m_log.Error("Load Trade XML", myE);
            }
           
            var query = from c in xmlDoc.Descendants(ns + "Trade")
                        select c;
            return query;

        }

        public void AddTrade(KaiTrade.Interfaces.Trade myTrade)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    var db = new DataClassesDataContext();
                    var trade = (Trade)myTrade;

                    db.Trades.InsertOnSubmit(trade);
                    db.SubmitChanges();
                    ts.Complete();
                }
                catch (Exception myE)
                {
                    Transaction.Current.Rollback();
                    m_log.Error("Add Trade", myE);
                }
            }


        }

        public void LoadTrades2Db(List<TBS.DAL.Trade> myTrades)
        {

            foreach (var trade in myTrades)
            {
                AddTrade(trade);
            }

        }

        public List<KaiTrade.Interfaces.Trade> GetTrades(DateTime start, DateTime end)
        {
            List<KaiTrade.Interfaces.Trade> trade = null;
            try
            {
                var db = new DataClassesDataContext();
                var trades = from p in db.Trades
                             where DateTime.Parse(p.TransactTime) >= start &&
                             DateTime.Parse(p.TransactTime) <= end
                             select p;

                if (trades != null)
                {

                    trade = (List<Interfaces.Trade>)trades;
                }
            }
            catch (Exception myE)
            {

                m_log.Error("Get Trades", myE);
            }
           

            return trade;
        } 
        #endregion
    }
}
