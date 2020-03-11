using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Haste : BaseAbility
{
    public override void Effect(Card card)
    {
        card.CanAttack = true;
    }
}
