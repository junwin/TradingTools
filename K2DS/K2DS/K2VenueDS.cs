using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace K2DS
{
    public class K2VenueDS
    {
        private log4net.ILog m_Log;
         
        #region AccountService Members
        public List<KaiTrade.Interfaces.IVenue> GetVenues()
        {

            var venues = new List<KaiTrade.Interfaces.IVenue>();

            try
            {
                using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
                {
                    var venueList = from c in db.Venues
                                    select c;

                    foreach (K2DataObjects.TradeVenue venue in venueList)
                    {
                        venues.Add(venue);
                    }
                }

                
            }
            catch (Exception myE)
            {
                
                m_Log.Error("Get Venue", myE);
            }

            return venues;
        }

        public void AddVenue(KaiTrade.Interfaces.IVenue myVenue)
        {
            
                try
                {
                    using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
                    {
                        var venue = (K2DataObjects.TradeVenue)myVenue;
                        db.Venues.InsertOnSubmit(venue);

                        db.SubmitChanges();
                    }
                }
                catch (Exception myE)
                {
 
                    m_Log.Error("Add Venue", myE);
                }
            
        }


        public void DeleteVenue(KaiTrade.Interfaces.IVenue myVenue)
        {

                try
                {
                    using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
                    {
                        var venue = db.Venues.Where(s => s.Name == myVenue.Name).FirstOrDefault();
                        db.Venues.DeleteOnSubmit(venue);
                        db.SubmitChanges();
                    }
 
                }
                catch (Exception myE)
                {

                    m_Log.Error("Delete Venue", myE);
                }
           
        }

       

        public void UpdateVenue(KaiTrade.Interfaces.IVenue myVenue)
        {

                try
                {
                    using (K2DataObjects.DataContext db = Factory.Instance().GetDSContext())
                    {
                        var venue = db.Venues.Where(s => s.Name == myVenue.Name).FirstOrDefault();
                        venue.AccountNumber = myVenue.AccountNumber;
                        venue.Code = myVenue.Code;
                        venue.DataFeedVenue = myVenue.DataFeedVenue;
                        venue.DefaultCFICode = myVenue.DefaultCFICode;
                        venue.DefaultCurrencyCode = myVenue.DefaultCurrencyCode;
                        venue.DefaultSecurityExchange = myVenue.DefaultSecurityExchange;
                        venue.DriverCode = myVenue.DriverCode;                       
                        venue.TargetVenue = myVenue.TargetVenue;
                        venue.UseSymbol = myVenue.UseSymbol;

                        db.SubmitChanges();
                    }

                }
                catch (Exception myE)
                {

                    m_Log.Error("Update Venue", myE);
                }

        }
        #endregion
    }
}
