using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAbility : ScriptableObject
{
    [SerializeField]
    private string abilityName;

    protected string AbilityName { get => abilityName; set => abilityName = value; }

    public virtual void Effect() { }

    public virtual void Effect(Card card) { }
}
