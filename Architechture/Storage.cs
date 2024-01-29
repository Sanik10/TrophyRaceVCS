using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Storage {
    private string filePath;
    private BinaryFormatter formatter;

    public Storage() {
        var directory = Application.persistentDataPath + "/saves";
        if(!Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }
        filePath = directory + "/GameSave.save";
        InitBinaryFormatter();
    }

    private void InitBinaryFormatter() {
        formatter = new BinaryFormatter();
    }

    public object Load(object saveDataByDefault) {
        if(!File.Exists(filePath)) {
            if(saveDataByDefault != null) {
                Save(saveDataByDefault);
            }
            return saveDataByDefault;
        }

        var file = File.Open(filePath, FileMode.Open);
        var savedData = formatter.Deserialize(file);
        file.Close();
        return savedData;
    }

    public void Save(object saveData) {
        var file = File.Create(filePath);
        formatter.Serialize(file, saveData);
        file.Close();
    }
}