using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using tourism_api.Domain;
using tourism_api.Repositories;

namespace tourism_api.Controllers
{
    [Route("api/restaurants/{restaurantId}/reviews")]
    [ApiController]

    public class RestaurantReviewController : ControllerBase
    {
        private readonly RestaurantReviewRepository _reviewRepository;
        private readonly RestaurantReservationRepository _reservationRepository;
        private readonly UserRepository _userRepository;
        private readonly RestaurantRepository _restaurantRepository;

        public RestaurantReviewController(IConfiguration configuration)
        {
            _reviewRepository = new RestaurantReviewRepository(configuration);
            _reservationRepository = new RestaurantReservationRepository(configuration);
            _userRepository = new UserRepository(configuration);
            _restaurantRepository = new RestaurantRepository(configuration);
        }

        [HttpPost]
        public ActionResult<RestaurantReview> Create(int restaurantId, [FromBody] RestaurantReview restaurantReview)
        {
            if (restaurantReview == null || !restaurantReview.IsValid())
            {
                return BadRequest("Invalid review data.");
            }

            Restaurant restaurant = _restaurantRepository.GetById(restaurantId);
            if (restaurant == null)
            {
                return NotFound($"Restaurant with Id {restaurantId} not found.");
            }
            restaurantReview.ResturantId = restaurantId;

            User tourist = _userRepository.GetById(restaurantReview.TouristId);
            if (tourist == null)
            {
                return NotFound($"Tourist with Id {restaurantReview.TouristId} not found.");
            }

            try
            {
                List<RestaurantReservation> reservations = _reservationRepository.GetAllTouristReservationsFromRestaurant(restaurantId, restaurantReview.TouristId);
                List<RestaurantReview> restaurantReviews = _reviewRepository.GetAllByRestaurantId(restaurantId);

                foreach (RestaurantReview review in restaurantReviews)
                {
                    if (restaurantReview.Equals(review))
                    {
                        return Conflict("Restaurant review exists.");
                    }
                }

                foreach (RestaurantReservation reservation in reservations)
                {
                    TimeSpan timeFromReservation = DateTime.Now - reservation.ReservationDate;
                    double hoursFromReservation = timeFromReservation.TotalHours;

                    if (hoursFromReservation > 1 && hoursFromReservation < 72)
                    {
                        RestaurantReview createdRestaurantReview = _reviewRepository.Create(restaurantReview);
                        restaurant.AverageRating = _reviewRepository.GetAverageRating(restaurantId);
                        _restaurantRepository.Update(restaurant);
                        return Ok(createdRestaurantReview);
                    }
                }

                return BadRequest("Rating not allowed: must be 1–72 hours after reservation time.");
            }
            catch (Exception ex)
            {

                return Problem("An error occured wihle creating review.");
            }
        }

        [HttpGet]
        public ActionResult<List<RestaurantReview>> GetByRestaurantIdSorted(int restaurantId, [FromQuery] string orderBy) 
        {
            Restaurant restaurant = _restaurantRepository.GetById(restaurantId);
            if (restaurant == null)
            {
                return NotFound($"Restaurant with id {restaurantId} not found.");
            }

            List<string> validOrderByColumn = new List<string> { "DateOfReview", "Rating"};
            if (!validOrderByColumn.Contains(orderBy))
            {
                orderBy = "DateOfReview";
            }

            try
            {
                List<RestaurantReview> reviews = _reviewRepository.GetByRestaurantIdSorted(restaurantId, orderBy);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return Problem("An error occured while fetching restaurant reviews.");
            }
        }
    }
}
