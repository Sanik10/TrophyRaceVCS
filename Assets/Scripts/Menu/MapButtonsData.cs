using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapButtonsData : MonoBehaviour {

    public static event Action MapButtonsDataLoadedEvent;

    [SerializeField]
    private int _selctedEvent;
    public int selectedEvent {
        get { return this._selctedEvent; }
        set { this._selctedEvent = value; }
    }

    [SerializeField]
    private List<MapButtonItem> _mapButtonsList = new List<MapButtonItem>();
    public List<MapButtonItem> mapButtonsList => this._mapButtonsList;

    private void Start() {
        LoadMapButtonsData();
    }

    void LoadMapButtonsData() {
        for(int i = 0; i < _mapButtonsList.Count; i++) {
            LoadData(_mapButtonsList[i]);
        }

        OnMapButtonsDataLoaded();
    }

    void OnMapButtonsDataLoaded() {
        // Логика или действия, которые необходимо выполнить после загрузки данных
        MapButtonsDataLoadedEvent?.Invoke();
    }

    private void LoadData(MapButtonItem button) {
        int loadedDataInt;
        bool loadedDataBool;
        LoadData(button.dataNodeName, "openedAwards", button.id, out loadedDataInt, int.Parse);
        button.openedAwards = loadedDataInt;
        LoadData(button.dataNodeName, "openedMap", button.id, out loadedDataBool, bool.Parse);
        button.openedMap = loadedDataBool;
        // LoadData("mapName", out button.mapName, s => s);
        // Продолжайте так же для каждого поля, которое необходимо загрузить
    }

    private void LoadData<T>(string dataNodeName, string dataType, int id, out T field, Func<string, T> tryParser) {
        string decryptedData = SaveLoadManager.LoadFromXml<MapButtonItem>(dataNodeName, id, dataType);
        if (!string.IsNullOrEmpty(decryptedData)) {
            field = tryParser(decryptedData);
        } else {
            Debug.LogWarning($"Отсутствуют данные для '{dataType}'.");
            field = default(T);
        }
    }

    // delegate bool TryParseDelegate<T>(string s, out T result);


    public void Save() {
        foreach (var button in mapButtonsList) {
            SaveLoadManager.SaveToXml(button);
        }
    }

    public void SaveCurrintButton(int id) {
        SaveLoadManager.SaveToXml(mapButtonsList[id]);
    }

    private void OnDestroy() {
        Save();
    }
}

[System.Serializable]
public class MapButtonItem : ISaveable {
    public string nameForNavigation;

    private string _dataNodeName = "button";

    [SerializeField]
    private int _id;

    [SerializeField]
    private Sprite _sprite;

    [SerializeField]
    private mapNameToStart _mapToStart;

    [SerializeField]
    private string _mapName;

    [SerializeField] [Range(0, 5)]
    private int _openedAwards;

    [SerializeField]
    private bool _openedMap = false;

    [SerializeField]
    private int[] _allowedVehiclesId;



    public int id {
        get {
            return this._id;
        }
    }
    public string dataNodeName {
        get {
            return this._dataNodeName;
        }
    }
    public Sprite sprite => this._sprite;
    public mapNameToStart mapToStart => this._mapToStart;
    public string mapName => this._mapName;
    public int openedAwards {
        get {return this._openedAwards;}
        set {this._openedAwards = value;}
    }
    public bool openedMap {
        get {return this._openedMap;}
        set {this._openedMap = value;}
    }
    public int[] allowedVehiclesId => this._allowedVehiclesId;
}

[System.Serializable]
public enum mapNameToStart {
    Desert, Forest, Sybir, DriftTrack, Desert_test
}