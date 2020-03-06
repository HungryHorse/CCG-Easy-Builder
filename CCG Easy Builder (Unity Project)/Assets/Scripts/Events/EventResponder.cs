using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventResponder : MonoBehaviour
{
    public void RespondToDrawEvent(Card card)
    {
        if (card == GetComponent<PrefabEvents>().ThisCard)
        {
            foreach (Effect effect in card.Effects)
            {
                if (effect.Trigger == Triggers.Drawn)
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
    }

    public void RespondToCreatureEnteringBoardEvent(Card card)
    {
        if (card == GetComponent<PrefabEvents>().ThisCard)
        {
            foreach (Effect effect in card.Effects)
            {
                if (effect.Trigger == Triggers.EntersBoard)
                {
                    if (effect.HasTarget && card.Targets.Count < 1)
                    {
                        GameManager.Instance.Target(card.CardGameObject.transform.position, card, Triggers.EntersBoard);
                        return;
                    }
                }
            }

            foreach (Effect effect in card.Effects)
            {
                if (effect.Trigger == Triggers.EntersBoard)
                {
                    if (effect.HasTarget)
                    {
                        effect.PerformEffect(card, card.Targets[0]);
                        Debug.Log("Damage Call");
                    }
                    else
                    {
                        effect.PerformEffect(card);
                    }
                }
            }
        }
    }

    public void RespondToPlayEvent(Card card)
    {

    }
}
