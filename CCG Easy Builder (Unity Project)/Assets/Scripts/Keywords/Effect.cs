using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Effect : ScriptableObject
{
    [SerializeField]
    private Triggers _trigger;
    [SerializeField]
    private List<Keyword> _responses = new List<Keyword>();

    [SerializeField]
    protected bool _hasTarget;

    public Triggers Trigger { get => _trigger; set => _trigger = value; }

    public void PerformEffect(Card card)
    {
        foreach(Keyword keyword in _responses)
        {
            keyword.Effect(card);
        }
    }

    public void PerformEffect(Card card, Card target)
    {
        foreach (Keyword keyword in _responses)
        {
            keyword.Effect(card, target);
        }
    }
}
