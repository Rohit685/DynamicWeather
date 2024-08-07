using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Rage;

namespace DynamicWeather.IO;

internal class XMLParser<E>
{
    internal string FilePath { get; private set; }
    private XmlSerializer Serializer { get; set; }
    private XmlSerializerNamespaces Namespaces { get; set; }
    private XmlWriterSettings Settings { get; set; }
    
    internal XMLParser(string filePath)
    {
        FilePath = filePath;
        Serializer = new XmlSerializer(typeof(E)); 
        Namespaces = new XmlSerializerNamespaces();
        Settings = new XmlWriterSettings()
        {
            Indent = true,
            OmitXmlDeclaration = true
        };
        Namespaces.Add("", "");
    }

    internal void SerializeXML(E data)
    {
        using XmlWriter write = XmlWriter.Create(FilePath, Settings);
        Serializer.Serialize(write, data);
    }

    internal E DeserializeXML()
    {
        E xmlObject = default;
        using FileStream fs = new(FilePath, FileMode.Open, FileAccess.Read);
        try
        {
            xmlObject = (E)Serializer.Deserialize(fs);
        }
        catch (Exception e)
        {
            Game.LogTrivial($"Error deserializing XML File: {FilePath}");
            Game.LogTrivial(e.ToString());
        }
        return xmlObject;
    }

    internal bool DoesFileExist()
    {
        return File.Exists(FilePath);
    }

    internal void DeleteFile()
    {
        if(DoesFileExist())
        {
            File.Delete(FilePath);
        }
    }
}