using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public GameObject enemy;
    public int xPos;
    public int zPos;
    public int enemyCount;
    [SerializeField] int targetEnemyCount = 10;
    [SerializeField] int range = 15;
    [SerializeField] int spawnInterval = 3;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        int positionx = (int)transform.position.x;
        int positionz = (int)transform.position.z;
        while (enemyCount < targetEnemyCount)
        {
            xPos = Random.Range(positionx - range, positionx + range);
            zPos = Random.Range(positionz - range, positionz + range);
            Instantiate(enemy, new Vector3(xPos, 2.5f, zPos), Quaternion.identity);
            yield return new WaitForSeconds(spawnInterval);
            enemyCount += 1;
        }
    }
}
