using System;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static event Action onSpawned;

    static Vector3 _spawnOffset = new Vector3(0, -0.52f, 0);

    public ToSpawn[] toSpawn;

    int _spawnI;

    public bool IsStillSpawning => _spawnI < toSpawn.Length;

    void OnEnable()
    {
        Array.Sort(toSpawn, (a, b) => a.time.CompareTo(b.time));

        Refs.I.Spawners.Add(this);
        GameController.onTick += HandleTick;
    }

    void OnDisable()
    {
        if (Refs.I != null)
            Refs.I.Spawners.Remove(this);

        GameController.onTick -= HandleTick;
    }

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
            transform.position + _spawnOffset,
            Quaternion.identity
        );

        var invader = obj.GetComponent<Invader>();
        if (invader != null)
            invader.wasSpawned = true;
        
        onSpawned?.Invoke();
    }
}

[Serializable]
public struct ToSpawn
{
    public GameObject prefab;
    public float time;
}
