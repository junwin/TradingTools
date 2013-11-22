using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace K2DSUnitTest
{
    [TestClass]
    public class UserUnitTest
    {
        [TestMethod]
        public void AddUserTest()
        {
            K2DataObjects.User user = new K2DataObjects.User();

            user.UserID = "ablauder";
            user.UserPwd = "abcdef";
            user.K2Config = "<>";
            user.UserName = "Fred Quimby";
            user.Enabled = false;

            K2DS.K2UserDS ds = new K2DS.K2UserDS();
            ds.InsertUser(user);

            List<K2DataObjects.User> userList = ds.GetUsers();

            var resultUser = userList.Where(u => u.UserID == "ablauder").Single();

            Assert.AreEqual(user.UserID, resultUser.UserID);

            Assert.AreEqual(user.UserPwd, resultUser.UserPwd);



        }
    }
}
