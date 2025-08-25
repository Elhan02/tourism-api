
namespace tourism_api.Domain
{
    public class RestaurantReview
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime DateOfReview { get; set; }
        public int TouristId { get; set; }
        public int ResturantId { get; set; }
        public Restaurant Restaurant { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is RestaurantReview))
            { return false; }
            return obj is RestaurantReview review &&
                  this.TouristId == review.TouristId &&
                  this.ResturantId == review.ResturantId;
        }

        public bool IsValid()
        {
            return this.Rating > 0;
        }



    }
}
