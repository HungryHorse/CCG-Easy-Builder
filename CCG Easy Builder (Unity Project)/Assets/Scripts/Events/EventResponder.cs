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
            else if ((effect.Trigger == Triggers.OpponentDrawsCard && !card.PlayerCard) || (effect.Trigger == Triggers.PlayerDrawsCard && card.PlayerCard))
            {
                EffectResponse(effect, card);
            }
        }
    }

    public void RespondToCreatureEnteringBoardEvent(Card card)
    {
        if (card == prefabEvents.ThisCard)
        {
            foreach (Effect effect in card.Effects)
            {
                if (effect.Trigger == Triggers.EntersBoard)
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
        else
        {
            foreach(Effect effect in prefabEvents.ThisCard.Effects)
            {
                if (effect.Trigger == Triggers.GenericCreatureEntersBoard || (effect.Trigger == Triggers.OpponentCreatureEntersBoard && !card.PlayerCard) || (effect.Trigger == Triggers.PlayerCreatureEntersBoard && card.PlayerCard))
                {
                    EffectResponse(effect, card);
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
        else
        {
            foreach(Effect effect in prefabEvents.ThisCard.Effects)
            {
                if (effect.Trigger == Triggers.GenericCardPlayed || (effect.Trigger == Triggers.OpponentCardPlayed && !card.PlayerCard) || (effect.Trigger == Triggers.PlayerCardPlayed && card.PlayerCard))
                {
                    EffectResponse(effect, card);
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

    public void EffectResponse(Effect effect, Card card)
    {
        bool effectActive = false;
        if (effect.HasSpecificCardTriggers)
        {
            switch (effect.ResponseType)
            {
                case ResponseTypes.CardName:
                    foreach (string name in effect.TriggerCardNames)
                    {
                        if (name == card.CardName)
                        {
                            effectActive = true;
                        }
                    }
                    break;
                case ResponseTypes.Cards:
                    foreach (Card currCard in effect.TriggerCards)
                    {
                        if (currCard == card)
                        {
                            effectActive = true;
                        }
                    }
                    break;
                case ResponseTypes.CardType:
                    if (effect.TriggerCardType == card.CardType)
                    {
                        effectActive = true;
                    }
                    break;
            }
        }
        else
        {
            effectActive = true;
        }

        if (effectActive)
        {
            if (TargetingEffect(prefabEvents.ThisCard, effect))
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
