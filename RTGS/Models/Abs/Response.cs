namespace RTGS.Models.Abs;

public class Response
{
    public required string MsgId { get; set; }
    public required string Id { get; set; }
    public string? Type { get; set; }
    public required string Message { get; set; }
}