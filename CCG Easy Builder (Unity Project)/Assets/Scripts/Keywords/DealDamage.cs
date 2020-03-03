using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DealDamage : Keyword
{
    public override void Effect(Card card)
    {
        throw new System.NotImplementedException();
    }

    public override void Effect(Card card, Card target)
    {
        target.TakeDamage(_effectValue);
    }
}
