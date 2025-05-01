using System.Xml.Serialization;
using System.Xml;
using System.Text;

namespace Topicality.Client.Application;

public static class Utils
{
    //TODO pārbaudīt vai tiek izmantots šajā koroservisā
    /// <summary>
    /// Deserialize from string
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="xml"></param>
    /// <returns></returns>
    public static T? DeserializeFromString<T>(string xml)
    {
        var serializer = new XmlSerializer(typeof(T));
        using var sr = new StringReader(xml);
        using var xmlReader = XmlReader.Create(sr);
        var result =  (T?)serializer.Deserialize(xmlReader);
        return result;
    }

    /// <summary>
    /// Deserialize from File
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="xml"></param>
    /// <returns></returns>
    public static T? DeserializeXmlFromFile<T>(string path)
    {
        using var sr = new FileStream(path, FileMode.Open);
        using var xmlReader = XmlReader.Create(sr);
        var serializer = new XmlSerializer(typeof(T));
        return (T?)serializer.Deserialize(xmlReader);
    }

    /// <summary>
    /// Serialize object to xml (utf-8)
    /// </summary>
    /// <param name="dpiOecd"></param>
    /// <returns></returns>
    public static string Serialize<T>(T dpiOecd)
    {
        var xmlWriterSettings = new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = false,
            Encoding = Encoding.UTF8
        };

        using var memoryStream = new MemoryStream();
        using var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
        var xmlSerializer = new XmlSerializer(typeof(T));
        xmlSerializer.Serialize(memoryStream, dpiOecd);

        memoryStream.Position = 0;
        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }
}
