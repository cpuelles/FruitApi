using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Color
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    [JsonIgnore] 
    [InverseProperty(nameof(Fruit.ColorNavigation))]
    public virtual IEnumerable<Fruit>? Fruits { get; set; } = new List<Fruit>();
}