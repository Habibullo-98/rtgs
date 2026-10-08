using System.ComponentModel.DataAnnotations;

namespace RTGS.Models.Controller;

public class StatusRequest
{
    [Required(ErrorMessage = "Id is required")]
    [MinLength(1, ErrorMessage = "Id cannot be empty")]
    public required string Id { get; init; }

    [Required(ErrorMessage = "PartnerId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "PartnerId must be greater than 0")]
    public int PartnerId { get; init; }
    
    [Required(ErrorMessage = "Key is required")]
    [MinLength(1, ErrorMessage = "Key cannot be empty")]
    public required string Key { get; init; }
}