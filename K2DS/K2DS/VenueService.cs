using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using NLog;

namespace K2DS
{
    public class VenueService
    {
        private Logger m_log = LogManager.GetLogger("TBSLog");
         
        #region AccountService Members
        public List<KaiTrade.Interfaces.Venue> GetVenues()
        {

            var venues = new List<KaiTrade.Interfaces.Venue>();
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    var db = new DataClassesDataContext();
                    var venueList = from c in db.Venues
                                      select c;

                    foreach (Venue venue in venueList)
                    {
                        venues.Add(venue);
                    }

                    ts.Complete();
                }
                catch (Exception myE)
                {
                    Transaction.Current.Rollback();
                    m_log.Error("Get Venue", myE);
                }
            }
            return venues;
        }

        public void AddVenue(KaiTrade.Interfaces.Venue myVenue)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    var db = new DataClassesDataContext();
                    var venue = (Venue)myVenue;
                    db.Venues.InsertOnSubmit(venue);

                    db.SubmitChanges();
                    ts.Complete();
                }
                catch (Exception myE)
                {
                    Transaction.Current.Rollback();
                    m_log.Error("Add Venue", myE);
                }
            }
        }


        public void DeleteVenue(KaiTrade.Interfaces.Venue myVenue)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    var db = new DataClassesDataContext();
                    var venue = db.Venues.Where(s => s.Name == myVenue.Name).FirstOrDefault();
                    db.Venues.DeleteOnSubmit(venue);
                    db.SubmitChanges();
                    ts.Complete();
                }
                catch (Exception myE)
                {
                    Transaction.Current.Rollback();
                    m_log.Error("Delete Venue", myE);
                }
            }
        }

        public void DeactivateVenue(Boolean isActive, String name)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    var db = new DataClassesDataContext();
                    var venue =
                    (from c in db.Venues
                     where c.Name == name
                     select c).SingleOrDefault();

                    venue.IsActive = isActive;

                    db.SubmitChanges();
                    ts.Complete();
                }
                catch (Exception myE)
                {
                    Transaction.Current.Rollback();
                    m_log.Error("Deactivate Venue", myE);
                }
            }
        }

        public void UpdateVenue(KaiTrade.Interfaces.Venue myVenue)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    var db = new DataClassesDataContext();
                    var venue = db.Venues.Where(s => s.Name == myVenue.Name).FirstOrDefault();
                    venue.AccountNumber = myVenue.AccountNumber;
                    venue.Code = myVenue.Code;
                    venue.DataFeedVenue = myVenue.DataFeedVenue;
                    venue.DefaultCFICode = myVenue.DefaultCFICode;
                    venue.DefaultCurrencyCode = myVenue.DefaultCurrencyCode;
                    venue.DefaultSecurityExchnage = myVenue.DefaultSecurityExchnage;
                    venue.DriverCode = myVenue.DriverCode;
                    venue.IsActive = myVenue.IsActive;
                    venue.TargetVenue = myVenue.TargetVenue;
                    venue.UseSymbol = myVenue.UseSymbol;

                    db.SubmitChanges();
                    ts.Complete();
                }
                catch (Exception myE)
                {
                    Transaction.Current.Rollback();
                    m_log.Error("Update Venue", myE);
                }
            }
        }
        #endregion
    }
}
