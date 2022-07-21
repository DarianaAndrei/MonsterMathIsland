using System.Collections.Generic;
using System.Collections;
using TutorialAssets.Scripts;
using UnityEngine;
using TMPro;

public class MonsterManager : MonoBehaviour
{
    [SerializeField]
    private Transform _spawnPoint;

    [SerializeField]
    private Transform _queuePoint;

    [SerializeField]
    private Transform _attackPoint;

    [SerializeField]
    private Transform _killedPoint;

    [SerializeField]
    private int _amountOfMonsters = 20;

    [SerializeField]
    private GameObject[] _monsterPrefabs;

    [SerializeField]
    private float waveDifficulty;

    [SerializeField]
    private Transform _healthBarUI;

    [SerializeField]
    private TMP_Text _roundTextUI;

    public List<GameObject> _monsters;

    private int _currentRound = 0;
    private int _originalAmountOfMonsters;

    // Start is called before the first frame update
    private void Awake()
    {
        _originalAmountOfMonsters = _amountOfMonsters;
        StartWave();
    }

    public void StartWave()
    {
        _currentRound++;
        _amountOfMonsters = _originalAmountOfMonsters + Mathf.FloorToInt(_currentRound / 2f);
        _roundTextUI.text = "Wave: " + _currentRound;

        for (var i = 0; i < _amountOfMonsters; i++)
        {
            InstantiateMonster();
        }

        MonsterAttacks(0);
        MoveNextMonsterToQueue();

        CalculateWaveDifficulty(ref waveDifficulty);
    }

    //Returns a value between 0 to 1 for the difficulty of this monster wave
    private float CalculateWaveDifficulty(ref float difficulty)
    {
        foreach (var monster in _monsters)
        {
            difficulty += monster.GetComponent<MonsterController>().points;
        }

        difficulty /= (_amountOfMonsters * 3); //use 3 as 3 is the maximum points a single monster can yield

        return difficulty;
    }

    private void InstantiateMonster()
    {
        int max = _monsterPrefabs.Length;
        if (_currentRound < 3)
        {
            max = 1;
        }
        else if (_currentRound >= 3 && _currentRound < 6)
        {
            max = 2;
        }

        int monsterIndex = Mathf.FloorToInt(Random.Range(0, max));
        var monster = Instantiate(
                _monsterPrefabs[monsterIndex],
                _spawnPoint.position,
                Quaternion.identity
            )
            .GetComponent<Stats>();
        monster.level = Random.Range(0, _currentRound + 1);
        monster.LevelUp();

        _monsters.Add(monster.gameObject);
    }

    private void MoveMonsterToQueuePoint(int monsterIndex)
    {
        if (_monsters.Count <= monsterIndex)
            return;

        Transform monster = _monsters[monsterIndex].transform;
        monster.GetComponent<MonsterController>().state = MonsterState.Queue;
        StartCoroutine(LerpToPosition(monster, _queuePoint.position, _queuePoint.rotation, 0.3f));
    }

    private IEnumerator LerpToPosition(
        Transform transform,
        Vector3 position,
        Quaternion rotation,
        float speed
    )
    {
        float distanceToPosition = Vector3.Distance(transform.position, position);
        float timer = 0;
        while (distanceToPosition > 0.1f)
        {
            if (!transform)
            {
                yield break;
            }

            transform.position = Vector3.Lerp(transform.position, position, timer * speed);
            transform.rotation = rotation;
            timer += Time.deltaTime;

            yield return null;
        }
    }

    private void KillMonster(int monsterIndex)
    {
        GameObject monster = _monsters[monsterIndex];

        StopAllCoroutines();
        StartCoroutine(
            LerpToPosition(
                monster.transform,
                _killedPoint.position,
                monster.transform.rotation,
                0.6f
            )
        );

        StartCoroutine(DestroyMonster(monster, 1f));
        _monsters.RemoveAt(monsterIndex);
    }

    private IEnumerator DestroyMonster(GameObject monster, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(monster);
    }

    private void MonsterAttacks(int monsterIndex)
    {
        if (_monsters.Count <= monsterIndex)
            return;

        Transform monster = _monsters[monsterIndex].transform;
        monster.GetComponent<MonsterController>().state = MonsterState.Attack;
        StartCoroutine(LerpToPosition(monster, _attackPoint.position, _attackPoint.rotation, 0.3f));

        var monsterHealth = monster.GetComponent<Health>();
        monsterHealth.SetHealthBar(_healthBarUI);
        monsterHealth.onDeath.AddListener(MonsterDeath);
    }

    private void MonsterDeath()
    {
        KillMonster(0);
        MonsterAttacks(0);
        MoveNextMonsterToQueue();
    }

    private void MoveNextMonsterToQueue()
    {
        if (_monsters.Count <= 1)
            return;

        MoveMonsterToQueuePoint(1);
    }

    public bool IsMonsterListEmpty()
    {
        return _monsters.Count == 0;
    }

    public MonsterType GetMonsterType(int monsterIndex)
    {
        return _monsters[monsterIndex].GetComponent<MonsterController>().type;
    }
}
