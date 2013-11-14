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

namespace K2DS
{
    public class K2ProductDS
    {
        private log4net.ILog m_Log;

        public K2ProductDS()
        {
            m_Log = log4net.LogManager.GetLogger("K2DS");
        }

        public List<K2DataObjects.Product> GetProducts(string mnemonic)
        {
            using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
            {
                var product =
                   (from p in db.Products
                    where p.Mnemonic == mnemonic
                    select p);

                return (product.ToList());

            }
        }

        public List<K2DataObjects.Product> GetProducts(string venueCode, string mnemonic)
        {
            using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
            {
                var product =
                   (from p in db.Products
                    where (p.Mnemonic == mnemonic) && (p.TradeVenue == venueCode)
                    select p);

                return (product.ToList());

            }
        }

        public List<K2DataObjects.Product> GetProduct(string identity)
        {

            using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
            {

                var product =
                   (from p in db.Products
                    where p.Identity == identity
                    select p);
                return product.ToList();

            }
            return null;
        }

        public void Insert(K2DataObjects.Product p)
        {
            try
            {
                using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
                {
                    db.Products.InsertOnSubmit(p);
                    db.SubmitChanges();
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("Insert", myE);
            }
        }

        public void Delete(K2DataObjects.Product p)
        {
            try
            {
                
                using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
                {
                    var products =
                      (from prod in db.Products
                       where prod.Mnemonic == p.Mnemonic
                       select prod).SingleOrDefault();

                    if (products != null)
                    {
                        db.Products.DeleteOnSubmit(products);
                        db.SubmitChanges();
                    }


                }
            }
            catch (Exception myE)
            {
                m_Log.Error("Delete", myE);
            }
        }

        public void Update(string Identity, K2DataObjects.Product p)
        {
            try
            {
                using (K2DataObjects.DataContext context = Factory.Instance().GetDSContext())
                {
                    // get the product
                    var existingProduct =
                      (from products in context.Products
                       where products.Identity == Identity
                       select products).SingleOrDefault();

                    // update the product
                    if (existingProduct != null)
                    {
                        existingProduct = p;
                    }

                    // replace the product
                    context.SubmitChanges();

                }
            }
            catch (Exception myE)
            {
            }
        }



        public List<K2DataObjects.TradeVenue> GetVenue(string venueCode)
        {

            using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
            {

                var venue =
                   (from v in db.Venues
                    where v.Code == venueCode
                    select v);

                return (venue.ToList());

            }
        }

        public void Insert(K2DataObjects.TradeVenue v)
        {
            try
            {
                using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
                {
                    db.Venues.InsertOnSubmit(v);
                    db.SubmitChanges();
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("Insert", myE);
            }
        }

        public void Update(string venueCode, K2DataObjects.TradeVenue v)
        {
            try
            {
                using (K2DataObjects.DataContext context = Factory.Instance().GetDSContext())
                {
                    // get the product
                    var existingVenue =
                      (from venues in context.Venues
                       where venues.Code == venueCode
                       select venues).SingleOrDefault();

                    // update the product
                    if (existingVenue != null)
                    {
                        existingVenue = v;
                    }

                    // replace the product
                    context.SubmitChanges();

                }
            }
            catch (Exception myE)
            {
            }
        }


        public List<K2DataObjects.Exchange> GetExchange(string exchCode)
        {

            using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
            {

                var exch =
                   (from e in db.Exchanges
                    where e.ExchangeCode == exchCode
                    select e);

                return (exch.ToList());

            }
        }

        public void Insert(K2DataObjects.Exchange e)
        {
            try
            {
                using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
                {
                    db.Exchanges.InsertOnSubmit(e);
                    db.SubmitChanges();
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("Insert", myE);
            }
        }

        public void Update(string exchCode, K2DataObjects.Exchange e)
        {
            try
            {
 

                using (K2DataObjects.DataContext context = Factory.Instance().GetDSContext())
                {
                    // get the product
                    var existingExch =
                      (from exchs in context.Exchanges
                       where exchs.ExchangeCode == exchCode
                       select exchs).SingleOrDefault();

                    // update the product
                    if (existingExch != null)
                    {
                        existingExch = e;
                    }

                    // replace the product
                    context.SubmitChanges();

                }
            }
            catch (Exception myE)
            {
            }
        }

    }
}
