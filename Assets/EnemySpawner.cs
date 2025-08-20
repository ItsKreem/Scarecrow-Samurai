using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [Header("Flying Enemy Settings")]
    public GameObject flyingEnemyPrefab;
    public float flyingSpawnInterval = 5f;
    public int maxFlyingEnemies = 5;
    public List<Transform> flyingSpawnPoints;

    [Header("Ground Enemy Settings")]
    public GameObject groundEnemyPrefab;
    public float groundSpawnInterval = 5f;
    public int maxGroundEnemies = 5;
    public List<Transform> groundSpawnPoints;

    [Header("Round Settings")]
    public float preRoundDelay = 3f; // how long the text shows before spawning starts
    public TMP_Text roundText; // reference to a TMP text element on your Canvas
    private int currentRound = 1;

    private List<GameObject> spawnedFlyingEnemies = new List<GameObject>();
    private List<GameObject> spawnedGroundEnemies = new List<GameObject>();

    void Start()
    {
        StartCoroutine(RoundStartRoutine());
    }

    IEnumerator RoundStartRoutine()
    {
        // Show round message
        if (roundText != null)
        {
            roundText.gameObject.SetActive(true);
            roundText.text = "Round " + currentRound + " Starting...";
        }

        // Wait before starting spawns
        yield return new WaitForSeconds(preRoundDelay);

        if (roundText != null)
        {
            roundText.gameObject.SetActive(false);
        }

        // Start spawning enemies
        StartCoroutine(SpawnFlyingEnemies());
        StartCoroutine(SpawnGroundEnemies());
    }

    IEnumerator SpawnFlyingEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(flyingSpawnInterval);

            // Clean up destroyed enemies
            spawnedFlyingEnemies.RemoveAll(enemy => enemy == null);

            if (spawnedFlyingEnemies.Count < maxFlyingEnemies && flyingSpawnPoints.Count > 0)
            {
                int index = Random.Range(0, flyingSpawnPoints.Count);
                GameObject enemy = Instantiate(flyingEnemyPrefab, flyingSpawnPoints[index].position, Quaternion.identity);
                spawnedFlyingEnemies.Add(enemy);
            }
        }
    }

    IEnumerator SpawnGroundEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(groundSpawnInterval);

            // Clean up destroyed enemies
            spawnedGroundEnemies.RemoveAll(enemy => enemy == null);

            if (spawnedGroundEnemies.Count < maxGroundEnemies && groundSpawnPoints.Count > 0)
            {
                int index = Random.Range(0, groundSpawnPoints.Count);
                GameObject enemy = Instantiate(groundEnemyPrefab, groundSpawnPoints[index].position, Quaternion.identity);
                spawnedGroundEnemies.Add(enemy);
            }
        }
    }
}


