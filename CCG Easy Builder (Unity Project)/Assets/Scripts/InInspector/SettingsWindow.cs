using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

public static class PropertyExtension
{
        // --------------------------------------------------------------------------------------------------------------------
         // <author>
         //   HiddenMonk
         //   http://answers.unity3d.com/users/496850/hiddenmonk.html
         //   
         //   Johannes Deml
         //   send@johannesdeml.com
         // </author>
         // -------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Extension class for SerializedProperties
        /// See also: http://answers.unity3d.com/questions/627090/convert-serializedproperty-to-custom-class.html
        /// </summary>
        /// <summary>
        /// Get the object the serialized property holds by using reflection
        /// </summary>
        /// <typeparam name="T">The object type that the property contains</typeparam>
        /// <param name="property"></param>
        /// <returns>Returns the object type T if it is the type the property actually contains</returns>
        public static T GetValue<T>(this SerializedProperty property)
        {
            return GetNestedObject<T>(property.propertyPath, GetSerializedPropertyRootComponent(property));
        }

        /// <summary>
        /// Set the value of a field of the property with the type T
        /// </summary>
        /// <typeparam name="T">The type of the field that is set</typeparam>
        /// <param name="property">The serialized property that should be set</param>
        /// <param name="value">The new value for the specified property</param>
        /// <returns>Returns if the operation was successful or failed</returns>
        public static bool SetValue<T>(this SerializedProperty property, T value)
        {

            object obj = GetSerializedPropertyRootComponent(property);
            //Iterate to parent object of the value, necessary if it is a nested object
            string[] fieldStructure = property.propertyPath.Split('.');
            for (int i = 0; i < fieldStructure.Length - 1; i++)
            {
                obj = GetFieldOrPropertyValue<object>(fieldStructure[i], obj);
            }
            string fieldName = fieldStructure.Last();

            return SetFieldOrPropertyValue(fieldName, obj, value);

        }

        /// <summary>
        /// Get the component of a serialized property
        /// </summary>
        /// <param name="property">The property that is part of the component</param>
        /// <returns>The root component of the property</returns>
        public static object GetSerializedPropertyRootComponent(SerializedProperty property)
        {
            return (object)property.serializedObject.targetObject;
        }
        
        /// <summary>
        /// Iterates through objects to handle objects that are nested in the root object
        /// </summary>
        /// <typeparam name="T">The type of the nested object</typeparam>
        /// <param name="path">Path to the object through other properties e.g. PlayerInformation.Health</param>
        /// <param name="obj">The root object from which this path leads to the property</param>
        /// <param name="includeAllBases">Include base classes and interfaces as well</param>
        /// <returns>Returns the nested object casted to the type T</returns>
        public static T GetNestedObject<T>(string path, object obj, bool includeAllBases = false)
        {
            foreach (string part in path.Split('.'))
            {
                obj = GetFieldOrPropertyValue<object>(part, obj, includeAllBases);
            }
            return (T)obj;
        }

        public static T GetFieldOrPropertyValue<T>(string fieldName, object obj, bool includeAllBases = false, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            FieldInfo field = obj.GetType().GetField(fieldName, bindings);
            if (field != null) return (T)field.GetValue(obj);

            PropertyInfo property = obj.GetType().GetProperty(fieldName, bindings);
            if (property != null) return (T)property.GetValue(obj, null);

            if (includeAllBases)
            {

                foreach (Type type in GetBaseClassesAndInterfaces(obj.GetType()))
                {
                    field = type.GetField(fieldName, bindings);
                    if (field != null) return (T)field.GetValue(obj);

                    property = type.GetProperty(fieldName, bindings);
                    if (property != null) return (T)property.GetValue(obj, null);
                }
            }

            return default(T);
        }

        public static bool SetFieldOrPropertyValue(string fieldName, object obj, object value, bool includeAllBases = false, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            FieldInfo field = obj.GetType().GetField(fieldName, bindings);
            if (field != null)
            {
                field.SetValue(obj, value);
                return true;
            }

            PropertyInfo property = obj.GetType().GetProperty(fieldName, bindings);
            if (property != null)
            {
                property.SetValue(obj, value, null);
                return true;
            }

            if (includeAllBases)
            {
                foreach (Type type in GetBaseClassesAndInterfaces(obj.GetType()))
                {
                    field = type.GetField(fieldName, bindings);
                    if (field != null)
                    {
                        field.SetValue(obj, value);
                        return true;
                    }

                    property = type.GetProperty(fieldName, bindings);
                    if (property != null)
                    {
                        property.SetValue(obj, value, null);
                        return true;
                    }
                }
            }
            return false;
        }

