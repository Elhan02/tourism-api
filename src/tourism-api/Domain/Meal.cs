namespace tourism_api.Domain;

public class Meal
{
    public int Id { get; set; }
    public int Order { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public string Ingredients { get; set; }
    public string? ImageUrl { get; set; }
    public int RestaurantId { get; set; }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Name) && Price > 0 && !string.IsNullOrWhiteSpace(Ingredients);
    }
}
