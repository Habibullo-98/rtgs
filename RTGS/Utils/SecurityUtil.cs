using System.Security.Cryptography;
using System.Text;

namespace RTGS.Utils;

public class SecurityUtil
{
    public static string GetMD5Hash(string input)
    {
        using var md5 = MD5.Create();
        var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        var result = new StringBuilder();
        foreach (var b in bytes)
        {
            result.Append(b.ToString("x2"));
        }
        return result.ToString();
    }
}