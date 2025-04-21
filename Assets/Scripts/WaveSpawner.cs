using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private float completedSpawners;
    [SerializeField] private int spawnerCount;
    public int waveNum = 1;

    public static Action<int> OnWaveComplete;


    private void OnEnable()
    {
        EnemySpawner.OnSpawnerComplete += UpdateWave;
    }


    // Start is called before the first frame update
    void Start()
    {
        spawnerCount = transform.childCount;
    }

    private void UpdateWave(bool complete)
    {
        completedSpawners++;

        if (completedSpawners >= spawnerCount)
        {
            waveNum++;
            OnWaveComplete?.Invoke(waveNum);
            completedSpawners = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
