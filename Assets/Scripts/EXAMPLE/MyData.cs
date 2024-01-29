using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[Serializable]
public class MyData
{
    public int intValue;
    public string stringValue;

    // Этот атрибут указывает на то, что это поле должно быть сериализовано в XML
    [XmlElement("MyList")]
    public List<float> floatList;
}