
namespace tourism_api.Domain
{
    public class RestaurantReview
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime DateOfReview { get; set; }
        public int ReservationId { get; set; }
        public int TouristId { get; set; }
        public User? Tourist { get; set; }


        public bool IsValid()
        {
            return this.Rating > 0;
        }



    }
}
