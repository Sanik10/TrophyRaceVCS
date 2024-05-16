using System;
using UnityEngine;

[CreateAssetMenu(fileName = "VehicleData", menuName = "Vehicle Data/Vehicle Config File", order = 1)]
public class VehicleData : ScriptableObject, ISaveable {

    public static event Action onDataLoadedEvent;

    [SerializeField]
    private bool _includeVehicleInGame;
    public bool includeVehicleInGame => this._includeVehicleInGame;
    [SerializeField]
    private GameObject _Prefab;
    public GameObject Prefab => this._Prefab;
    [SerializeField]
    private string _vehicleName;
    public string vehicleName => this._vehicleName;

    [SerializeField]
    private int _id;
    // public int id => this._id;
    [SerializeField]
    private string _guid;
    public string guid => this._guid;
    [SerializeField]
    private int _price;
    public int price => this._price;

    [SerializeField]
    private bool _isOwned;
    public bool isOwned {
        get {return this._isOwned;}
        set {this._isOwned = value;}
    }

    private string _dataNodeName = "vehicle";
    public string dataNodeName => this._dataNodeName;

    #region Engine
    [Header("Двигатель")]
    [SerializeField]
    private int _maxPower = 105;
    [SerializeField]
    private int _idleRpm = 700;
    [SerializeField]
    private int _medRpm = 3000;
    [SerializeField]
    private int _maxRpm = 6000;
    [SerializeField] [Range(1, 100)]
    private int _maxPowerProcentAtIdleRpm = 50;
    [SerializeField] [Range(1, 100)]
    private int _maxPowerProcentAtMaxRpm = 20;
    [SerializeField] [Range(0.1f, 0.5f)]
    private float _engineInertia = 0.2f;
    [SerializeField] [Range(0.1f, 1)]
    private float _engineSmoothTime = 0.15f;

    public int maxPower {
        get {return this._maxPower;}
        set {this._maxPower = value;}
    }
    public int medRpm {
        get {return this._medRpm;}
        set {this._medRpm = value;}
    }
    public int idleRpm => this._idleRpm;
    public int maxRpm => this._maxRpm;
    public int maxPowerProcentAtIdleRpm {
        get {return this._maxPowerProcentAtIdleRpm;}
        set {this._maxPowerProcentAtIdleRpm = value;}
    }
    public int maxPowerProcentAtMaxRpm {
        get {return this._maxPowerProcentAtMaxRpm;}
        set {this._maxPowerProcentAtMaxRpm = value;}
    }
    public float engineInertia {
        get {return this._engineInertia;}
        set {this._engineInertia = value;}
    }
    public float engineSmoothTime {
        get {return this._engineSmoothTime;}
        set {this._engineSmoothTime = value;}
    }
    #endregion


    #region Transmission
    [Header("Трансмиссия")]
    [SerializeField]
    private transmissionType _transmission;
    [SerializeField]
    private int _frontGearsQuantity = 5;
    [SerializeField]
    private float _firstGear = 8f;
    [SerializeField] [Range(0.25f, 2f)]
    private float _shiftTime = 0.5f;
    [SerializeField]
    private int _rpmUpShift = 6000;
    [SerializeField]
    private int _rpmDownShift = 3000;
    private float _finalDrive = 1;

    public int frontGearsQuantity {
        get {return this._frontGearsQuantity;}
        set {this._frontGearsQuantity = value;}
    }
    public float firstGear {
        get {return this._firstGear;}
        set {this._firstGear = value;}
    }
    public float shiftTime {
        get {return this._shiftTime;}
        set {this._shiftTime = value;}
    }
    public float finalDrive {
        get {return this._finalDrive;}
        set {this._finalDrive = value;}
    }
    public transmissionType transmission => this._transmission;
    public int rpmUpShift => this._rpmUpShift;
    public int rpmDownShift => this._rpmDownShift;
    #endregion


