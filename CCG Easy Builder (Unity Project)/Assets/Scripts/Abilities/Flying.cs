using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Flying : BaseAbility
{
    public override void OnEnterEffect(Card card)
    {
        card.Flying = true;
    }
}
