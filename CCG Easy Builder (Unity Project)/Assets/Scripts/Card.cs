using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Card : ScriptableObject
{
    // Fields should include health, attack, description and name.
    [SerializeField, Header("Card Info")]
    private string _cardName;
    [SerializeField]
    private string _description;
    [SerializeField]
    private int _health;
    [SerializeField]
    private int _attack;

    public string CardName { get => _cardName; set => _cardName = value; }
    public string Description { get => _description; set => _description = value; }
    public int Health { get => _health; set => _health = value; }
    public int Attack { get => _attack; set => _attack = value; }
}
