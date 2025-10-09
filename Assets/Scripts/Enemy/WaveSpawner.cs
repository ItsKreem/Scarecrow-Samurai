using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System; // for Action

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
    public TMP_Text roundText;
    public float roundTextDuration = 2f;

    [Header("LockRoom")]
    public GameObject LockRoom;

    private int currentWaveIndex = 0;
    private List<GameObject> aliveEnemies = new List<GameObject>();
    private bool spawningWave = false;
    private bool isActive = false; // controlled by CameraRoom

    // Event fired when all waves are done
    public event Action OnAllWavesCompleted;

    void Update()
    {
        if (!isActive) return; // only run logic after CameraRoom tells us to start

        aliveEnemies.RemoveAll(e => e == null);

        if (aliveEnemies.Count == 0 && !spawningWave && currentWaveIndex < waves.Count)
        {
            StartCoroutine(SpawnNextWave());
        }

        if (currentWaveIndex >= waves.Count && aliveEnemies.Count == 0 && !spawningWave)
        {
            Debug.Log("All waves completed!");
            if (roundText != null)
                roundText.text = "All waves completed!";

            OnAllWavesCompleted?.Invoke();
            isActive = false; // stop running
        }
    }

    public void BeginSpawning()
    {
        LockRoom.SetActive(false);
        if (isActive) return;
        isActive = true;
        currentWaveIndex = 0;
        aliveEnemies.Clear();
        StartCoroutine(SpawnNextWave());
    }

    IEnumerator SpawnNextWave()
    {
        spawningWave = true;
        yield return new WaitForSeconds(timeBetweenWaves);

        if (currentWaveIndex >= waves.Count)
        {
            spawningWave = false;
            yield break;
        }

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

        List<Transform> flyingShuffled = new List<Transform>(flyingSpawnPoints);
        ShuffleList(flyingShuffled);

        for (int i = 0; i < wave.flyingEnemyCount && i < flyingShuffled.Count; i++)
        {
            Transform spawnPoint = flyingShuffled[i];
            GameObject enemy = Instantiate(flyingEnemyPrefab, spawnPoint.position, Quaternion.identity);
            aliveEnemies.Add(enemy);
            yield return new WaitForSeconds(spawnDelay);
        }

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
            int randIndex = UnityEngine.Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randIndex];
            list[randIndex] = temp;
        }
    }
}

