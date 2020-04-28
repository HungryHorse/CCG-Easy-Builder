using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : ScriptableObject
{
    public virtual void TakeDamage(int damage) { }
    public virtual void AddHealth(int healing) { }

    public virtual int Health { set; get; }
}
