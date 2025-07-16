using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using tourism_api.Domain;

namespace tourism_api.Repositories
{
    public class TourReservationRepository
    {
        private readonly string _connectionString;
        public TourReservationRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionString:SQLiteConnection"];
        }

        public TourReservation GetById(int id)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @$"
                    SELECT r.Id, u.Id AS TouristId, r.Guests, t.Id AS TourId  
                    FROM ToursReservations r
                    INNER JOIN Users u ON r.TouristId = u.Id
                    LEFT JOIN Tours t ON r.TourId = t.Id
                    WHERE r.Id = @ReservationId;";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@ReservationId", id);

                using SqliteDataReader reader = command.ExecuteReader();

                TourReservation reservation = new TourReservation();

                if (reader.Read())
                {
                    reservation = new TourReservation
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        TouristId = Convert.ToInt32(reader["TouristId"]),
                        Guests = Convert.ToInt32(reader["Guests"]),
                        TourId = Convert.ToInt32(reader["TourId"])
                    };
                }
                return reservation;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greška pri konekciji ili izvršavanju neispravnih SQL upita: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greška u konverziji podataka iz baze: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena više puta: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neočekivana greška: {ex.Message}");
                throw;
            }
        }

        public List<TourReservation> GetByTourist(int touristId) 
        {
            List<TourReservation> reservations = new List<TourReservation>();

            try
            {
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @$"
                    SELECT r.Id, u.Id AS TouristId, r.Guests, t.Id AS TourId  
                    FROM ToursReservations r
                    INNER JOIN Users u ON r.TouristId = u.Id
                    LEFT JOIN Tours t ON r.TourId = t.Id
                    WHERE r.TouristId = @TouristId;";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@TouristId", touristId);

                using SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    reservations.Add(new TourReservation
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        TouristId = Convert.ToInt32(reader["TouristId"]),
                        Guests = Convert.ToInt32(reader["Guests"]),
                        TourId = Convert.ToInt32(reader["TourId"])
                    });
                }

                return reservations;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greška pri konekciji ili izvršavanju neispravnih SQL upita: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greška u konverziji podataka iz baze: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena više puta: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neočekivana greška: {ex.Message}");
                throw;
            }
        }

        public List<TourReservation> GetByTour(int tourId)
        {
            List<TourReservation> reservations = new List<TourReservation>();

            try
            {
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @$"
                    SELECT r.Id, u.Id AS TouristId, r.Guests, t.Id AS TourId  
                    FROM ToursReservations r
                    INNER JOIN Users u ON r.TouristId = u.Id
                    LEFT JOIN Tours t ON r.TourId = t.Id
                    WHERE r.TourId = @TourId;";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@TourId", tourId);

                using SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    reservations.Add(new TourReservation
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        TouristId = Convert.ToInt32(reader["TouristId"]),
                        Guests = Convert.ToInt32(reader["Guests"]),
                        TourId = Convert.ToInt32(reader["TourId"])
                    });
                }

                return reservations;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greška pri konekciji ili izvršavanju neispravnih SQL upita: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greška u konverziji podataka iz baze: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena više puta: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neočekivana greška: {ex.Message}");
                throw;
            }
        }

        public TourReservation Create(TourReservation reservation)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @"
                    INSERT INTO ToursReservations (TouristId, Guests, TourId)
                    VALUES (@TouristId, @Guests, @TourId);
                    SELECT LAST_INSERT_ROWID();";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@TouristId", reservation.TouristId);
                command.Parameters.AddWithValue("@Guests", reservation.Guests);
                command.Parameters.AddWithValue("@TourId", reservation.TourId);

                reservation.Id = Convert.ToInt32(command.ExecuteScalar());

                return reservation;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greška pri konekciji ili izvršavanju neispravnih SQL upita: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greška u konverziji podataka iz baze: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena više puta: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neočekivana greška: {ex.Message}");
                throw;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = "DELETE FROM ToursReservations WHERE Id = @Id";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greška pri konekciji ili izvršavanju neispravnih SQL upita: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena više puta: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neočekivana greška: {ex.Message}");
                throw;
            }
        }
    }
}
