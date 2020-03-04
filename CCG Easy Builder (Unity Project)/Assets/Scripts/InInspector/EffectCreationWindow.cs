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

    public static void ShowWindow(Keyword[] importedKeywords, CardCreator cardCreationReferance)
    {
        _cardCreationReferance = cardCreationReferance;
        _keywords = importedKeywords;
        _keyWordNames = new string[_keywords.Length];
        for(int i = 0; i < _keywords.Length; i++)
        {
            _keyWordNames[i] = _keywords[i].name;
        }
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(EffectCreationWindow), false, _cardCreationReferance.CardName + " Effect Editor");
    }

    private void OnGUI()
    {
        _triggerForNewEffect = (Triggers)EditorGUILayout.EnumPopup("Trigger for effect", _triggerForNewEffect);

        if(_triggerForNewEffect != Triggers.Null)
        {
            EditorGUILayout.Popup(_index, _keyWordNames);
        }

    }
}
