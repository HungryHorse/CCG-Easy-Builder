using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Effect : ScriptableObject
{
    [SerializeField]
    private Triggers _trigger;
    [SerializeField]
    private ResponseTypes _responseType;
    [SerializeField]
    private string[] _triggerCardNames;
    [SerializeField]
    private CardType _triggerCardType;
    [SerializeField]
    private bool _hasSpecificCardTriggers;
    [SerializeField]
    private List<Keyword> _responses = new List<Keyword>();
    [SerializeField]
    private Targets _target;

    [SerializeField]
    private bool hasTarget;

    public Triggers Trigger { get => _trigger; set => _trigger = value; }
    public List<Keyword> Responses { get => _responses; set => _responses = value; }
    public bool HasTarget { get => hasTarget; set => hasTarget = value; }
    public string[] TriggerCardNames { get => _triggerCardNames; set => _triggerCardNames = value; }
    public CardType TriggerCardType { get => _triggerCardType; set => _triggerCardType = value; }
    public bool HasSpecificCardTriggers { get => _hasSpecificCardTriggers; set => _hasSpecificCardTriggers = value; }
    public ResponseTypes ResponseType { get => _responseType; set => _responseType = value; }
    public Targets Target { get => _target; set => _target = value; }

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