    #region Wheels Settings
    [Header("Ходовая")]
    [SerializeField]
    private int _maxSpeed = 120;
    [SerializeField]
    private driveType _driveWheels;
    [SerializeField] [Range(0.1f, 1f)]
    private float _tireIntegrity = 1f;
    [SerializeField] [Range(500, 10000)]
    private int _brakingPowerVar = 2000;
    [SerializeField] [Range(0, 1)]
    private float _brakingDistribution = 0.5f;
    [SerializeField]
    private int _radius = 12;
    [SerializeField] [Range(1, 10)]
    private int _steeringWheelSpeed = 10;
    [SerializeField]
    private long _mileage = 0;

    public int maxSpeed {
        get {return this._maxSpeed;}
        set {this._maxSpeed = value;}
    }
    public float tireIntegrity {
        get {return this._tireIntegrity;}
        set {this._tireIntegrity = value;}
    }
    public int brakingPowerVar {
        get {return this._brakingPowerVar;}
        set {this._brakingPowerVar = value;}
    }
    public float brakingDistribution {
        get {return this._brakingDistribution;}
        set {this._brakingDistribution = value;}
    }
    public int radius {
        get {return this._radius;}
        set {this._radius = value;}
    }
    public int steeringWheelSpeed {
        get {return this._steeringWheelSpeed;}
        set {this._steeringWheelSpeed = value;}
    }
    public long mileage {
        get {return this._mileage;}
        set {this._mileage = value;}
    }
    public driveType driveWheels => this._driveWheels;
    #endregion


    #region General Info
    [Header("Цвета")]
    [SerializeField]
    private bool _colorCustomization;
    [SerializeField]
    private int _bodyMaterailId;
    [SerializeField]
    private int _diskMaterailId;


    public bool colorCustomization => _colorCustomization;

    public int bodyMaterailId {
        get {return this._bodyMaterailId;}
        set {this._bodyMaterailId = value;}
    }

    public int diskMaterailId {
        get {return this._diskMaterailId;}
        set {this._diskMaterailId = value;}
    }


    [Header("Другое")]
    [SerializeField]
    private int _mass;
    [SerializeField]
    private VehicleType _type;

    public VehicleType type => this._type;
    public int mass => this._mass;
    #endregion

    #region Vehicle range
    // !!! ПРИ ДОБАВЛЕНИИ НОВОГО АВТО СВЕРЯТЬ ЭТИ ПОКАЗАТЕЛИ, ЕСЛИ ОНИ БУДУТ РАЗНИТЬСЯ, ТО НАДО ИСПРАВИТЬ !!! //
    private static float maxStatPower = 2060;
    private static float minStatPower = 100;

    private static float maxMass = 8700;
    private static float minMass = 1975;

    private static float maxStatSpeed = 300;
    private static float minStatSpeed = 120;

    public float normalizedPower {
        get {
            return (this._maxPower - minStatPower) / (maxStatPower - minStatPower);
        }
    }

    public float normalizedPowerProcentIdle {
        get {
            return ((this._maxPowerProcentAtIdleRpm) - 1f) / 99f;
        }
    }

    public float normalizedPowerProcentMax {
        get {
            return ((this._maxPowerProcentAtMaxRpm) - 1f) / 99f;
        }
    }

    public float normalizedInertia {
        get {
            return (this._engineInertia - 0.1f) / 0.4f;
        }
    }

    public float normalizedShiftTime {
        get {
            return (this._shiftTime - 0.25f) / 1.75f;
        }
    }

    public float normalizedMaxSpeed {
        get {
            return (this._maxSpeed - minStatSpeed) / (maxStatSpeed - minStatSpeed);
        }
    }

    public float normalizedBrakeTorque {
        get {
            return (this._brakingPowerVar - 500) / 9500;
        }
    }

    public float normalizedTireIntegrity {
        get {
            return this._tireIntegrity;
        }
    }

    public float normalizedSteeringSpeed {
        get {
            return (this._steeringWheelSpeed - 1) / 9;
        }
    }

    public float normalizedMass {
        get {
            return 1f - (this._mass - minMass) / (maxMass - minMass);
        }
    }

