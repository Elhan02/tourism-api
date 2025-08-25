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

                string queryString = @"INSERT INTO RestaurantReview(Review, Comment, DateOfReview, TouristId, RestaurantId)
                                    VALUES(@Rating, @Comment, @DateOfReview, @TouristId, @RestaurantId); SELECT LAST_INSERT_ROWID()";
                using SqliteCommand command = new SqliteCommand(queryString, connection);
                command.Parameters.AddWithValue("@Rating", restaurantReview.Rating);
                command.Parameters.AddWithValue("@Comment", restaurantReview.Comment);
                command.Parameters.AddWithValue("@TouristId", restaurantReview.TouristId);
                command.Parameters.AddWithValue("@DateOfReview", restaurantReview.DateOfReview);


                command.Parameters.AddWithValue("@RestaurantId", restaurantReview.ResturantId);
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
                            FROM RestaurantReview
                            WHERE RestaurantId = @RestaurantId;";

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

                string query = @$"SELECT r.Id, r.Rating, r.Comment, r.DateOfReview, r.TouristId, r.RestaurantId
                            FROM RestaurantReview r
                            WHERE r.RestaurantId = @RestaurantId";

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
                        TouristId = Convert.ToInt32(reader["TouristId"]),
                        ResturantId = Convert.ToInt32(reader["RestaurantId"])
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

        public List<RestaurantReview> GetByRestaurantIdSorted(int restaurantId, string? orderBy)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(this._connectionString);
                connection.Open();

                string query = @$"SELECT r.Id, r.Rating, r.Comment, r.DateOfReview, r.TouristId, r.RestaurantId
                            FROM RestaurantReview r
                            WHERE r.RestaurantId = @RestaurantId
                            ORDER BY {orderBy}";
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
                        TouristId = Convert.ToInt32(reader["TouristId"]),
                        ResturantId = Convert.ToInt32(reader["RestaurantId"])
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
