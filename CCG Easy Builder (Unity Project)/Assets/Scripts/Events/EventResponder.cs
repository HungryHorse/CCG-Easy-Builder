using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventResponder : MonoBehaviour
{
    public PrefabEvents prefabEvents;

    private void Awake()
    {
        prefabEvents = GetComponent<PrefabEvents>();
    }

    public void RespondToDrawEvent(Card card)
    {
        
        foreach (Effect effect in card.Effects)
        {
            if (card == prefabEvents.ThisCard)
            {
                if (effect.Trigger == Triggers.Drawn)
                {
                    if (TargetingEffect(card, effect))
                    {
                        return;
                    }

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
            else if (card.PlayerCard)
            {
                if (effect.Trigger == Triggers.PlayerDrawsCard)
                {
                    if (TargetingEffect(card, effect))
                    {
                        return;
                    }

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
            else
            {
                if (effect.Trigger == Triggers.OpponentDrawsCard)
                {
                    if (TargetingEffect(card, effect))
                    {
                        return;
                    }

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
        if (card == prefabEvents.ThisCard)
        {
            foreach (Effect effect in card.Effects)
            {
                if(TargetingEffect(card, effect))
                {
                    return;
                }

                if (effect.Trigger == Triggers.EntersBoard)
                {
                    if (effect.HasTarget)
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

    public void RespondToPlayEvent(Card card)
    {
        if(card == prefabEvents.ThisCard)
        {
            foreach (Effect effect in card.Effects)
            {
                if (effect.Trigger == Triggers.Played)
                {
                    if (TargetingEffect(card, effect))
                    {
                        return;
                    }
                    if (effect.HasTarget)
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

    public bool TargetingEffect(Card card, Effect effect)
    {
        if (effect.Trigger == Triggers.EntersBoard)
        {
            if (effect.HasTarget && card.Targets.Count < 1)
            {
                GameManager.Instance.Target(card.CardGameObject.transform.position, card, effect.Trigger);
                return true;
            }
        }
        else
        {
            if (effect.HasTarget && card.Targets.Count < 1)
            {
                GameManager.Instance.Target(GameManager.Instance.PlayerObject.transform.position, card, effect.Trigger);
                return true;
            }
        }
        return false;
    }
}
