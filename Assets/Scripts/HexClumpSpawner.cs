using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexClumpSpawner : MonoBehaviour
{
    public HexClump HexClumpPrefab;

    [HideInInspector] public float SecondsPerSpawn;
    [HideInInspector] public int MinimumLaserFrequency;

    public bool SpawnImmediatelyOnStart;
    public float InitialSpawnIntervalDelay;

    private float _currentSpawnInterval;
    private int _numSpawns;

    private void Start()
    {
        SecondsPerSpawn = GameManager.Settings.SecondsPerSpawn;
        MinimumLaserFrequency = GameManager.Settings.MinimumLaserFrequency;

        if (SpawnImmediatelyOnStart)
            SpawnClump();

        _currentSpawnInterval -= InitialSpawnIntervalDelay;
    }

    private void Update()
    {
        _currentSpawnInterval += Time.deltaTime;

        if (_currentSpawnInterval > SecondsPerSpawn)
        {
            _currentSpawnInterval = 0;
            SpawnClump();
        }
    }

    private void SpawnClump()
    {
        HexClump clump = Instantiate(HexClumpPrefab, transform.position, Quaternion.identity);

        bool spawnLaser = Random.Range(0, MinimumLaserFrequency) == 0;

        if (++_numSpawns % MinimumLaserFrequency == 0 || spawnLaser)
            clump.CreateLaser();
        else
            clump.CreateClump();

        if (spawnLaser) // Laser was manually spawned, reset laser spawn count.
            _numSpawns = 0;

        clump.Deploy();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var hexPiece = collision.GetComponent<HexPiece>();

        if (hexPiece != null && hexPiece.clump == null) // Piece is attached to the magnet
            GameManager.Instance.OnGameOver();
    }
}
