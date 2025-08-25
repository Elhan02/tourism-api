using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Diagnostics.Contracts;
using tourism_api.Domain;

namespace tourism_api.Repositories
{
    public class RestaurantReservationRepository
    {
        private readonly string _connectionString;

        public RestaurantReservationRepository(IConfiguration configuration)
        {
            this._connectionString = configuration["ConnectionString:SQLiteConnection"];
        }

        public RestaurantReservation Create(RestaurantReservation newReservation, int capacity)
        {

            try
            {
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @"INSERT INTO RestaurantReservation(TouristId, RestaurantId, ReservationDate, MealType, NumberOfGuests)
                                VALUES(@TouristId, @RestaurantId, @ReservationDate, @MealType, @NumberOfGuests); SELECT LAST_INSERT_ROWID();";

                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@TouristId", newReservation.TouristId);
                command.Parameters.AddWithValue("@RestaurantId", newReservation.RestaurantId);
                command.Parameters.AddWithValue("@ReservationDate", newReservation.ReservationDate);
                command.Parameters.AddWithValue("@MealType", newReservation.MealType);
                command.Parameters.AddWithValue("@NumberOfGuests", newReservation.NumberOfGuests);

                newReservation.Id = Convert.ToInt32(command.ExecuteScalar());
                return newReservation;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greska pri konekciji ili izvrsavanju neispravnih SQL naredbi: {ex.Message}");

                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greska u konverziji podataka iz baze: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena vise puta:{ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neocekivana greska: {ex.Message}");
                throw;
            }

        }

        public int countReservedSeats(int restaurantId, string mealType, DateTime reservationDate)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @"SELECT SUM(NumberOfGuests)
                            FROM RestaurantReservation
                            WHERE RestaurantId = @RestaurantId AND MealType = @MealType AND ReservationDate = @ReservationDate";

                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@RestaurantId", restaurantId);
                command.Parameters.AddWithValue("@MealType", mealType);
                command.Parameters.AddWithValue("@ReservationDate", reservationDate);

                int reservedSeats = command.ExecuteScalar() == DBNull.Value ? 0 : Convert.ToInt32(command.ExecuteScalar());
                return reservedSeats;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greska pri konekciji ili izvrsavanju neispravnih SQL naredbi: {ex.Message}");

                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greska u konverziji podataka iz baze: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena vise puta:{ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neocekivana greska: {ex.Message}");
                throw;
            }

        }

        public List<RestaurantReservation> GetAllReservationsByTouristId(int touristId)
        {

            try
            {
                List<RestaurantReservation> reservations = new List<RestaurantReservation>();
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();


                string query = @"
                            SELECT r.Id as RestaurantId, r.Name, r.Description, r.Capacity, r.ImageUrl, r.Latitude, r.Longitude, r.Status, r.OwnerId,
                            rr.Id as ReservationId, rr.TouristId, rr.ReservationDate, rr.MealType, rr.NumberOfGuests, rr.RestaurantId
                            FROM Restaurants r
                            INNER JOIN RestaurantReservation rr on r.ID =  rr.RestaurantID
                            WHERE TouristId = @TouristId";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@TouristId", touristId);

                using SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    RestaurantReservation reservation = new RestaurantReservation
                    {
                        Id = Convert.ToInt32(reader["ReservationId"]),
                        TouristId = Convert.ToInt32(reader["TouristId"]),
                        ReservationDate = Convert.ToDateTime(reader["ReservationDate"]),
                        MealType = Convert.ToString(reader["MealType"]),
                        NumberOfGuests = Convert.ToInt32(reader["NumberOfGuests"]),
                        RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                        Restaurant = new Restaurant
                        {
                            Id = Convert.ToInt32(reader["RestaurantId"]),
                            Name = Convert.ToString(reader["Name"]),
                            Description = Convert.ToString(reader["Description"]),
                            Capacity = Convert.ToInt32(reader["Capacity"]),
                            ImageUrl = Convert.ToString(reader["ImageUrl"]),
                            Latitude = Convert.ToDouble(reader["Latitude"]),
                            Longitude = Convert.ToDouble(reader["Longitude"]),
                            Status = Convert.ToString(reader["Status"]),
                            OwnerId = Convert.ToInt32(reader["OwnerId"])
                        },
                        
                    };
                    reservations.Add(reservation);
                }
                return reservations;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greska pri konekciji ili izvrsavanju neispravnih SQL naredbi: {ex.Message}");

                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greska u konverziji podataka iz baze: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena vise puta:{ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neocekivana greska: {ex.Message}");
                throw;
            }
        }

        public RestaurantReservation GetById(int reservationId) 
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @"SELECT r.Id, r.TouristId, r.ReservationDate, r.MealType, r.NumberOfGuests, r.RestaurantId
                                FROM RestaurantReservation r
                                WHERE r.Id = @Id;";

                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@Id", reservationId);

                using SqliteDataReader reader = command.ExecuteReader();


                if (reader.Read())
                {
                    RestaurantReservation reservation = new RestaurantReservation
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        TouristId = Convert.ToInt32(reader["TouristId"]),
                        ReservationDate = Convert.ToDateTime(reader["ReservationDate"]),
                        MealType = Convert.ToString(reader["MealType"]),
                        NumberOfGuests = Convert.ToInt32(reader["NumberOfGuests"]),
                        RestaurantId = Convert.ToInt32(reader["RestaurantId"]),
                    };
                    return reservation;
                }
                return null;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greska pri konekciji ili izvrsavanju neispravnih SQL naredbi: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greska u konverziji podataka iz baze: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena vise puta: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neocekivana greska: {ex.Message}");
                throw;
            }

        }
        public List<RestaurantReservation> GetAllTouristReservationsFromRestaurant(int restaurantId, int touristId) 
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(this._connectionString);
                connection.Open();

                string query = @"SELECT r.Id, r.TouristId, r.ReservationDate, r.MealType, r.NumberOfGuests, r.RestaurantId
                            FROM RestaurantReservation r
                            WHERE r.RestaurantId = @RestaurantId AND r.TouristId = @TouristId";

                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@RestaurantId", restaurantId);
                command.Parameters.AddWithValue("@TouristId", touristId);

                using SqliteDataReader reader = command.ExecuteReader();
                List<RestaurantReservation> restaurantReservations = new List<RestaurantReservation>();
                while (reader.Read())
                {
                    RestaurantReservation restaurantReservation = new RestaurantReservation
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        TouristId = Convert.ToInt32(reader["TouristId"]),
                        ReservationDate = Convert.ToDateTime(reader["ReservationDate"]),
                        MealType = Convert.ToString(reader["MealType"]),
                        NumberOfGuests = Convert.ToInt32(reader["NumberOfGuests"]),
                        RestaurantId = Convert.ToInt32(reader["RestaurantId"])
                    };
                    restaurantReservations.Add(restaurantReservation);
                }
                return restaurantReservations;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greska pri konekciji ili pri izvrsavanju neisparvnih SQLite naredbi: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greska pri konvertovanju podataka iz baze: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena vise puta: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neocekivana greska: {ex.Message}");
                throw;
            }
        }

        public bool Delete(int reservationId)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @"DELETE FROM RestaurantReservation WHERE Id = @Id";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@Id", reservationId);

                int affectedRows = command.ExecuteNonQuery();

                return affectedRows > 0;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greska pri konekciji ili izvrsavanju neispravnih SQL naredbi: {ex.Message}");
                throw;
            }           
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena vise puta: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neocekivana greska: {ex.Message}");
                throw;
            }
        }
    }
}
