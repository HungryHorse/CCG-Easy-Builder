using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

[CustomEditor(typeof(Deck))]
public class MyDeckEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myScript = target as Deck;

        if (GUILayout.Button("Shuffle Deck", EditorStyles.miniButton))
        {
            myScript.ShuffleDeck();
        }

        DrawDefaultInspector();
    }
}

//[CustomEditor(typeof(GameManager))]
//public class MyManagerEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        var myScript = target as GameManager;

//        DrawDefaultInspector();

//        myScript.maxPlayerResourceEnabled = GUILayout.Toggle(myScript.maxPlayerResourceEnabled, "Maximum resource");

//        if (myScript.maxPlayerResourceEnabled)
//        {
//            myScript.maxPlayerResource = EditorGUILayout.IntSlider("Maximum resource:", myScript.maxPlayerResource, 1, 30);
//        }
//    }
//}

[CustomEditor(typeof(CardCreator))]
[CanEditMultipleObjects]
public class MyCardCreationEditor : Editor
{
    SerializedProperty cardCreator;

    public override void OnInspectorGUI()
    {
        var myScript = target as CardCreator;

        DrawDefaultInspector();

        EditorUtility.SetDirty(target);

        if (myScript.cardType == CardType.Creature)
        {
            myScript.attack = EditorGUILayout.IntField("Attack", myScript.attack);
            myScript.maxHealth = EditorGUILayout.IntField("Health", myScript.maxHealth);
        }

        if (GUILayout.Button("Create new effect", EditorStyles.miniButton))
        {
            EffectCreationWindow.ShowWindow(myScript.availableKeywords, myScript);
        }

        if (GUILayout.Button("Save Card", EditorStyles.miniButton))
        {
            myScript.filePath = EditorUtility.SaveFilePanel("Save new card", "Assets/Prefabs/Cards/", myScript.CardName.Replace(" ", ""), "asset");
            myScript.SaveCard();
        }

        myScript.EditorUpdate();
    }
}

[CustomEditor(typeof(Settings))]
[CanEditMultipleObjects]
public class MySettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myScript = target as Settings;

        base.OnInspectorGUI();

        if (GUILayout.Button("Settings Menu", EditorStyles.miniButton))
        {
            myScript.LoadFromFile();
            SettingsWindow.ShowWindow(myScript);
        }
    }
}

