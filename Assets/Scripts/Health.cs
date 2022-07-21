using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField]
    private int _hp;

    [SerializeField]
    private Transform _healthBarUI;

    public int maxHp = 150;
    public UnityEvent onDeath;

    private int imageChildIndex = 1;
    private int textChildIndex = 2;

    // Start is called before the first frame update
    void Start()
    {
        UpdateHealthBarUI();
    }

    public void TakeDamage(int damage)
    {
        _hp -= damage;
        if (_hp <= 0)
        {
            _hp = 0;
            UpdateHealthBarUI();
            onDeath.Invoke();
            return;
        }
        UpdateHealthBarUI();
    }

    public void SetHealthBar(Transform healthBarUI)
    {
        _healthBarUI = healthBarUI;
        UpdateHealthBarUI();
    }

    private void UpdateHealthBarUI()
    {
        if (!_healthBarUI)
        {
            return;
        }

        _healthBarUI.GetChild(imageChildIndex).GetComponent<Image>().fillAmount =
            (float)_hp / maxHp;

        _healthBarUI.GetChild(textChildIndex).GetComponent<TMP_Text>().text = $"{_hp} / {maxHp}";
    }

    public int CalculateDamage(Stats attacker, Stats defender)
    {
        return Mathf.Max(1, attacker.attack - defender.defense);
    }

    // Update is called once per frame
    void Update() { }
}
