using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrewbayProject.Models;

public class AdministratorList
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("User")]
    public string UserId { get; set; }
}