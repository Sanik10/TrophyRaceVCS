using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Security.Cryptography;

public static class SaveLoadManager {

    private static string key = "3f8a2b6e9d5c7a1f4e2d8b6a4c9e7f2a";
    private static string iv = "2d8b6a4c9e7f2abe";

    public static void SaveToXml<T>(T data) where T : ISaveable { 
        string filePath = Application.dataPath + $"/SavedData/{typeof(T).Name}.xml";

        XmlDocument xmlDoc = new XmlDocument();

        if(File.Exists(filePath)) {
            xmlDoc.Load(filePath);
        } else {
            XmlElement rootElement = xmlDoc.CreateElement(typeof(T).Name);
            xmlDoc.AppendChild(rootElement);
        }

        string encryptedId = EncryptElementId(data.id).ToString();

        // Поиск узла данных по зашифрованному идентификатору
        XmlNode dataNode = xmlDoc.SelectSingleNode($"//{data.dataNodeName}[@id='{encryptedId}']");
        if(dataNode == null) {
            // Создание нового узла данных, если не найден
            XmlElement rootElement = xmlDoc.DocumentElement;
            XmlElement dataElement = xmlDoc.CreateElement(data.dataNodeName);
            dataElement.SetAttribute("id", encryptedId);
            rootElement.AppendChild(dataElement);
            dataNode = dataElement;
        }

        // Пример обработки свойств для VehicleData (можно использовать switch для других типов)
        if(typeof(T) == typeof(VehicleData)) {
            // Преобразуйте объект в VehicleData для доступа к его свойствам
            VehicleData vehicleData = data as VehicleData;

            // Пример обработки свойств VehicleData
            #region Engine
            UpdateOrCreateNode(xmlDoc, dataNode, "maxPower", vehicleData.maxPower.ToString());
            UpdateOrCreateNode(xmlDoc, dataNode, "idleRpm", vehicleData.idleRpm.ToString());
            UpdateOrCreateNode(xmlDoc, dataNode, "medRpm", vehicleData.medRpm.ToString());
            UpdateOrCreateNode(xmlDoc, dataNode, "maxRpm", vehicleData.maxRpm.ToString());
            UpdateOrCreateNode(xmlDoc, dataNode, "additionRpmOnNeutral", vehicleData.additionRpmOnNeutral.ToString());

            UpdateOrCreateNode(xmlDoc, dataNode, "maxPowerProcentAtIdleRpm", vehicleData.maxPowerProcentAtIdleRpm.ToString());
            UpdateOrCreateNode(xmlDoc, dataNode, "maxPowerProcentAtMaxRpm", vehicleData.maxPowerProcentAtMaxRpm.ToString());
            UpdateOrCreateNode(xmlDoc, dataNode, "engineInertia", vehicleData.engineInertia.ToString());
            UpdateOrCreateNode(xmlDoc, dataNode, "engineSmoothTime", vehicleData.engineSmoothTime.ToString());
            #endregion

            #region Transmission
            UpdateOrCreateNode(xmlDoc, dataNode, "frontGearsQuantity", vehicleData.frontGearsQuantity.ToString());
            UpdateOrCreateNode(xmlDoc, dataNode, "firstGear", vehicleData.firstGear.ToString());
            UpdateOrCreateNode(xmlDoc, dataNode, "shiftTime", vehicleData.shiftTime.ToString());
            UpdateOrCreateNode(xmlDoc, dataNode, "finalDrive", vehicleData.finalDrive.ToString());
            #endregion

            #region Wheels settings
            UpdateOrCreateNode(xmlDoc, dataNode, "maxSpeed", vehicleData.maxSpeed.ToString());
            UpdateOrCreateNode(xmlDoc, dataNode, "tireIntegrity", vehicleData.tireIntegrity.ToString());
            UpdateOrCreateNode(xmlDoc, dataNode, "brakingPowerVar", vehicleData.brakingPowerVar.ToString());
            UpdateOrCreateNode(xmlDoc, dataNode, "brakingDistribution", vehicleData.brakingDistribution.ToString());
            UpdateOrCreateNode(xmlDoc, dataNode, "radius", vehicleData.radius.ToString());
            UpdateOrCreateNode(xmlDoc, dataNode, "steeringWheelSpeed", vehicleData.steeringWheelSpeed.ToString());
            #endregion

            #region Other
            UpdateOrCreateNode(xmlDoc, dataNode, "mileage", vehicleData.mileage.ToString());
            UpdateOrCreateNode(xmlDoc, dataNode, "bodyMaterialId", vehicleData.bodyMaterailId.ToString());
            UpdateOrCreateNode(xmlDoc, dataNode, "diskMaterialId", vehicleData.diskMaterailId.ToString());
            UpdateOrCreateNode(xmlDoc, dataNode, "isOwned", vehicleData.isOwned.ToString());
            #endregion

        } else if(typeof(T) == typeof(MapButtonItem)) {
            MapButtonItem mapButton = data as MapButtonItem;

            // Добавьте свойства кнопки в XML
            UpdateOrCreateNode(xmlDoc, dataNode, "openedAwards", mapButton.openedAwards.ToString());
            UpdateOrCreateNode(xmlDoc, dataNode, "openedMap", mapButton.openedMap.ToString());
            // Добавьте другие свойства кнопки...

        }

        xmlDoc.Save(filePath);
    }

