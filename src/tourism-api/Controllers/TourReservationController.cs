using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tourism_api.Domain;
using tourism_api.Repositories;

namespace tourism_api.Controllers
{
    [Route("api/tours/reservations")]
    [ApiController]
    public class TourReservationController : ControllerBase
    {
        private readonly TourReservationRepository _tourReservationRepo;
        private readonly TourRepository _tourRepo;
        private readonly UserRepository _userRepo;

        public TourReservationController(IConfiguration configuration)
        {
            _tourReservationRepo = new TourReservationRepository(configuration);
            _tourRepo = new TourRepository(configuration);
            _userRepo = new UserRepository(configuration);
        }

        [HttpGet("{touristId}")]

        public ActionResult<List<TourReservation>> GetByTourist(int touristId)
        {
            try
            {
                User user = _userRepo.GetById(touristId);
                if (user == null)
                {
                    return NotFound($"User with ID {touristId} not found.");
                }

                List<TourReservation> tourReservations = _tourReservationRepo.GetByTourist(touristId);
                foreach (TourReservation reservation in tourReservations)
                {
                    reservation.Tour = _tourRepo.GetById(reservation.TourId);
                }
                return Ok(tourReservations);
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while fetching the reservations.");
            }
        }

        [HttpPost]

        public ActionResult<TourReservation> Create([FromBody] TourReservation newReservation)
        {
            if (!newReservation.IsValid())
            {
                return BadRequest("Invalid reservation data.");
            }

            try
            {
                User user = _userRepo.GetById(newReservation.TouristId);
                if (user == null)
                {
                    return NotFound($"User with ID {newReservation.TouristId} not found.");
                }

                Tour tour = _tourRepo.GetById(newReservation.TourId);
                if (tour == null)
                {
                    return NotFound($"Tour with ID {newReservation.TourId} not found.");
                }

                List<TourReservation> tourReservations = _tourReservationRepo.GetByTour(newReservation.TourId);
                int tourCapacity = tour.MaxGuests;

                foreach (TourReservation reservation in tourReservations)
                {
                    tourCapacity -= reservation.Guests;
                }

                if (newReservation.Guests > tourCapacity)
                {
                    return BadRequest($"There is not enough capacity. Free space: {tourCapacity}.");
                }

                TourReservation createdReservation = _tourReservationRepo.Create(newReservation);
                return Ok(createdReservation);
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while creating the reservation.");
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                TourReservation reservation = _tourReservationRepo.GetById(id);
                Tour tour = _tourRepo.GetById(reservation.TourId);

                DateTime tourStart = tour.DateTime;
                DateTime now = DateTime.Now;
                TimeSpan difference = tourStart - now;

                if (difference.TotalHours < 24)
                {
                    return BadRequest("Operation not allowed. Tour starts in less than 24 hours.");
                }

                bool isDeleted = _tourReservationRepo.Delete(id);
                if (isDeleted)
                {
                    return NoContent();
                }
                return NotFound($"Reservation with ID {id} not found.");
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while deleting the reservation.");
            }
        }
    }
}
