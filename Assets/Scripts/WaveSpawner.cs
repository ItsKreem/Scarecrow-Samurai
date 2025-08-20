using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int flyingEnemyCount;
        public int groundEnemyCount;
    }

    [Header("Enemy Prefabs")]
    public GameObject flyingEnemyPrefab;
    public GameObject groundEnemyPrefab;

    [Header("Spawn Points")]
    public List<Transform> flyingSpawnPoints;
    public List<Transform> groundSpawnPoints;

    [Header("Wave Settings")]
    public List<Wave> waves = new List<Wave>();
    public float spawnDelay = 0.5f;
    public float timeBetweenWaves = 3f;

    [Header("UI")]
    public TMP_Text roundText;           // Assign in inspector
    public float roundTextDuration = 2f; // How long the text shows before spawning

    private int currentWaveIndex = 0;
    private List<GameObject> aliveEnemies = new List<GameObject>();
    private bool spawningWave = false;

    void Start()
    {
        StartCoroutine(SpawnNextWave());
    }

    void Update()
    {
        aliveEnemies.RemoveAll(e => e == null);

        if (aliveEnemies.Count == 0 && !spawningWave && currentWaveIndex < waves.Count)
        {
            StartCoroutine(SpawnNextWave());
        }
    }

    IEnumerator SpawnNextWave()
    {
        spawningWave = true;
        yield return new WaitForSeconds(timeBetweenWaves);

        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("All waves completed!");
            if (roundText != null)
                roundText.text = "All waves completed!";
            yield break;
        }

        // Show round start text
        if (roundText != null)
        {
            roundText.gameObject.SetActive(true);
            roundText.text = $"Round {currentWaveIndex + 1} Starting!";
        }

        yield return new WaitForSeconds(roundTextDuration);

        if (roundText != null)
            roundText.gameObject.SetActive(false);

        Wave wave = waves[currentWaveIndex];
        Debug.Log($"Spawning Wave {currentWaveIndex + 1}");

        // Flying enemies
        List<Transform> flyingShuffled = new List<Transform>(flyingSpawnPoints);
        ShuffleList(flyingShuffled);

        for (int i = 0; i < wave.flyingEnemyCount && i < flyingShuffled.Count; i++)
        {
            Transform spawnPoint = flyingShuffled[i];
            GameObject enemy = Instantiate(flyingEnemyPrefab, spawnPoint.position, Quaternion.identity);
            aliveEnemies.Add(enemy);
            yield return new WaitForSeconds(spawnDelay);
        }

        // Ground enemies
        List<Transform> groundShuffled = new List<Transform>(groundSpawnPoints);
        ShuffleList(groundShuffled);

        for (int i = 0; i < wave.groundEnemyCount && i < groundShuffled.Count; i++)
        {
            Transform spawnPoint = groundShuffled[i];
            GameObject enemy = Instantiate(groundEnemyPrefab, spawnPoint.position, Quaternion.identity);
            aliveEnemies.Add(enemy);
            yield return new WaitForSeconds(spawnDelay);
        }

        currentWaveIndex++;
        spawningWave = false;
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randIndex];
            list[randIndex] = temp;
        }
    }
}