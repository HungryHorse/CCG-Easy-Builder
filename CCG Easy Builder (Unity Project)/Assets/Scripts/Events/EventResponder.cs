using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventResponder : MonoBehaviour
{
    public void RespondToDrawEvent(Card card)
    {
        foreach (Effect effect in card.Effects)
        {
            if (effect.Trigger == Triggers.Draw)
            {
                if (card.Targets.Count >= 1)
                {
                    effect.PerformEffect(card, card.Targets[0]);
                }
                else
                {
                    effect.PerformEffect(card);
                }
            }
        }
    }

    public void RespondToCreatureEnteringBoardEvent(Card card)
    {

    }

    public void RespondToPlayEvent(Card card)
    {

    }
}
