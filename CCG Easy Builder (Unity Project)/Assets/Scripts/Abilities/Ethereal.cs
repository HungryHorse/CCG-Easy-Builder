using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Ethereal : BaseAbility
{
    public override void Effect(Card card)
    {
        if(card.TurnsSpentOnBoard > 1)
        {
            card.TakeDamage(card.Health);
        }
    }
}
