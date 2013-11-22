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



    public class K2UserDS
    {
        private log4net.ILog m_Log;

        private string m_ConnectString;
        

        public K2UserDS()
        {
            m_Log = log4net.LogManager.GetLogger("K2DS");
            
        }

          
        /// <summary>
        /// Add a user to the database
        /// </summary>
        /// <param name="user">user data object</param>
        public void InsertUser(K2DataObjects.User user)
        {
            try
            {
                using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
                {
                    user.LastSignIn = DateTime.Now;
                    user.IsSignedIn = false;
                    db.Users.InsertOnSubmit(user);
                    db.SubmitChanges();
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("Add User", myE);
                throw myE;
            }
        }

        public void UpdateUser(K2DataObjects.User userUpdate)
        {
            try
            {
                using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
                {
                    var user =
                      (from c in db.Users
                       where c.ID == userUpdate.ID 
                       select c).SingleOrDefault();

                    if (user != null)
                    {
                        user = userUpdate;
                    }
                    db.SubmitChanges();

                }
            }
            catch (Exception myE)
            {
                m_Log.Error("Update User", myE);
                throw myE;
            }
        }

        public List<K2DataObjects.User> GetUsers()
        {
            using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
            {

                var user =
                  (from c in db.Users
                   select c);

                return (user.ToList());

            }
        }

        public List<K2DataObjects.User> GetUsers(string ID)
        {
            using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
            {

                var user =
                  (from c in db.Users
                   where c.ID == ID 
                   select c);

                return (user.ToList());

            }
        }

        public List<K2DataObjects.User> GetUsers(string name, string pwd)
        {
            try
            {
                using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
                {

                    var user =
                      (from c in db.Users
                       where c.UserID == name && c.UserPwd == pwd
                       select c);

                    return (user.ToList());
                    
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("Get Users", myE);
                throw myE;
            }
            return null;
        }

        

        
        /// <summary>
        /// Delete a user based on their unique ID
        /// </summary>
        /// <param name="myID"></param>
        public void DeleteUser(string myID)
        {
            try
            {
                using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
                {
                    var user =
                      (from c in db.Users
                       where c.ID == myID
                       select c).SingleOrDefault();

                    if (user != null)
                    {
                        db.Users.DeleteOnSubmit(user);
                        db.SubmitChanges();
                    }


                }
            }
            catch (Exception myE)
            {
                m_Log.Error("Delete User", myE);
                throw myE;
            }
        }

        

       
        

    }
}
