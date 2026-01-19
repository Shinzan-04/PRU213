using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component responsible of spawning wolves, given set respawn points and spawn frequency values.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    // === CÁC BIẾN PREFAB ===
    [SerializeField] Transform wolfPrefab;
    [SerializeField] Transform wolfEaterPrefab;
    [SerializeField] Transform enemy00Prefab;
    [SerializeField] Transform enemy01Prefab;
    [SerializeField] Transform BossPrefab;

    [SerializeField] Transform[] spawnPoints;

    [SerializeField] int eaterChance = 3;       // Tỷ lệ sinh của nhóm kẻ thù khó
    [SerializeField] float spawnTime;
    [SerializeField] float spawnReductionPer;
    [SerializeField] float spawnFloor;
    [SerializeField] float bossSpawnTime = 20f; // Boss xuất hiện khi còn 20s

    Manager gameManager;

    // Danh sách kẻ thù khó (Khởi tạo trước để tái sử dụng)
    private Transform[] hardEnemies;
    // Danh sách kẻ thù thường (Khởi tạo trước để tái sử dụng)
    private Transform[] commonEnemies;

    float currentSpawnTime;
    float timer;
    bool bossSpawned = false;

    void Start()
    {
        // Khởi tạo các danh sách một lần duy nhất
        // Nhóm Khó: wolfEaterPrefab và enemy00Prefab
        hardEnemies = new Transform[] { wolfEaterPrefab, enemy00Prefab };

        // Nhóm Thường: wolfPrefab và enemy01Prefab
        commonEnemies = new Transform[] { wolfPrefab, enemy01Prefab };

        currentSpawnTime = spawnTime;
        timer = Time.time;

        gameManager = FindObjectOfType<Manager>();
    }

    void Update()
    {
        if (!bossSpawned && gameManager != null && gameManager.GetTime() <= bossSpawnTime)
        {
            SpawnBoss();
            bossSpawned = true;
        }

        if (Time.time > timer)
        {
            Spawn();
            // Cơ chế giảm thời gian sinh giữ nguyên
            currentSpawnTime -= spawnReductionPer;
            if (currentSpawnTime <= spawnFloor)
            {
                currentSpawnTime = spawnFloor;
            }
            timer = Time.time + currentSpawnTime;
        }
    }

    void SpawnBoss()
    {
        Vector3 spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        Instantiate(BossPrefab, spawnPosition, Quaternion.identity);
        Debug.Log("⚠️ BOSS SPAWNED AT 20s!");
    }

    void Spawn()
    {
        // Chọn ngẫu nhiên một vị trí sinh
        Vector3 spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Length)].position;

        // Tính toán tỷ lệ sinh (Random từ 0 đến 10)
        if (Random.Range(0, 11) <= eaterChance)
        {
            // === VAI TRÒ KHÓ (Chia sẻ tỷ lệ) ===
            // Chọn ngẫu nhiên giữa wolfEaterPrefab và enemy00Prefab
            Transform enemyToSpawn = hardEnemies[Random.Range(0, hardEnemies.Length)];
            Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        }
        else
        {
            // === VAI TRÒ THƯỜNG (Chia sẻ tỷ lệ) ===
            // Chọn ngẫu nhiên giữa wolfPrefab và enemy01Prefab
            Transform enemyToSpawn = commonEnemies[Random.Range(0, commonEnemies.Length)];
            Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}