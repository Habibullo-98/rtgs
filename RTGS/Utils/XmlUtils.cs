using System.Net;
using Serilog;
using System.Xml.Serialization;

namespace RTGS.Utils;

public abstract class XmlUtils
{
    public static string Serialize<T>(T obj)
    {
        Log.Debug("[XML UTILS] Serializing {Object}", obj);
        var serializer = new XmlSerializer(typeof(T));
        using var stringWriter = new StringWriter();
        using var xmlWriter = new System.Xml.XmlTextWriter(stringWriter);
        serializer.Serialize(xmlWriter, obj);
        return stringWriter.ToString();
    }
    
    public static T Deserialize<T>(string xmlContent)
    {
        Log.Debug("[XML UTILS] Deserializing {Content}", xmlContent);
        try
        {
            var decodedContent = WebUtility.HtmlDecode(xmlContent);
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StringReader(decodedContent);
            var result = serializer.Deserialize(reader);

            if (result == null)
                throw new InvalidOperationException($"Failed to deserialize XML into type {typeof(T).Name}.");

            return (T)result;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Failed to deserialize XML into type {typeof(T).Name} {e.Message}.", e);
        }
    }
}