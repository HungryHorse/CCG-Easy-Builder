using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthObject : Target
{
    private int _health;
    private Stats _playerStats;

    public override int Health { get => _health; set => _health = value; }
    public Stats PlayerStats { get => _playerStats; set => _playerStats = value; }

    public override void TakeDamage(int damage)
    {
        _health -= damage;
        _playerStats.UpdateHealthText();
    }

    public override void AddHealth(int healing)
    {
        _health += healing;
        _playerStats.UpdateHealthText();
    }
}
