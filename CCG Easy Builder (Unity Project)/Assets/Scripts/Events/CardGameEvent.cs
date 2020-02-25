using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CardEvent : UnityEvent<Card> { };

[CreateAssetMenu]
public class CardGameEvent : GameEvent
{
    private List<CardGameEventListener> cardListeners = new List<CardGameEventListener>();

    public void RaiseCard(Card card)
    {
        for (int i = cardListeners.Count - 1; i >= 0; i--)
        {
            cardListeners[i].OnEventRaised(card);
        }
    }

    public void RegisterListener(CardGameEventListener listener)
    {
        cardListeners.Add(listener);
    }

    public void UnregisterListener(CardGameEventListener listener)
    {
        cardListeners.Remove(listener);
    }
}
