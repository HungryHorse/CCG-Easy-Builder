using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EffectCreationWindow : EditorWindow
{
    private Triggers _triggerForNewEffect;
    private static string[] _keyWordNames = null;
    private static Keyword[] _keywords = null;
    private int _index;
    private static CardCreator _cardCreationReferance;
    private bool _hasTarget;
    private Targets _target;
    private static GUIStyle _editorStyle = new GUIStyle(GUI.skin.textField);
    private static GUIStyle _textEditorStyle = new GUIStyle(GUI.skin.textField);
    private static GUIStyle _popUpStyle = new GUIStyle(GUI.skin.GetStyle("popup"));
    private static GUIStyle _mainPopUpStyle = new GUIStyle(GUI.skin.GetStyle("popup"));
    private static GUIStyle _buttonStyle = new GUIStyle(GUI.skin.GetStyle("miniButton"));
    private int _value;
    private string _effectName;

    public static void ShowWindow(Keyword[] importedKeywords, CardCreator cardCreationReferance)
    {
        _popUpStyle.margin = new RectOffset(15,15,5,5);
        _editorStyle.margin = new RectOffset(15,15,5,5);
        _mainPopUpStyle.margin = new RectOffset(0, 15, 5, 15);
        _textEditorStyle.margin = new RectOffset(0, 15, 5, 15);
        _buttonStyle.margin = new RectOffset(15,15,20,0);

        _cardCreationReferance = cardCreationReferance;
        _keywords = importedKeywords;
        _keyWordNames = new string[_keywords.Length];
        for(int i = 0; i < _keywords.Length; i++)
        {
            _keyWordNames[i] = _keywords[i].name;
        }
        //Show existing window instance. If one doesn't exist, make one.
        EffectCreationWindow window = (EffectCreationWindow)EditorWindow.GetWindow(typeof(EffectCreationWindow), false, _cardCreationReferance.CardName + " Effect Editor");
        window.minSize = new Vector2(500, 300);
        window.maxSize = new Vector2(1000, 600);
        window.position = new Rect(new Vector2(Screen.width / 1.2f, Screen.height / 3f), new Vector2(600,360));
    }

    private void OnGUI()
    {
        _effectName = EditorGUILayout.TextField("Name of effect", _effectName, _textEditorStyle);

        _triggerForNewEffect = (Triggers)EditorGUILayout.EnumPopup("Trigger for effect", _triggerForNewEffect, _mainPopUpStyle);

        if (_triggerForNewEffect != Triggers.Null)
        {
            _hasTarget = EditorGUILayout.Toggle("Has target other than self", _hasTarget);
            _index = EditorGUILayout.Popup("Keyword", _index, _keyWordNames, _popUpStyle);
            if (_hasTarget)
            {
                _target = (Targets)EditorGUILayout.EnumPopup("Target(s) for effect", _target, _popUpStyle);
            }
            _value = EditorGUILayout.IntField("Value for keyword", _value, _editorStyle);

            if(GUILayout.Button("Apply new effect", _buttonStyle))
            {
                Effect newEffect = ScriptableObject.CreateInstance<Effect>();

                newEffect.HasTarget = _hasTarget;
                newEffect.Trigger = _triggerForNewEffect;

                Keyword newKeyword = (Keyword)ScriptableObject.CreateInstance(_keywords[_index].GetType());
                newKeyword.EffectValue = _value;

                AssetDatabase.CreateAsset(newKeyword, "Assets/Prefabs/Keywords/" + _effectName.Replace(" ", "") + "Keyword" + ".asset");
                newEffect.Responses.Add((Keyword)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Keywords/" + _effectName.Replace(" ", "") + "Keyword" + ".asset", _keywords[_index].GetType()));

                AssetDatabase.CreateAsset(newEffect, "Assets/Prefabs/Effects/" + _effectName.Replace(" ", "") + ".asset");
                AssetDatabase.SaveAssets();

                _cardCreationReferance.Effects.Add((Effect)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Effects/" + _effectName.Replace(" ", "") + ".asset", newEffect.GetType()));
            }
        }

    }
}
