using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject Timer;
    private Text _time;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private int _waveNumber = 0;

    private readonly float _waveTimer = 60.0f;
    private readonly float _firstWaveTimer = 10.0f;
    private float _waveTimerDelta;

    private int _enemyCount = 5;

    private void Start()
    {
        _waveTimerDelta = _firstWaveTimer;

        _time = GetComponent<Text>();
    }

    private void Update()
    {
        if (_waveTimerDelta > 0)
        {
            _waveTimerDelta -= Time.deltaTime;
        }
        else
        {
            SpawnEnemy();
            _waveTimerDelta = _waveTimer;
        }
    }

    private void SpawnEnemy()
    {
        for (int i = 0; i < _enemyCount; i++)
        {
            byte spawnerIndex = (byte)Random.Range(0, spawnPoints.Length);
            Instantiate(enemyPrefab, spawnPoints[spawnerIndex].transform.position, Quaternion.identity);
        }
    }

    private void TimerBehaviour()
    {

    }
}
