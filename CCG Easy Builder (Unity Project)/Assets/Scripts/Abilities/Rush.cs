using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Rush : BaseAbility
{
    public override void OnEnterEffect(Card card)
    {
        card.CanAttackMinions = true;
    }
}
