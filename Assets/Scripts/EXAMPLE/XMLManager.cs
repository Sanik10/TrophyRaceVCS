using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;

public class XMLManager : MonoBehaviour
{
    private void Start()
    {
        // Создаем экземпляр класса MyData
        MyData data = new MyData();
        data.intValue = 10;
        data.stringValue = "Hello World";
        data.floatList = new List<float>() { 1.1f, 2.2f, 3.3f };

        // Сохраняем объект в XML-файл
        SaveData(data, "data.xml");

        // Загружаем объект из XML-файла
        MyData loadedData = LoadData("data.xml");

        // Выводим значения из загруженного объекта
        Debug.Log(loadedData.intValue);
        Debug.Log(loadedData.stringValue);
        foreach (float value in loadedData.floatList)
        {
            Debug.Log(value);
        }
    }

    private void SaveData(MyData data, string fileName)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(MyData));
        using (StreamWriter writer = new StreamWriter(fileName))
        {
            serializer.Serialize(writer, data);
        }
    }

    private MyData LoadData(string fileName)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(MyData));
        using (StreamReader reader = new StreamReader(fileName))
        {
            return (MyData)serializer.Deserialize(reader);
        }
    }
}