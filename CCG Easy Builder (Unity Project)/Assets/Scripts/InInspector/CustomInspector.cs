﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