        public static IEnumerable<Type> GetBaseClassesAndInterfaces(this Type type, bool includeSelf = false)
        {
            List<Type> allTypes = new List<Type>();

            if (includeSelf) allTypes.Add(type);

            if (type.BaseType == typeof(object))
            {
                allTypes.AddRange(type.GetInterfaces());
            }
            else
            {
                allTypes.AddRange(
                        Enumerable
                        .Repeat(type.BaseType, 1)
                        .Concat(type.GetInterfaces())
                        .Concat(type.BaseType.GetBaseClassesAndInterfaces())
                        .Distinct());
            }

            return allTypes;
        }
}

public class SettingsWindow : EditorWindow
{
    private static GUIStyle _editorStyle = new GUIStyle(GUI.skin.textField);
    private static GUIStyle _textEditorStyle = new GUIStyle(GUI.skin.textField);
    private static GUIStyle _popUpStyle = new GUIStyle(GUI.skin.GetStyle("popup"));
    private static GUIStyle _mainPopUpStyle = new GUIStyle(GUI.skin.GetStyle("popup"));
    private static GUIStyle _buttonStyle = new GUIStyle(GUI.skin.GetStyle("miniButton"));

    //Card settings
    private static GameObject _normalCardPrefab;
    private static GameObject _creatureCardPrefab;
    private static float _cardSize;

    //Hand settings
    private static Vector2 _physicalHandSize;
    private static bool _maxHandSizeOn;
    private static int _maxHandSize;

    //Board settings
    private static Vector2 _physicalBoardSize;
    private static float _boardSpacing;

    //Stack settings
    private static bool _stackEnabled;
    private static Vector2 _stackPosition;

    //Player settings
    private static int _startingHealths;
    private static bool _maximumResourceOn;
    private static int _maximumResource;

    //Interactions with cards
    private static Vector2 _hoverPosition;

    //Game settings
    private static bool _seperateCombatPhase;

    private static Settings _settingsInstance;

    //Attack State
    private AttckStates _currentState;

    public static void ShowWindow(Settings instanceOfSettings)
    {
        _popUpStyle.margin = new RectOffset(15, 15, 5, 5);
        _editorStyle.margin = new RectOffset(15, 15, 5, 5);
        _mainPopUpStyle.margin = new RectOffset(0, 15, 5, 5);
        _textEditorStyle.margin = new RectOffset(0, 15, 5, 15); 
        _buttonStyle.margin = new RectOffset(15, 15, 20, 0);

        _settingsInstance = instanceOfSettings;

        UpdateFields();

        SettingsWindow window = (SettingsWindow)SettingsWindow.GetWindow(typeof(SettingsWindow), false, "Settings Menu");
        window.minSize = new Vector2(1000, 700);
        window.maxSize = new Vector2(2000, 1400);
        window.position = new Rect(new Vector2(Screen.width / 1.2f, Screen.height / 3f), new Vector2(600, 360));
    }

    private void Awake()
    {
        UpdateFields();
    }

