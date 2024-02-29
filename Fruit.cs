using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Fruit
{
    public int Id { get; set; }
    [Required] public string Name { get; set; } = "";
    public bool Instock { get; set; }

    public int? ColorId { get; set; }
    [ForeignKey(nameof(ColorId))]
    [InverseProperty(nameof(Color.Fruits))]
    public virtual Color? ColorNavigation { get; set; }
}
