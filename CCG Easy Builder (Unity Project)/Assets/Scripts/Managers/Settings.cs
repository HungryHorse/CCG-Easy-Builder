using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[System.Serializable]
public class SettingSerialization
{
    public int _normalCardPrefab;
    public int _creatureCardPrefab;
    public float _cardSize;

    public Vector2 _physicalHandSize;
    public bool _maxHandSizeOn;
    public int _maxHandSize;

    public Vector2 _physicalBoardSize;
    public float _boardSpacing;

    public bool _stackEnabled;
    public Vector2 _stackPosition;

    public int _startingHealths;
    public bool _maximumResourceOn;
    public int _maximumResource;

    public Vector2 _hoverPosition;

    public bool _seperateCombatPhase;

    public AttckStates _currentState;

    public SettingSerialization(int normalPrefab, int creaturePrefab, float cardSize, Vector2 physicalHandSize, bool maxHandSizeOn, int maxHandSize, Vector2 physicalBoardSize, float boardSpacing,
        bool stackEnabled, Vector2 stackPosition, int startingHealths, bool maximumResourceOn, int maximumResource, Vector2 hoverPosition, bool seperateCombatPhase, AttckStates currentState)
    {
        _normalCardPrefab = normalPrefab;
        _creatureCardPrefab = creaturePrefab;
        _cardSize = cardSize;
        _physicalHandSize = physicalHandSize;
        _maxHandSizeOn = maxHandSizeOn;
        _maxHandSize = maxHandSize;
        _physicalBoardSize = physicalBoardSize;
        _boardSpacing = boardSpacing;
        _stackEnabled = stackEnabled;
        _stackPosition = stackPosition;
        _startingHealths = startingHealths;
        _maximumResourceOn = maximumResourceOn;
        _maximumResource = maximumResource;
        _hoverPosition = hoverPosition;
        _seperateCombatPhase = seperateCombatPhase;
        _currentState = currentState;
    }
}

public class Settings : MonoBehaviour
{
    [SerializeField]
    private CardGameEvent[] _events;
    [SerializeField]
    private CardFrames _frames;
    [SerializeField]
    private Hand _handReferance;
    [SerializeField]
    private Board _boardReferance;
    [SerializeField]
    private GameManager _managerReferance;

    //Card settings
    private GameObject _normalCardPrefab;
    private GameObject _creatureCardPrefab;
    private float _cardSize;

    //Hand settings
    private Vector2 _physicalHandSize;
    private bool _maxHandSizeOn;
    private int _maxHandSize;

    //Board settings
    private Vector2 _physicalBoardSize;
    private float _boardSpacing;

    //Stack settings
    private bool _stackEnabled;
    private Vector2 _stackPosition;

    //Player settings
    private int _startingHealths;
    private bool _maximumResourceOn;
    private int _maximumResource;

    //Interactions with cards
    private Vector2 _hoverPosition;

    //Game settings
    private bool _seperateCombatPhase;

    //Attack State
    private AttckStates _currentState;

    public GameObject NormalCardPrefab { get => _normalCardPrefab; set => _normalCardPrefab = value; }
    public GameObject CreatureCardPrefab { get => _creatureCardPrefab; set => _creatureCardPrefab = value; }
    public float CardSize { get => _cardSize; set => _cardSize = value; }
    public Vector2 PhysicalHandSize { get => _physicalHandSize; set => _physicalHandSize = value; }
    public bool MaxHandSizeOn { get => _maxHandSizeOn; set => _maxHandSizeOn = value; }
    public int MaxHandSize { get => _maxHandSize; set => _maxHandSize = value; }
    public Vector2 PhysicalBoardSize { get => _physicalBoardSize; set => _physicalBoardSize = value; }
    public float BoardSpacing { get => _boardSpacing; set => _boardSpacing = value; }
    public bool StackEnabled { get => _stackEnabled; set => _stackEnabled = value; }
    public Vector2 StackPosition { get => _stackPosition; set => _stackPosition = value; }
    public int StartingHealths { get => _startingHealths; set => _startingHealths = value; }
    public bool MaximumResourceOn { get => _maximumResourceOn; set => _maximumResourceOn = value; }
    public int MaximumResource { get => _maximumResource; set => _maximumResource = value; }
    public Vector2 HoverPosition { get => _hoverPosition; set => _hoverPosition = value; }
    public bool SeperateCombatPhase { get => _seperateCombatPhase; set => _seperateCombatPhase = value; }
    public AttckStates CurrentState { get => _currentState; set => _currentState = value; }

    private void Awake()
    {
        LoadFromFile();
        UpdateSettings();
    }

