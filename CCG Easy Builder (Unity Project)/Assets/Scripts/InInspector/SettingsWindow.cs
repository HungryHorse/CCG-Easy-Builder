using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class CardFrameReferenceHolder : ScriptableObject
{
    public CardFrames[] frames;
}

public class SettingsWindow : EditorWindow
{
    private static GUIStyle _editorStyle = new GUIStyle(GUI.skin.textField);
    private static GUIStyle _textEditorStyle = new GUIStyle(GUI.skin.textField);
    private static GUIStyle _popUpStyle = new GUIStyle(GUI.skin.GetStyle("popup"));
    private static GUIStyle _mainPopUpStyle = new GUIStyle(GUI.skin.GetStyle("popup"));
    private static GUIStyle _buttonStyle = new GUIStyle(GUI.skin.GetStyle("miniButton"));

    private static CardFrameReferenceHolder _cardFrameReferenceHolder = ScriptableObject.CreateInstance<CardFrameReferenceHolder>();
    private static SerializedObject _serializedCardFrameHolder;
    private static SerializedProperty _serializedCardFrameHolderList;

    //Hand settings
    private Vector2 _physicalHandSize;
    private CardFrames[] _cardFrames;

    public static void ShowWindow()
    {
        _popUpStyle.margin = new RectOffset(15, 15, 5, 5);
        _editorStyle.margin = new RectOffset(15, 15, 5, 5);
        _mainPopUpStyle.margin = new RectOffset(0, 15, 5, 5);
        _textEditorStyle.margin = new RectOffset(0, 15, 5, 15); 
        _buttonStyle.margin = new RectOffset(15, 15, 20, 0);

        _serializedCardFrameHolder = new UnityEditor.SerializedObject(_cardFrameReferenceHolder);
        _serializedCardFrameHolderList = _serializedCardFrameHolder.FindProperty("frames");

        SettingsWindow window = (SettingsWindow)SettingsWindow.GetWindow(typeof(SettingsWindow), false, "Settings Menu");
        window.minSize = new Vector2(1000, 600);
        window.maxSize = new Vector2(2000, 1200);
        window.position = new Rect(new Vector2(Screen.width / 1.2f, Screen.height / 3f), new Vector2(600, 360));
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField(" ");
        EditorGUILayout.LabelField("Hand Settings", EditorStyles.boldLabel);
        EditorGUILayout.Vector2Field("Physical Hand Size", _physicalHandSize);

        EditorGUILayout.PropertyField(_serializedCardFrameHolderList, true);
        _cardFrames = new CardFrames[_serializedCardFrameHolderList.arraySize];
        for (int i = 0; i < _serializedCardFrameHolderList.arraySize; i++)
        {
            SerializedProperty property = _serializedCardFrameHolderList.GetArrayElementAtIndex(i);
            _cardFrames[i] = (CardFrames)property.objectReferenceValue;
        }
    }
}
