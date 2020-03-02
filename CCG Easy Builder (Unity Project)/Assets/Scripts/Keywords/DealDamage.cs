using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : Keyword
{
    new protected bool _hasTarget = true;
    public override void Efect(Card card, Card target)
    {
        target.TakeDamage(_effectValue);
    }
}
