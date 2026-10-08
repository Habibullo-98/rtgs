using System.ComponentModel.DataAnnotations;

namespace RTGS.Models.DB;

public class Partner
{
    [Key]
    public required int Id { get; init; }
    
    [MaxLength(50)]
    public required string Name { get; init; }

    [MaxLength(15)]
    public required string Ip { get; init; }
    
    [MaxLength(50)]
    public required string Key { get; init; }

    [MaxLength(200)]
    public required string CallBackUrl { get; init; }
}