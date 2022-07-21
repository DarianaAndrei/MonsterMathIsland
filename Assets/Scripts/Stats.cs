using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    public int level;
    public int exp;

    public int baseMaxHp = 54;
    public int baseAttack = 10;
    public int baseDefense = 10;

    public int maxHp;
    public int attack;
    public int defense;

    public Image experienceBar;
    public GameObject levelUpWindow;

    private int nextLevelUp;
    private int prevLvlUp;

    // Start is called before the first frame update
    void Start()
    {
        CalculateStats();
        exp = prevLvlUp;
    }

    private void CalculateStats()
    {
        prevLvlUp = (int)Mathf.Pow(level, 4f);
        nextLevelUp = (int)Mathf.Pow(level + 1, 4f);
        maxHp = baseMaxHp + (int)Mathf.Pow(level + 1, 1.2f) * 10 + (int)Random.Range(0, 10);
        attack = baseAttack + (int)Mathf.Pow(level, 1.15f);
        defense = baseDefense + (int)Mathf.Pow(level, 1.15f);
    }

    public void GainExperience(int amount)
    {
        int previousExperience = exp;
        exp += amount;

        StartCoroutine(LerpExpBar(previousExperience));

        if (exp >= nextLevelUp)
        {
            LevelUp();
        }
    }

    private IEnumerator LerpExpBar(int previousExp)
    {
        if (!experienceBar)
        {
            yield break;
        }

        float timeTakenToLerp = 0f;
        while (timeTakenToLerp < 1)
        {
            timeTakenToLerp += Time.deltaTime;

            float currentLevelExperience = exp - prevLvlUp;
            float previousLvlExp = previousExp - prevLvlUp;
            float totalLevelExperience = nextLevelUp - prevLvlUp;
            float mappedExperience = currentLevelExperience / totalLevelExperience;
            float mappedPreviousExp = previousLvlExp / totalLevelExperience;
            experienceBar.fillAmount = Mathf.Lerp(
                mappedPreviousExp,
                mappedExperience,
                timeTakenToLerp
            );

            yield return null;
        }
    }

    public void LevelUp()
    {
        float prevMaxHp = maxHp;
        float prevAttack = attack;
        float prevDefense = defense;

        level++;
        CalculateStats();

        if (!levelUpWindow)
        {
            return;
        }

        levelUpWindow.SetActive(true);
        levelUpWindow.transform.GetChild(1).GetComponent<TMP_Text>().text =
            $"Level: {level}\r\n"
            + $"Max HP: {maxHp} (+{maxHp - prevMaxHp})\r\n"
            + $"Attack: {attack} (+{attack - prevAttack})\r\n"
            + $"Defense: {defense} (+{defense - prevDefense})";

        Invoke(nameof(CloseLevelUpWindow), 3f);
    }

    private void CloseLevelUpWindow()
    {
        levelUpWindow.SetActive(false);
    }

    // Update is called once per frame
    void Update() { }
}
