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
    private bool hasTarget;

    public Triggers Trigger { get => _trigger; set => _trigger = value; }
    public List<Keyword> Responses { get => _responses; set => _responses = value; }
    public bool HasTarget { get => hasTarget; set => hasTarget = value; }

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
