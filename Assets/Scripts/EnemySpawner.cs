using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    // Start is called before the first frame update
    void Start()
    {
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

            xPos = Random.Range(positionx - range, positionx + range);
            zPos = Random.Range(positionz - range, positionz + range);
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
        }
    }
}
