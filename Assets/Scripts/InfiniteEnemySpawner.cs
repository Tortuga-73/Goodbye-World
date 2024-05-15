using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class InfiniteEnemySpawner : MonoBehaviour
{

    public GameObject enemy;
    public float xPos;
    public float zPos;
    public int enemyCount;

    [SerializeField] int targetEnemyCount = 10;
    [SerializeField] float range = 15;
    [SerializeField] float spawnInterval = 3;

    float timer = 0f;
    bool shouldSpawn;
    // Start is called before the first frame update
    void Update()
    {
        float positionx = transform.position.x;
        float positionz = transform.position.z;
        while ((transform.childCount < targetEnemyCount) && shouldSpawn)
        {
            xPos = Random.Range(positionx - range, positionx + range);
            zPos = Random.Range(positionz - range, positionz + range);
            GameObject Enemy = Instantiate(enemy, new Vector3(xPos, 2.5f, zPos), Quaternion.identity);
            Enemy.transform.SetParent(this.gameObject.transform);
            shouldSpawn = false;
        }
        if(timer >= spawnInterval)
        {
            timer = 0f;
            shouldSpawn = true;
        }
    }
     void FixedUpdate()
    {
        if(transform.childCount < targetEnemyCount)
        {
            timer += 0.02f;
        }
        else
        {
            timer = 0;
        }
    }
}
