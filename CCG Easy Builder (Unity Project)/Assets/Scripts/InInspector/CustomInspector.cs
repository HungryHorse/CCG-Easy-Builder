using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

[CustomEditor(typeof(Hand))]
public class MyHandEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myScript = target as Hand;

        DrawDefaultInspector();

        myScript.maxHandSizeOn = GUILayout.Toggle(myScript.maxHandSizeOn, "Max Hand Size");

        if (myScript.maxHandSizeOn)
        {
            myScript.maxHandSize = EditorGUILayout.IntSlider("Maximum hand size:", myScript.maxHandSize, 1, 30);
        }
    }
}

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

[CustomEditor(typeof(GameManager))]
public class MyManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myScript = target as GameManager;

        DrawDefaultInspector();

        myScript.maxPlayerResourceEnabled = GUILayout.Toggle(myScript.maxPlayerResourceEnabled, "Maximum resource");

        if (myScript.maxPlayerResourceEnabled)
        {
            myScript.maxPlayerResource = EditorGUILayout.IntSlider("Maximum resource:", myScript.maxPlayerResource, 1, 30);
        }
    }
}

[CustomEditor(typeof(CardCreator))]
[CanEditMultipleObjects]
public class MyCardCreationEditor : Editor
{
    SerializedProperty cardCreator;

    public override void OnInspectorGUI()
    {
        var myScript = target as CardCreator;        

        DrawDefaultInspector();

        if (myScript.cardType == CardType.Creature)
        {
            myScript.attack = EditorGUILayout.IntField("Attack", myScript.attack);
            myScript.maxHealth = EditorGUILayout.IntField("Health", myScript.maxHealth);
        }

        myScript.EditorUpdate();
    }
}
