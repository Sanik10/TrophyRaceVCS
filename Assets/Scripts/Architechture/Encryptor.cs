// using System;
// using System.IO;
// using System.Security.Cryptography;
// using System.Text;
// using System.Xml;

// public static string Encrypt(string plainText, string key, string iv) {
//     byte[] encrypted;

//     using (AesManaged aes = new AesManaged()) {
//         aes.Key = Encoding.UTF8.GetBytes(key);
//         aes.IV = Encoding.UTF8.GetBytes(iv);

//         ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

//         using (MemoryStream ms = new MemoryStream()) {
//             using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {
//                 using (StreamWriter sw = new StreamWriter(cs)) {
//                     sw.Write(plainText);
//                 }
//                 encrypted = ms.ToArray();
//             }
//         }
//     }
//     return Convert.ToBase64String(encrypted);
// }

// public static string Decrypt(string cipherText, string key, string iv) {
//     byte[] cipherBytes = Convert.FromBase64String(cipherText);
//     string decrypted;

//     using (AesManaged aes = new AesManaged()) {
//         aes.Key = Encoding.UTF8.GetBytes(key);
//         aes.IV = Encoding.UTF8.GetBytes(iv);

//         ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

//         using (MemoryStream ms = new MemoryStream(cipherBytes)) {
//             using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read)) {
//                 using (StreamReader sr = new StreamReader(cs)) {
//                     decrypted = sr.ReadToEnd();
//                 }
//             }
//         }
//     }

//     return decrypted;
// }

// using UnityEngine;
// using System.Xml;

// public class CarEncryption : MonoBehaviour {
//     private static string key = "myEncryptionKey123";
//     private static string iv = "myEncryptionIV456";

//     void Start() {
//         // Загрузка XML файла
//         string filePath = Application.dataPath + "/cars.xml";
//         XmlDocument xmlDoc = new XmlDocument();
//         xmlDoc.Load(filePath);

//         // Получение списка всех элементов "car"
//         XmlNodeList carNodes = xmlDoc.SelectNodes("//car");

//         // Шифрование максимальной скорости каждого автомобиля
//         foreach (XmlNode carNode in carNodes) {
//             XmlNode maxSpeedNode = carNode.SelectSingleNode("maxSpeed");
//             string maxSpeed = maxSpeedNode.InnerText;

//             string encryptedMaxSpeed = Encrypt(maxSpeed, key, iv);
//             maxSpeedNode.InnerText = encryptedMaxSpeed;
//         }

//         // Сохранение зашифрованных данных обратно в XML файл
//         xmlDoc.Save(filePath);
//     }

//     private static string Encrypt(string plainText, string key, string iv) {
//         // Код шифрования (представлен в вашем вопросе)
//     }
// }

// void Start()
//     {
//         byte[] keyBytes = StringToByteArray(key);

//         string encryptedDataFilePath = "encrypted_data.xml";

//         // Чтение зашифрованных данных из XML-файла
//         string encryptedData = ReadEncryptedDataFromXml(encryptedDataFilePath);

//         // Расшифровка данных
//         string decryptedData = Decrypt(encryptedData, key, iv);

//         Debug.Log("Расшифрованные данные: " + decryptedData);
//     }

//     // Функция для чтения зашифрованных данных из XML-файла
//     private static string ReadEncryptedDataFromXml(string filePath)
//     {
//         XmlDocument xmlDoc = new XmlDocument();
//         xmlDoc.Load(filePath);

//         XmlNode encryptedDataNode = xmlDoc.SelectSingleNode("//EncryptedData");
//         string encryptedDataBase64 = encryptedDataNode.InnerText;

//         return encryptedDataBase64;
//     }

//     // Функция для расшифровки данных
//     public static string Decrypt(string cipherText, string key, string iv)
//     {
//         byte[] cipherBytes = Convert.FromBase64String(cipherText);
//         string decrypted;

//         using (AesManaged aes = new AesManaged())
//         {
//             aes.Key = Encoding.UTF8.GetBytes(key);
//             aes.IV = Encoding.UTF8.GetBytes(iv);

//             ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

//             using (MemoryStream ms = new MemoryStream(cipherBytes))
//             {
//                 using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
//                 {
//                     using (StreamReader sr = new StreamReader(cs))
//                     {
//                         decrypted = sr.ReadToEnd();
//                     }
//                 }
//             }
//         }
//         return decrypted;
//     }

//     // Вспомогательная функция для преобразования строки в массив байтов
//     private static byte[] StringToByteArray(string hex)
//     {
//         int numberChars = hex.Length;
//         byte[] bytes = new byte[numberChars / 2];
//         for (int i = 0; i < numberChars; i += 2)
//         {
//             bytes[i / 2] = System.Convert.ToByte(hex.Substring(i, 2), 16);
//         }
//         return bytes;
//     }