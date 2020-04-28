using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Stats : MonoBehaviour
{
    private int _attack;
    private HealthObject _healthObject;

    [SerializeField]
    private TextMeshProUGUI _healthText;

    public int Attack { get => _attack; set => _attack = value; }
    public HealthObject HealthObject { get => _healthObject; set => _healthObject = value; }

    private void Awake()
    {
        _healthObject = ScriptableObject.CreateInstance<HealthObject>();
        _healthObject.PlayerStats = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _healthObject.Health = GameManager.Instance.PlayerHealth;
        UpdateHealthText();
    }

    public void UpdateHealthText()
    {
        _healthText.text = _healthObject.Health.ToString();
    }
}
