using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using tourism_api.Domain;
using tourism_api.Repositories;

namespace tourism_api.Controllers
{
    [ApiController]
    public class RestaurantReservationController : ControllerBase
    {
        private readonly RestaurantReservationRepository _restaurantReservationRepo;
        private readonly RestaurantRepository _restaurantRepo;
        private readonly UserRepository _userRepository;

        public RestaurantReservationController(IConfiguration configuration)
        {
            _restaurantReservationRepo = new RestaurantReservationRepository(configuration);
            _restaurantRepo = new RestaurantRepository(configuration);
            _userRepository = new UserRepository(configuration);
        }

        [HttpPost("api/restaurants/{restaurantId}/reservations")]
        public ActionResult<RestaurantReservation> Create(int restaurantId, [FromBody] RestaurantReservation newReservation)
        {
            if (newReservation == null || !newReservation.isValid())
            {
                return BadRequest("Invalid reservation data.");
            }

            try
            {
                Restaurant restaurant = _restaurantRepo.GetById(restaurantId);
                if (restaurant == null)
                {
                    return NotFound($"Restaurant with ID {restaurantId} not found.");
                }

                newReservation.RestaurantId = restaurantId;

                User tourist = _userRepository.GetById(newReservation.TouristId);
                if (tourist == null)
                {
                    return NotFound($"Tourist with ID {newReservation.TouristId} not found.");
                }

                int reservedSeats = _restaurantReservationRepo.countReservedSeats(newReservation.RestaurantId, newReservation.MealType, newReservation.ReservationDate);
                int availableSeats = restaurant.Capacity - reservedSeats;

                if (newReservation.NumberOfGuests <= availableSeats)
                {
                    RestaurantReservation createdReservation = _restaurantReservationRepo.Create(newReservation, restaurant.Capacity);
                    return Ok(createdReservation);
                }
                else 
                {
                    return BadRequest($"The number of available seats for {newReservation.MealType} on {newReservation.ReservationDate.ToString("dd.MM.yyyy.")} is {availableSeats}.");
                }

            }
            catch (Exception)
            {
                return Problem("An error occurred while creating new reservation.");
            }

        }

        [HttpGet("api/reservations")]
        public ActionResult<List<RestaurantReservation>> GetAllReservationsByTouristId([FromQuery] int touristId)
        {
            User tourist = _userRepository.GetById(touristId);
            if (tourist == null)
            {
                return NotFound($"Tourist with ID {touristId} not found.");
            }

            try
            {
                List<RestaurantReservation> reservations = _restaurantReservationRepo.GetAllReservationsByTouristId(touristId);
                return Ok(reservations);
            }
            catch (Exception)
            {
                return Problem("An error occured while fetching tourist restaurant reservations.");
            }
        }

        [HttpDelete("api/reservations/{reservationId}")]
        public ActionResult DeleteReservation(int reservationId) 
        {
            try
            {
                DateTime dateOfDeletion = DateTime.Now;

                RestaurantReservation reservation = _restaurantReservationRepo.GetById(reservationId);
                if (reservation == null)
                {
                    return NotFound($"Reservation with ID: {reservationId} not found.");
                }

                if (reservation.MealType.Trim().ToLower() == "breakfast")
                {
                    TimeSpan remainingTime = reservation.ReservationDate - dateOfDeletion;

                    if (remainingTime.TotalHours < 12)
                    {
                        return BadRequest("Breakfast cannot be cancelled less than 4 hours before the scheduled time.");
                    }
                    
                }
                else if (reservation.MealType.Trim().ToLower() == "lunch" || reservation.MealType.Trim().ToLower() == "dinner")
                {
                    TimeSpan remainingTime = reservation.ReservationDate - dateOfDeletion;

                    if (remainingTime.TotalHours < 4)
                    {
                        return BadRequest($"{reservation.MealType} cannot be cancelled less than 4 hours before the scheduled time.");
                    }
                }

                bool deletedReservation = _restaurantReservationRepo.Delete(reservationId);
                if (!deletedReservation)
                {
                    return NotFound($"Reservation with ID: {reservationId} not found.");
                }
                return Ok("Your reservation has been successfully canceled.");

            }
            catch (Exception)
            {
                return Problem("An error occured while deleting tourist restaurant reservations.");
            }
        }
    }
}
