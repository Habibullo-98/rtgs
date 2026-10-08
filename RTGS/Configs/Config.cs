namespace RTGS.Configs;

public class Config(IConfiguration config)
{
    public readonly string InputDir = config["InputDir"] ?? throw new ArgumentNullException();
    public readonly string InputHistoryDir = config["InputHistoryDir"] ?? throw new ArgumentNullException();
    public readonly string OutputDir = config["OUtputDir"] ?? throw new ArgumentNullException();
    public readonly string OutputHistoryDir = config["OutputHistoryDir"] ?? throw new ArgumentNullException();
    public readonly string TransitDir = config["TransitDir"] ?? throw new ArgumentNullException();
    
    public readonly string AbsUrl = config["AbsUrl"] ?? throw new ArgumentNullException();
    public readonly string AbsTransferUrl = config["AbsTransferUrl"] ?? throw new ArgumentNullException();
    public readonly string AbsAuthKey = config["AbsAuthKey"] ?? throw new ArgumentNullException();
}