using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Enemy; // Prefab
    public static GameObject Player;
    List<GameObject> Enemies;
    float distanceBetween;
    public float minZ, maxZ;
    private float zPos = 0;
    int stride = 1;
    public int minEnemy = 15;
    int xIndex = 0;
    int[] xPos;

    void Start()
    {
        distanceBetween = (maxZ - minZ) / (float)(minEnemy - 1);
        Enemies = new List<GameObject>();
        xPos = new int[minEnemy];
        for (int i = 0; i < minEnemy; i++)
        {
            if (i < 10)
            {
                xPos[i] = Random.Range(0, 9);
            }
            else if (i == 10)
            {
                xIndex = Random.Range(0, 10);
                zPos = distanceBetween * (float)xIndex;
                stride = 3;
            }

            SpawnEnemy();
        }

        Player = GameObject.FindGameObjectWithTag("Player");
    }

    public GameObject SpawnEnemy()
    {
        var enemy = Instantiate(Enemy, RandomPoint(), Quaternion.identity);
        Enemies.Add(enemy);
        return enemy;
    }
    private Vector3 RandomPoint()
    {
        float rX = -4.5f + (float)(xPos[xIndex % 10] % 9);
        var temp = new Vector3(rX, 0f, (zPos % maxZ) + minZ);
        zPos += distanceBetween * (float)stride;
        xPos[xIndex % 10] += 4 + 3 * (xIndex % 2);
        xIndex += stride;
        return temp;
    }
}
