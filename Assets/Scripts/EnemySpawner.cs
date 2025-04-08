using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{

    public GameObject zombie;
    public GameObject fastZombie;
    public int xPos;
    public int zPos;
    public int zombieCount;
    private float randomNum;

    [SerializeField] int targetEnemyCount = 10;
    [SerializeField] int range = 15;
    [SerializeField] int spawnInterval = 3;
    [SerializeField] float spawnerDifficulty = 0f;

    private bool spawningComplete = false;
    public static Action<bool> OnSpawnerComplete;

    private void OnEnable()
    {
        Enemy.OnKilledEnemy += CheckSpawnerStatus;
        WaveSpawner.OnWaveComplete += NextWave;
    }
    private void OnDisable()
    {
        Enemy.OnKilledEnemy -= CheckSpawnerStatus;
        WaveSpawner.OnWaveComplete -= NextWave;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private void CheckSpawnerStatus(float score)
    {
        if (spawningComplete)
        {
            StartCoroutine(SpawnerCheck());
        }
    }

    private IEnumerator SpawnerCheck()
    {
        yield return new WaitForEndOfFrame();
        if (transform.childCount <= 0)
        {
            OnSpawnerComplete?.Invoke(true);
            spawningComplete = false;
        }
    }

    private void NextWave(int waveNum)
    {
        spawnerDifficulty = 0.2f * waveNum;
        zombieCount = 0;
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        int positionx = (int)transform.position.x;
        int positionz = (int)transform.position.z;
        float positiony = transform.position.y;

        while (zombieCount < targetEnemyCount)
        {
            randomNum = Random.value;

            xPos = Random.Range(-range, range);
            zPos = Random.Range(-range, range);
            if (randomNum < spawnerDifficulty)
            {
                GameObject newZombie = Instantiate(fastZombie, new Vector3(xPos, positiony, zPos), Quaternion.identity);
                newZombie.transform.SetParent(transform, false);
            }
            else
            {
                GameObject newZombie = Instantiate(zombie, new Vector3(xPos, positiony, zPos), Quaternion.identity);
                newZombie.transform.SetParent(transform, false);
            }
            yield return new WaitForSeconds(spawnInterval);
            zombieCount += 1;
            Debug.Log("spawned zombie");
        }
        spawningComplete = true;
        Debug.Log("spawning complete");
        CheckSpawnerStatus(1);
    }
}