    public void UpdateSettings()
    {
        _handReferance.CardPrefab = _normalCardPrefab;
        _handReferance.PhysicalHandSizeX = _physicalHandSize.x;
        _handReferance.PhysicalHandSizeY = _physicalHandSize.y;
        _boardReferance.PhysicalBoardSizeX = _physicalBoardSize.x;
        _boardReferance.PhysicalBoardSizeY = _physicalBoardSize.y;
        _boardReferance.Spacing = _boardSpacing;
        _managerReferance.StackEnabled = _stackEnabled;
        _managerReferance.PlayerHealth = _startingHealths;
        _managerReferance.OpponentHealth = _startingHealths;
        _managerReferance.StackPositionX = _stackPosition.x;
        _managerReferance.StackPositionY = _stackPosition.y;
        _managerReferance.HoverPositionX = _hoverPosition.x;
        _managerReferance.HoverPositionY = _hoverPosition.y;
        _managerReferance.CardSize = _cardSize;
        _managerReferance.SeperateCombatPhase = _seperateCombatPhase;
        _managerReferance.maxPlayerResource = _maximumResource;
        _managerReferance.maxPlayerResourceEnabled = _maximumResourceOn;
        _managerReferance.CurrentState = _currentState;

        _handReferance.Frames = _frames;
        _managerReferance.Events = _events;
    }

    public void SaveSettings()
    {
        _handReferance.CardPrefab = _normalCardPrefab;
        _handReferance.PhysicalHandSizeX = _physicalHandSize.x;
        _handReferance.PhysicalHandSizeY = _physicalHandSize.y;
        _boardReferance.PhysicalBoardSizeX = _physicalBoardSize.x;
        _boardReferance.PhysicalBoardSizeY = _physicalBoardSize.y;
        _boardReferance.Spacing = _boardSpacing;
        _managerReferance.StackEnabled = _stackEnabled;
        _managerReferance.PlayerHealth = _startingHealths;
        _managerReferance.OpponentHealth = _startingHealths;
        _managerReferance.StackPositionX = _stackPosition.x;
        _managerReferance.StackPositionY = _stackPosition.y;
        _managerReferance.HoverPositionX = _hoverPosition.x;
        _managerReferance.HoverPositionY = _hoverPosition.y;
        _managerReferance.CardSize = _cardSize;
        _managerReferance.SeperateCombatPhase = _seperateCombatPhase;
        _managerReferance.maxPlayerResource = _maximumResource;
        _managerReferance.maxPlayerResourceEnabled = _maximumResourceOn;
        _managerReferance.CurrentState = _currentState;

        _handReferance.Frames = _frames;
        _managerReferance.Events = _events;

        SaveToFile();
    }

    public void SaveToFile()
    {
        SettingSerialization newSettingsSave = new SettingSerialization(_normalCardPrefab.GetInstanceID(), _creatureCardPrefab.GetInstanceID(), _cardSize, _physicalHandSize, _maxHandSizeOn, _maxHandSize, _physicalBoardSize, _boardSpacing,
            _stackEnabled, _stackPosition, _startingHealths, _maximumResourceOn, _maximumResource, _hoverPosition, _seperateCombatPhase, _currentState);

        string jsonSettings = JsonUtility.ToJson(newSettingsSave);

        string _fileName = "SavedSettings.settings";

        File.Open(_fileName, FileMode.OpenOrCreate, FileAccess.Write).Dispose();
        File.WriteAllText(_fileName, jsonSettings);
    }

    public void LoadFromFile()
    {
        string _fileName = "SavedSettings.settings";

        File.Open(_fileName, FileMode.OpenOrCreate, FileAccess.Read).Dispose();

        string jsonSettings = File.ReadAllText(_fileName);

        SettingSerialization settingSerialization = null;

        try
        {
            settingSerialization = JsonUtility.FromJson<SettingSerialization>(jsonSettings);
            _normalCardPrefab = (GameObject)EditorUtility.InstanceIDToObject(settingSerialization._normalCardPrefab);
            _creatureCardPrefab = (GameObject)EditorUtility.InstanceIDToObject(settingSerialization._creatureCardPrefab);
            _physicalHandSize = settingSerialization._physicalHandSize;
            _physicalBoardSize = settingSerialization._physicalBoardSize;
            _boardSpacing = settingSerialization._boardSpacing;
            _stackEnabled = settingSerialization._stackEnabled;
            _startingHealths = settingSerialization._startingHealths;
            _stackPosition = settingSerialization._stackPosition;
            _hoverPosition = settingSerialization._hoverPosition;
            _cardSize = settingSerialization._cardSize;
            _seperateCombatPhase = settingSerialization._seperateCombatPhase;
            _maximumResource = settingSerialization._maximumResource;
            _maximumResourceOn = settingSerialization._maximumResourceOn;
            _currentState = settingSerialization._currentState;

            UpdateSettings();
        }
        catch
        {
            Debug.LogError("Settings file load failed");
            return;
        }

        
    }
}