    private static void UpdateOrCreateNode(XmlDocument xmlDoc, XmlNode parentNode, string nodeName, string nodeValue) {
        XmlNode node = parentNode.SelectSingleNode(nodeName);
        if(node == null) {
            node = xmlDoc.CreateElement(nodeName);
            parentNode.AppendChild(node);
        }
        node.InnerText = Encrypt(nodeValue);
    }

    private static string Encrypt(string plainText) {
        byte[] encrypted;
        using (AesManaged aes = new AesManaged()) {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream()) {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {
                    using (StreamWriter sw = new StreamWriter(cs)) {
                        sw.Write(plainText);
                    }
                    encrypted = ms.ToArray();
                }
            }
        }
        return Convert.ToBase64String(encrypted);
    }

    public static string LoadFromXml<T>(string dataNodeName, int elementId, string nodeToRead) where T : ISaveable {
        string encryptedData = ReadEncryptedDataFromXml<T>(dataNodeName, elementId, nodeToRead);
        string decryptedData = Decrypt(encryptedData);
        return decryptedData;
    }

    private static string ReadEncryptedDataFromXml<T>(string dataNodeName, int elementId, string nodeToRead) where T : ISaveable {
        string filePath = Application.dataPath + $"/SavedData/{typeof(T).Name}.xml";
        string encryptedId = EncryptElementId(elementId).ToString();

        XmlDocument xmlDoc = new XmlDocument();
        if (File.Exists(filePath)) {
            xmlDoc.Load(filePath);
        } else {
            XmlElement rootElement = xmlDoc.CreateElement(typeof(T).Name);
            xmlDoc.AppendChild(rootElement);
        }

        XmlNode dataNode = xmlDoc.SelectSingleNode($"//{dataNodeName}[@id='{encryptedId}']");
        if (dataNode != null) {
            XmlNode encryptedDataNode = dataNode.SelectSingleNode($"./{nodeToRead}");
            if (encryptedDataNode != null) {
                return encryptedDataNode.InnerText;
            } else {
                // Node with data is missing, create an empty node
                UpdateOrCreateNode(xmlDoc, dataNode, nodeToRead, string.Empty);
                xmlDoc.Save(filePath);

                Debug.LogWarning($"Узел '{nodeToRead}' для этого элемента отсутствует. Аварийное создание узла.");
                return string.Empty;
            }
        } else {
            // Data node is missing, create a new node with empty data node
            XmlElement root = xmlDoc.DocumentElement;
            XmlElement dataElement = xmlDoc.CreateElement(dataNodeName);
            dataElement.SetAttribute("id", encryptedId);
            root.AppendChild(dataElement);

            UpdateOrCreateNode(xmlDoc, dataElement, nodeToRead, string.Empty);
            xmlDoc.Save(filePath);

            Debug.LogWarning($"Узел с данными для этого элемента не найден! Запуск аварийного создания данного узла.");

            return string.Empty;
        }
    }

    private static string Decrypt(string cipherText) {
        if (string.IsNullOrEmpty(cipherText)) {
            return string.Empty; // Возвращаем пустую строку, если входной текст пустой или null
        }

        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        string decrypted;

        using (AesManaged aes = new AesManaged()) {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream(cipherBytes)) {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read)) {
                    using (StreamReader sr = new StreamReader(cs)) {
                        decrypted = sr.ReadToEnd();
                    }
                }
            }
        }
        return decrypted;
    }

    private static int EncryptElementId(int vehicleId) {
        byte[] encrypted;
        using (AesManaged aes = new AesManaged()) {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream()) {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {
                    using (StreamWriter sw = new StreamWriter(cs)) {
                        sw.Write(vehicleId);
                    }
                    encrypted = ms.ToArray();
                }
            }
        }
        uint unsignedResult = BitConverter.ToUInt32(encrypted, 0);
        int result = (int)(unsignedResult & int.MaxValue);
    
        return result;
    }

    public static string GenerateKey() {
        const int keyLength = 16; // Длина ключа в байтах (32 байта = 256 бит)
        // Создание объекта для генерации случайных чисел
        using (var rng = new RNGCryptoServiceProvider()) {
            // Массив для хранения сгенерированного ключа
            byte[] keyBytes = new byte[keyLength];
            
            // Генерация случайных байтов для ключа
            rng.GetBytes(keyBytes);
            
            // Преобразование байтов в строку в шестнадцатеричном формате
            return BitConverter.ToString(keyBytes).Replace("-", "").ToLower();
        }
    }
}