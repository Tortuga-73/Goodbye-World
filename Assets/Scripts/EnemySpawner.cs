using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public GameObject enemy;
    public int xPos;
    public int zPos;
    public int enemyCount;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (enemyCount < 15)
        {
            xPos = Random.Range(25, 55);
            zPos = Random.Range(15, 45);
            Instantiate(enemy, new Vector3(xPos, 2.5f, zPos), Quaternion.identity);
            yield return new WaitForSeconds(3);
            enemyCount += 1;
        }
    }
}
