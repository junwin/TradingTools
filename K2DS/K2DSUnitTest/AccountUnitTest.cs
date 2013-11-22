using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace K2DSUnitTest
{
    [TestClass]
    public class AccountUnitTest
    {
        [TestMethod]
        public void AddAccountTest()
        {
            K2DS.K2AccountDS svc = new K2DS.K2AccountDS();
            K2DataObjects.Account account = new K2DataObjects.Account();
            account.FirmCode = "ZZZZ";
            account.AccountCode = "ZZ01";
            account.LongName = DateTime.Now.Ticks.ToString();
            account.VenueCode = "V001";
            svc.Insert(account, true);

            var accountResult = svc.GetAccount("ZZ01");
            Assert.AreEqual(accountResult.AccountCode, account.AccountCode);

            Assert.AreEqual(accountResult.LongName, account.LongName);


        }
    }
}
