using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyword : ScriptableObject
{
    [SerializeField]
    protected int _effectValue;

    public int EffectValue { get => _effectValue; set => _effectValue = value; }

    public virtual void Effect(Card card) { }

    public virtual void Effect(Card card, Card target) { Debug.Log("Virtual"); }
}
