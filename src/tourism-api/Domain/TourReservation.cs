using Microsoft.AspNetCore.Routing.Constraints;
using System.Xml.Linq;

namespace tourism_api.Domain
{
    public class TourReservation
    {
        public int Id { get; set; }

        public int TouristId { get; set; }
        public int Guests { get; set; }

        public int TourId { get; set; }
        public Tour? Tour { get; set; }

        public bool IsValid() 
        {
            return TouristId != null && TourId != null && Guests > 0;
        }
    }
}
