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

namespace K2DS
{
    public class K2AccountDS
    {
        private log4net.ILog m_Log;

        public K2AccountDS()
        {
            m_Log = log4net.LogManager.GetLogger("K2DS");
        }

        public K2DataObjects.Account GetAccount(string accountCode)
        {
            using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
            {
                return db.Accounts.Where(a => a.AccountCode == accountCode).Single();
            }

        }
        
        public void Insert(K2DataObjects.Account account, bool allowUpdate)
        {
            try
            {               
               using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
                {
                    var dbAccounts =
                      (from a in db.Accounts
                       where a.AccountCode == account.AccountCode 
                       select a).SingleOrDefault();

                    if (dbAccounts != null)
                    {
                        if (allowUpdate)
                        {
                            db.Accounts.DeleteOnSubmit(dbAccounts);
                            db.SubmitChanges();
                        }
                        else
                        {
                            throw new Exception("account exists");
                        }
                    }
                    db.Accounts.InsertOnSubmit(account);

                    db.SubmitChanges();

                }

            }
            catch (Exception myE)
            {
                m_Log.Error("Insert", myE);
            }
        }

    }
}
