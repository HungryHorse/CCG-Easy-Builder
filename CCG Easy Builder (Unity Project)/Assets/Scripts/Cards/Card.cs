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
    private int _cost;
    [SerializeField]
    private int _health;
    [SerializeField]
    private int _attack;
    [SerializeField]
    private CardType _cardType;
    [SerializeField, Header("Card Visuals")]
    private Sprite _cardImage;
    [SerializeField]
    private bool _canTarget;
    [SerializeField]
    private List<Card> _targets;

    private GameObject _cardGameObject;

    public string CardName { get => _cardName; set => _cardName = value; }
    public string Description { get => _description; set => _description = value; }
    public int Health { get => _health; set => _health = value; }
    public int Attack { get => _attack; set => _attack = value; }
    public CardType CardType { get => _cardType; set => _cardType = value; }
    public int Cost { get => _cost; set => _cost = value; }
    public Sprite CardImage { get => _cardImage; set => _cardImage = value; }
    public GameObject CardGameObject { get => _cardGameObject; set => _cardGameObject = value; }
    public bool CanTarget { get => _canTarget; set => _canTarget = value; }
    public List<Card> Targets { get => _targets; set => _targets = value; }
}
