﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    [SerializeField]
    private bool _canTarget;
    [SerializeField]
    private List<Card> _targets;
    [SerializeField, Header("Card Visuals")]
    private Sprite _cardImage;
    [SerializeField]
    private bool _playerCard;

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

    public void TakeDamage(int damage)
    {
        _health -= damage;
        if(_health <= 0)
        {
            if (_playerCard)
            {
                GameManager.Instance.PlayerBoard.Remove(this);
            }
            else
            {
                GameManager.Instance.OpponentBoard.Remove(this);
            }
            Destroy(_cardGameObject);
        }

        Transform cardFront = _cardGameObject.transform.GetChild(0).transform.GetChild(0);
        
        cardFront.Find("CardHealth").GetComponent<TextMeshProUGUI>().text = _health.ToString();
    }

    public void AddHealth(int healing)
    {
        _health += healing;

        Transform cardFront = _cardGameObject.transform.GetChild(0).transform.GetChild(0);

        cardFront.Find("CardHealth").GetComponent<TextMeshProUGUI>().text = _health.ToString();
    }
}
