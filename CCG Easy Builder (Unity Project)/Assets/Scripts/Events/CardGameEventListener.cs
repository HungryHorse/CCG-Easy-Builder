using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardGameEventListener : MonoBehaviour
{
    [SerializeField]
    private CardGameEvent gameEvent;
    [SerializeField]
    private CardEvent cardResponse;

    private void OnEnable()
    {
        gameEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        gameEvent.UnregisterListener(this);
    }

    public void OnEventRaised(Card card)
    {
        cardResponse.Invoke(card);
    }
}
