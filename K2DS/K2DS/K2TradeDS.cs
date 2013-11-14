using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K2DS
{
    public class K2TradeDS
    {
        private log4net.ILog m_Log;

        public K2TradeDS()
        {
            m_Log = log4net.LogManager.GetLogger("K2DS");

        }

      

        public List<K2DataObjects.Trade> GetTrade(string identity)
        {
            using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
            {

                var trade =
                   (from t in db.Trades
                    where t.Identity == identity
                    select t);

                return (trade.ToList());

            }
        }

        /// <summary>
        /// Get the trades for some order
        /// </summary>
        /// <param name="O"></param>
        /// <returns></returns>
        public List<K2DataObjects.Trade> GetTrade(K2DataObjects.Order order)
        {
            using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
            {

                var trade =
                   (from ot in db.OrderTrades
                    from t in db.Trades
                    where (ot.OrderIdentity == order.Identity) && (ot.TradeIdentity == t.Identity) 
                    select t );

             
                return (trade.ToList());

            }
        }

        public void Insert(K2DataObjects.Trade t)
        {
            try
            {
                t.SystemDate = DateTime.Now;
                using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
                {
                    db.Trades.InsertOnSubmit(t);
                    db.SubmitChanges();
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("Insert", myE);
            }
        }

        public void Update(string Identity, K2DataObjects.Trade t)
        {
            try
            {
                using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
                {
                    // get the product
                    var existingTrade =
                      (from trades in db.Trades
                       where trades.Identity == Identity
                       select trades).SingleOrDefault();

                    // update the product
                    if (existingTrade != null)
                    {
                        existingTrade = t;
                    }

                    // replace the product
                    db.SubmitChanges();

                }
            }
            catch (Exception myE)
            {
            }
        }

        #region fillprocessing

        public List<K2DataObjects.Fill> GetFill(string exchID)
        {
            using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
            {

                var fill =
                   (from f in db.Fills
                    where f.Identity == exchID
                    select f);

                return (fill.ToList());

            }
        }

        public void InsertFill(K2DataObjects.Fill f, bool allowUpdate)
        {
            try
            {
                using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
                {
                 
                    var dbFills =
                      (from fill in db.Fills
                       where fill.Identity == f.Identity
                       select fill).SingleOrDefault();

                    if (dbFills != null)
                    {
                        if (allowUpdate)
                        {
                            db.Fills.DeleteOnSubmit(f);
                            db.SubmitChanges();
                            //dbOrders = order;
                            //dbOrders.
                        }
                        else
                        {
                            throw new Exception("Order already exists");
                        }
                    }
                    db.Fills.InsertOnSubmit(f);
                    db.SubmitChanges();


                }
            }
            catch (Exception myE)
            {
                m_Log.Error("InsertFill", myE);
            }
        }

        public void UpdateFill(string exchID, K2DataObjects.Fill f)
        {
            try
            {
                using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
                {
                    // get the product
                    var existingFill =
                      (from fills in db.Fills
                       where fills.Identity == exchID
                       select fills).SingleOrDefault();

                    // update the product
                    if (existingFill != null)
                    {
                        existingFill = f;
                    }

                    // replace the product
                    db.SubmitChanges();

                }
            }
            catch (Exception myE)
            {
                m_Log.Error("UpdateFill", myE);
            }
        }

        #endregion


    }
}
