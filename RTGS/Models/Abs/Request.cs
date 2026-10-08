namespace RTGS.Models.Abs;

public class Request
{
    public required int MessageId { get; set; }
    public required string DataType { get; set; }
    public required string Data { get; set; }
}