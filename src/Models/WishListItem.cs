using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Wish.Models;

public class WishListItem
{
    public static WishListItem NotFound = new WishListItem();
    [Key]
    public int Id { get; set; }
    [JsonIgnore]
    public string? OwnerID { get; set; }
    [Required]
    public string Name { get; set; }
    public DateTime Acquired { get; set; }
    public string? ImageURL { get; set; }
    public List<string>? DescriptionBodies { get; set; }
    public List<string>? DescriptionHeaders { get; set; }
    [NotMapped]
    [JsonIgnore]
    public IFormFile? Image { get; set; }
}