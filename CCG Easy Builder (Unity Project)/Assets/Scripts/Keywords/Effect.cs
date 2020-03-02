using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Triggers { Draw, Enter, Play }

public class Effect : MonoBehaviour
{
    private Triggers _trigger;
    private List<Keyword> _responses = new List<Keyword>();

    public Triggers Trigger { get => _trigger; set => _trigger = value; }

    public void PerformEffect(Card card)
    {
        foreach(Keyword keyword in _responses)
        {
            keyword.Efect(card);
        }
    }

    public void PerformEffect(Card card, Card target)
    {
        foreach (Keyword keyword in _responses)
        {
            keyword.Efect(card, target);
        }
    }
}