    public float _range {
        get {
            return (
                normalizedPower * 1310 +
                normalizedPowerProcentIdle * 100 +
                normalizedPowerProcentMax * 150 +
                normalizedInertia * 200 +
                normalizedShiftTime * 500 +
                normalizedMaxSpeed * 1000 +
                normalizedBrakeTorque * 500 +
                normalizedTireIntegrity * 100 +
                normalizedSteeringSpeed * 500 +
                normalizedMass * 1190
            );
        }
    }

    public float range => this._range;
    #endregion

    public void Save(string sender) {
        SaveLoadManager.SaveToXml(this);
        Debug.Log("-------------   Vehicle data " + _id + " -- Saved!   ------------- " + sender);
    }

    public void Load() {
        #region Egine xml data
        LoadData("maxPower", ref _maxPower, int.TryParse);
        // LoadData("idleRpm", ref _idleRpm, int.TryParse);
        LoadData("medRpm", ref _medRpm, int.TryParse);
        // LoadData("maxRpm", ref _maxRpm, int.TryParse);
        // LoadData("additionRpmOnNeutral", ref _additionRpmOnNeutral, int.TryParse);
        
        LoadData("maxPowerProcentAtIdleRpm", ref _maxPowerProcentAtIdleRpm, int.TryParse);
        LoadData("maxPowerProcentAtMaxRpm", ref _maxPowerProcentAtMaxRpm, int.TryParse);
        LoadData("engineInertia", ref _engineInertia, float.TryParse);
        LoadData("engineSmoothTime", ref _engineSmoothTime, float.TryParse);
        #endregion

        #region Transmission xml data
        LoadData("frontGearsQuantity", ref _frontGearsQuantity, int.TryParse);
        LoadData("firstGear", ref _firstGear, float.TryParse);
        LoadData("shiftTime", ref _shiftTime, float.TryParse);
        LoadData("finalDrive", ref _finalDrive, float.TryParse);
        #endregion

        #region Vehicle Dynamics xml data
        LoadData("maxSpeed", ref _maxSpeed, int.TryParse);
        LoadData("tireIntegrity", ref _tireIntegrity, float.TryParse);
        LoadData("brakingPowerVar", ref _brakingPowerVar, int.TryParse);
        LoadData("brakingDistribution", ref _brakingDistribution, float.TryParse);
        LoadData("radius", ref _radius, int.TryParse);
        LoadData("steeringWheelSpeed", ref _steeringWheelSpeed, int.TryParse);
        #endregion

        #region Other
        LoadData("mileage", ref _mileage, long.TryParse);
        LoadData("bodyMaterialId", ref _bodyMaterailId, int.TryParse);
        LoadData("diskMaterialId", ref _diskMaterailId, int.TryParse);
        LoadData("isOwned", ref _isOwned, bool.TryParse);
        #endregion

        Debug.Log("-------------   Vehicle data " + _id + " -- Loaded!   -------------");
        onDataLoadedEvent?.Invoke();
    }

    private void LoadData<T>(string dataType, ref T field, TryParseDelegate<T> tryParser) {
        string decryptedData = SaveLoadManager.LoadFromXml<VehicleData>(this._dataNodeName, this._guid, dataType);
        if (decryptedData == null || decryptedData == string.Empty) {
            Debug.LogWarning($"Отсутствуют или некорректные данные для '{dataType}'. Загрузка остановлена.");
            return; // Остановка загрузки данных
        }

        if (tryParser(decryptedData, out T parsedValue)) {
            field = parsedValue; // Присваивание только в случае успешной загрузки и преобразования
        } else {
            Debug.LogWarning($"Ошибка при попытке преобразовать данные");
            // Можно выполнить дополнительные действия или прервать загрузку.
        }
    }

    private void OnValidate() {
        if(string.IsNullOrEmpty(this._guid)) {
            this._guid = Guid.NewGuid().ToString();
        }
    }

    delegate bool TryParseDelegate<T>(string s, out T result);
}

[System.Serializable]
public enum driveType {
    frontWeelsDrive, rearWheelsDrive, allWheelsDrive
}

[System.Serializable]
public enum VehicleType {
    Buggy, Prerunner, TrophyTruck, Truck, Motorcycle, Rally
}

[System.Serializable]
public enum transmissionType{
    Sequental, Manual, Automatic
}