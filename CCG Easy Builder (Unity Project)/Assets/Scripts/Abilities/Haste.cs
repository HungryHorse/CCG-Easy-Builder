using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Haste : BaseAbility
{
    public override void OnEnterEffect(Card card)
    {
        card.CanAttack = true;
    }
}