    private static void UpdateFields()
    {
        _settingsInstance.LoadFromFile();
        _normalCardPrefab = _settingsInstance.NormalCardPrefab;
        _creatureCardPrefab = _settingsInstance.CreatureCardPrefab;
        _cardSize = _settingsInstance.CardSize;
        _physicalBoardSize = _settingsInstance.PhysicalBoardSize;
        _physicalHandSize = _settingsInstance.PhysicalHandSize;
        _maxHandSize = _settingsInstance.MaxHandSize;
        _maxHandSizeOn = _settingsInstance.MaxHandSizeOn;
        _boardSpacing = _settingsInstance.BoardSpacing;
        _stackEnabled = _settingsInstance.StackEnabled;
        _stackPosition = _settingsInstance.StackPosition;
        _startingHealths = _settingsInstance.StartingHealths;
        _maximumResource = _settingsInstance.MaximumResource;
        _maximumResourceOn = _settingsInstance.MaximumResourceOn;
        _hoverPosition = _settingsInstance.HoverPosition;
        _seperateCombatPhase = _settingsInstance.SeperateCombatPhase;
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField(" ");
        EditorGUILayout.LabelField("Hand Settings", EditorStyles.boldLabel);
        _physicalHandSize = EditorGUILayout.Vector2Field("Physical Hand Size", _physicalHandSize);
        _maxHandSizeOn = EditorGUILayout.Toggle("Max Hand Size", _maxHandSizeOn);
        if(_maxHandSizeOn)
        {
            _maxHandSize = EditorGUILayout.IntSlider("Maximum hand size:", _maxHandSize, 1, 20);
        }

        EditorGUILayout.LabelField(" ");
        EditorGUILayout.LabelField("Board Settings", EditorStyles.boldLabel);
        _physicalBoardSize = EditorGUILayout.Vector2Field("Physical Board Size", _physicalBoardSize);
        _boardSpacing = EditorGUILayout.Slider("Board Spacing", _boardSpacing, 0, 0.1f);

        EditorGUILayout.LabelField(" ");
        EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
        _stackEnabled = EditorGUILayout.Toggle("Stack Enabled", _stackEnabled);
        _stackPosition = EditorGUILayout.Vector2Field("Stack Position", _stackPosition);
        _hoverPosition = EditorGUILayout.Vector2Field("Card being hovered Position", _hoverPosition);
        
        EditorGUILayout.LabelField(" ");
        EditorGUILayout.LabelField("Card Settings", EditorStyles.boldLabel);
        _normalCardPrefab = (GameObject)EditorGUILayout.ObjectField("Default Card Prefab", _normalCardPrefab, typeof(GameObject), false);
        _creatureCardPrefab = (GameObject)EditorGUILayout.ObjectField("Creature Card Prefab", _creatureCardPrefab, typeof(GameObject), false);
        _cardSize = EditorGUILayout.FloatField("Card Size", _cardSize);

        EditorGUILayout.LabelField(" ");
        EditorGUILayout.LabelField("Game Settings", EditorStyles.boldLabel);
        _seperateCombatPhase = EditorGUILayout.Toggle("Seperate Combat Phase", _seperateCombatPhase);
        _currentState = (AttckStates)EditorGUILayout.EnumPopup("Attacking state", _currentState, _popUpStyle);
        

        EditorGUILayout.LabelField(" ");
        EditorGUILayout.LabelField("Player Settings", EditorStyles.boldLabel);
        _startingHealths = EditorGUILayout.IntField("Starting Health", _startingHealths);
        _maximumResourceOn = EditorGUILayout.Toggle("Max Resource Enabled", _maximumResourceOn);
        if (_maximumResourceOn)
        {
            _maximumResource = EditorGUILayout.IntField("Maximum Resource:", _maximumResource);
        }

        if (GUILayout.Button("Save Settings", EditorStyles.miniButton))
        {
            SetOptions();
        }
    }

    void SetOptions()
    {
        _settingsInstance.NormalCardPrefab = _normalCardPrefab;
        _settingsInstance.CreatureCardPrefab = _creatureCardPrefab;
        _settingsInstance.CardSize = _cardSize;
        _settingsInstance.PhysicalHandSize = _physicalHandSize;
        _settingsInstance.MaxHandSizeOn = _maxHandSizeOn;
        _settingsInstance.MaxHandSize = _maxHandSize;
        _settingsInstance.PhysicalBoardSize = _physicalBoardSize;
        _settingsInstance.BoardSpacing = _boardSpacing;
        _settingsInstance.StackEnabled = _stackEnabled;
        _settingsInstance.StackPosition = _stackPosition;
        _settingsInstance.StartingHealths = _startingHealths;
        _settingsInstance.MaximumResourceOn = _maximumResourceOn;
        _settingsInstance.MaximumResource = _maximumResource;
        _settingsInstance.HoverPosition =  _hoverPosition;
        _settingsInstance.SeperateCombatPhase = _seperateCombatPhase;

        _settingsInstance.SaveSettings();
    }

}
