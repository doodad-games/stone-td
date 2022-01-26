using System;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public ToSpawn[] toSpawn;
    public Vector3 spawnOffset = new Vector3(0, -0.1f, 0);

    int _spawnI;

    void OnEnable()
    {
        Array.Sort(toSpawn, (a, b) => a.time.CompareTo(b.time));

        GameController.onTick += HandleTick;
    }

    void OnDisable() => GameController.onTick -= HandleTick;

    void HandleTick()
    {
        while (
            _spawnI < toSpawn.Length &&
            toSpawn[_spawnI].time <= (float)Refs.I.gc.time
        )
        {
            Spawn(toSpawn[_spawnI].prefab);
            ++_spawnI;
        }
    }

    void Spawn(GameObject prefab)
    {
        var obj = Instantiate(
            prefab,
            transform.position + spawnOffset,
            Quaternion.identity
        );

        var invader = obj.GetComponent<Invader>();
        if (invader != null)
            invader.wasSpawned = true;
    }
}

[Serializable]
public struct ToSpawn
{
    public GameObject prefab;
    public float time;
}
