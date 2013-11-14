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
    public class User
    {
       
        /// <summary>
        /// Set the password for some user
        /// </summary>
        /// <param name="ID">users unique identifier</param>
        /// <param name="myOldPwd">old password</param>
        /// <param name="myNewPwd">new password</param>
        public void SetPassWord(string ID, string myOldPwd, string myNewPwd)
        {
            try
            {
                K2DS.K2UserDS userDs = new K2DS.K2UserDS();
                IEnumerable<K2DataObjects.User> users = userDs.GetUsers(ID);
                if (users.Count() == 1)
                {
                    if (users.ElementAt(0).UserPwd == myOldPwd)
                    {
                        users.ElementAt(0).UserPwd = myNewPwd;
                        userDs.UpdateUser(users.ElementAt(0));
                    }
                }
            }
            catch (Exception myE)
            {
            }
        }

        /// <summary>
        /// Update a users profile based on their unique ID
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="myProfile"></param>
        public void UpdateProfile(string ID, string profile)
        {
            try
            {
                K2DS.K2UserDS userDs = new K2DS.K2UserDS();
                IEnumerable<K2DataObjects.User> users = userDs.GetUsers(ID);
                if (users.Count() == 1)
                {
                    users.ElementAt(0).K2Config = profile;
                    userDs.UpdateUser(users.ElementAt(0));
                }
            }
            catch (Exception myE)
            {
            }
        }

        public string getProfile(string ID)
        {
            string profile = "";
            try
            {
                K2DS.K2UserDS userDs = new K2DS.K2UserDS();
                IEnumerable<K2DataObjects.User> users = userDs.GetUsers(ID);
                if (users.Count() == 1)
                {
                    profile = users.ElementAt(0).K2Config;
                }
            }
            catch (Exception myE)
            {
            }
            return profile;
        }

        public void InsertUser(K2DataObjects.User user)
        {
            try
            {
                K2DS.K2UserDS userDs = new K2DS.K2UserDS();
                userDs.InsertUser(user);
                
            }
            catch (Exception myE)
            {
            }
        }

        /// <summary>
        /// Validate a user exists and sign them in
        /// </summary>
        /// <param name="myName"></param>
        /// <param name="myPwd"></param>
        /// <param name="myIP"></param>
        /// <returns>user data object</returns>
        public K2DataObjects.User ValidateUserSignOn(string myName, string myPwd, string myIP)
        {
            K2DataObjects.User user = null;

            try
            {
                K2DS.K2UserDS userDs = new K2DS.K2UserDS();
                IEnumerable<K2DataObjects.User> users = userDs.GetUsers(myName, myPwd);
                if (users.Count() == 1)
                {
                    user = users.ElementAt(0);
                    user.LastIP = myIP;
                    user.LastSignIn = DateTime.Now;
                    user.IsSignedIn = true;
                    userDs.UpdateUser(user);
                }
            }
            catch (Exception myE)
            {
            }
            return user;

        }

        /// <summary>
        /// signout a user based on their unique ID
        /// </summary>
        /// <param name="ID"></param>
        public void UserSignOut(string ID)
        {

            try
            {
                K2DS.K2UserDS userDs = new K2DS.K2UserDS();
                IEnumerable<K2DataObjects.User> users = userDs.GetUsers(ID);
                if (users.Count() == 1)
                {
                    users.ElementAt(0).IsSignedIn = false;
                }
            }
            catch (Exception myE)
            {
            }
        }
    }
}
