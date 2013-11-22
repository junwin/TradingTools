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
using System.Linq;
using System.Text;

namespace K2DomainSvc
{
    public class Product
    {
        /// <summary>
        /// Inserts a product and creates exchange and venue if needed
        /// </summary>
        /// <param name="p"></param>
        /// <param name="deleteExisting"></param>
        public void Insert(K2DataObjects.Product p, bool deleteExisting)
        {
            try
            {
                K2DS.K2ProductDS productDS = new K2DS.K2ProductDS();
                List<K2DataObjects.Product> products = productDS.GetProducts(p.TradeVenue, p.Mnemonic);
                foreach (K2DataObjects.Product x in products)
                {
                    
                }
                if (products.Count() == 0)
                {
                    productDS.Insert(p);
                    if (productDS.GetExchange(p.Exchange).Count() == 0)
                    {
                        K2DataObjects.Exchange e = new K2DataObjects.Exchange();
                        e.ExchangeCode = p.Exchange;
                        e.Name = p.Exchange;
                        productDS.Insert(e);
                    }
                    if (productDS.GetVenue(p.TradeVenue).Count() == 0)
                    {
                        K2DataObjects.TradeVenue v = new K2DataObjects.TradeVenue();
                        v.Code = p.TradeVenue;
                        v.Name = p.TradeVenue;
                        productDS.Insert(v);
                    }
                }
                else
                {
                    if (deleteExisting)
                    {
                        productDS.Delete(p);
                        productDS.Insert(p);
                    }
                    
                }
                
            }
            catch (Exception myE)
            {
            }
        }
    }
}
