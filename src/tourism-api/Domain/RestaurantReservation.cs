namespace tourism_api.Domain
{
    public class RestaurantReservation
    {
        public int Id { get; set; }
        public int TouristId { get; set; }

        public DateTime ReservationDate { get; set; }

        public string MealType { get; set; }
        public int NumberOfGuests { get; set; }
        public int RestaurantId { get; set; }

        public Restaurant? Restaurant { get; set; }

        public bool isValid()
        {
            return NumberOfGuests >= 0 && ReservationDate > DateTime.Now && !string.IsNullOrWhiteSpace(MealType);
        }


    }
}
