using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyword : MonoBehaviour
{
    [SerializeField]
    protected int _effectValue;
    [SerializeField]
    protected bool _hasTarget;

    protected int EffectValue { get => _effectValue; set => _effectValue = value; }

    public virtual void Efect(Card card) { }

    public virtual void Efect(Card card, Card target) { }
}
