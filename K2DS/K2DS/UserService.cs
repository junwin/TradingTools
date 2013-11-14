using System;
using System.Linq;
using NLog;

namespace K2DS
{
    public class UserService
    {

        private Logger m_log = LogManager.GetLogger("TBSLog");
        
        #region Constructor
        public UserService()
        {
           
        } 
        #endregion

        #region Methods
        public KaiTrade.Interfaces.User GetUser(String userName, String password)
        {
            KaiTrade.Interfaces.User myRet = null;
            try
            {
                DataClassesDataContext context = new DataClassesDataContext();
                using (context)
                {
                    var user =
                     (from c in context.Users
                      where c.UserName == userName && c.Password == password
                      select c).SingleOrDefault();

                    if (user != null)
                    {
                        myRet = user;
                        
                    }
                   
                    context.SubmitChanges();
                }
            }
            catch (Exception myE)
            {
                m_log.Error("GetUser", myE);
            }
            return myRet;
           
        }

      
       
        #endregion
    }
}
