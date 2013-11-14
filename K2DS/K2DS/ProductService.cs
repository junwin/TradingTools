using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Transactions;
using NLog;

namespace K2DS
{
    public class ProductService
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
                m_log.Error("Load Product XML", myE);
            }

            var query = from c in xmlDoc.Descendants(ns + "Product")
                        select c;
            return query;

        }

        public void AddProduct(KaiTrade.Interfaces.TradableProduct myProduct)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    var db = new DataClassesDataContext();
                    var product = (Product)myProduct;

                    db.Products.InsertOnSubmit(product);
                    db.SubmitChanges();
                    ts.Complete();
                }
                catch (Exception myE)
                {
                    Transaction.Current.Rollback();
                    m_log.Error("AddProduct", myE);
                }
            }


        }

        public void LoadProducts2Db(List<TBS.DAL.Product> myProducts)
        {

            foreach (var product in myProducts)
            {
                AddProduct(product);
            }

        }

       
        #endregion
    }
}
