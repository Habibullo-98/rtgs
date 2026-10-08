using System.ComponentModel.DataAnnotations;

namespace RTGS.Models.DB;

public enum Status
{
    InProgress = 1,
    Success = 2,
    Failure = 3
}

public enum MessageType
{
    Wallet = 1,
    Card = 2
}

public class Transaction
{
    [Key]
    public int Id { get; set; }
    public DateTime CreatedDate { get; init; }
    public DateTime UpdatedDate { get; init; }
    
    [MaxLength(80)]
    public required string TransactionId { get; init; }
    
    public Status Status { get; set; }
    public bool Approved { get; init; }
    public required MessageType MessageType { get; init; }
    public required int PartnerId { get; init; }
}