using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    private int _health;
    private int _attack;

    public int Health { get => _health; set => _health = value; }
    public int Attack { get => _attack; set => _attack = value; }

    // Start is called before the first frame update
    void Start()
    {
        _health = GameManager.Instance.PlayerHealth;
    }
}
