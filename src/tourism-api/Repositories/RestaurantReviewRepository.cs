using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Data.Sqlite;
using tourism_api.Domain;

namespace tourism_api.Repositories
{
    public class RestaurantReviewRepository
    {
        public readonly string _connectionString;

        public RestaurantReviewRepository(IConfiguration configuration)
        {
            this._connectionString = configuration["ConnectionString:SQLiteConnection"];
        }

        public RestaurantReview Create(RestaurantReview restaurantReview)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(this._connectionString);
                connection.Open();

                string queryString = @"INSERT INTO RestaurantReview(Rating, Comment, DateOfReview,TouristId, ReservationId)
                                    VALUES(@Rating, @Comment, @DateOfReview, @TouristId, @ReservationId); SELECT LAST_INSERT_ROWID()";
                using SqliteCommand command = new SqliteCommand(queryString, connection);
                command.Parameters.AddWithValue("@Rating", restaurantReview.Rating);
                command.Parameters.AddWithValue("@Comment", restaurantReview.Comment);
                command.Parameters.AddWithValue("@DateOfReview", restaurantReview.DateOfReview);
                command.Parameters.AddWithValue("@TouristId", restaurantReview.TouristId);
                command.Parameters.AddWithValue("@ReservationId", restaurantReview.ReservationId);
                restaurantReview.Id = Convert.ToInt32(command.ExecuteScalar());
                return restaurantReview;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greska pri konekciji ili izvrsavanju neisparvnih SQL naredbi: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greska pri konverziji podataka iz baze: {ex.Message}");
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

        public double GetAverageRating(int restaurantId)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(this._connectionString);
                connection.Open();

                string query = @"SELECT AVG(Rating)
                            FROM RestaurantReview r
                            INNER JOIN RestaurantReservation res ON r.ReservationId = res.Id
                            WHERE res.RestaurantId = @RestaurantId;";

                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@RestaurantId", restaurantId);

                double averageRating = Convert.ToInt32(command.ExecuteScalar());
                return averageRating;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greska pri konekciji ili pri izvrsavanju neispravnih SQL naredbi: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greska pri konverziji podataka iz baze: {ex.Message}");
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

        public List<RestaurantReview> GetAllByRestaurantId(int restaurantId)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(this._connectionString);
                connection.Open();

                string query = @$"SELECT r.Id, r.Rating, r.Comment, r.DateOfReview, r.TouristId, r.ReservationId,
                                res.Id as ReservationId, res.RestaurantId
                            FROM RestaurantReview r
                            LEFT JOIN RestaurantReservation res ON r.ReservationId = res.Id
                            WHERE res.RestaurantId = @RestaurantId";

                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@RestaurantId", restaurantId);

                using SqliteDataReader reader = command.ExecuteReader();
                List<RestaurantReview> reviews = new List<RestaurantReview>();
                while (reader.Read())
                {
                    RestaurantReview restaurantReview = new RestaurantReview
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Rating = Convert.ToInt32(reader["Rating"]),
                        Comment = reader["Comment"].ToString(),
                        DateOfReview = Convert.ToDateTime(reader["DateOfReview"]),
                        ReservationId = Convert.ToInt32(reader["ReservationId"])
                    };
                    reviews.Add(restaurantReview);
                }
                return reviews;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greska pri konekciji ili pri izvrsavanju nesipravnih SQL naredbi: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greska pri konverziji podataka iz baze: {ex.Message}");
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

        public List<RestaurantReview> GetByRestaurantIdSorted(int restaurantId, string orderBy)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(this._connectionString);
                connection.Open();

                string query = @$"SELECT r.Id as ReviewId, r.Rating, r.Comment, r.DateOfReview, r.TouristId ,r.ReservationId as ReviewReservationId,
                                    res.Id as ReservationId, res.RestaurantId,
                                    u.Id as UserId, u.Username, u.Password, u.Role
                            FROM RestaurantReview r
                            LEFT JOIN RestaurantReservation res ON r.ReservationId = res.Id
                            LEFT JOIN Users u ON  res.TouristId = UserId
                            WHERE res.RestaurantId = @RestaurantId
                            ORDER BY {orderBy}";

                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@RestaurantId", restaurantId);

                using SqliteDataReader reader = command.ExecuteReader();
                List<RestaurantReview> reviews = new List<RestaurantReview>();
                while (reader.Read())
                {
                    RestaurantReview restaurantReview = new RestaurantReview
                    {
                        Id = Convert.ToInt32(reader["ReviewId"]),
                        Rating = Convert.ToInt32(reader["Rating"]),
                        Comment = reader["Comment"].ToString(),
                        DateOfReview = Convert.ToDateTime(reader["DateOfReview"]),
                        TouristId= Convert.ToInt32(reader["TouristId"]),
                        ReservationId = Convert.ToInt32(reader["ReviewReservationId"]),
                        Tourist = new User
                        {
                            Username = reader["Username"].ToString(),
                            Password = reader["Password"].ToString(),
                            Role = reader["Role"].ToString()
                        }
                    };
                    reviews.Add(restaurantReview);
                }
                return reviews;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greska pri konekciji ili pri izvrsavanju nesipravnih SQL naredbi: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greska pri konverziji podataka iz baze: {ex.Message}");
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
