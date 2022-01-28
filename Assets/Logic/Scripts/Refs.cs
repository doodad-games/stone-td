using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(EXEC_ORDER)]
public class Refs : MonoBehaviour
{
    public const int EXEC_ORDER = -100;

    public static Refs I { get; private set; }

    public static Castle NearestCastle(Vector3 pos) =>
        Nearest<Castle>(pos, I.Castles);
    public static Crystal NearestCrystal(Vector3 pos) =>
        Nearest<Crystal>(pos, I.Crystals);

    static T Nearest<T>(Vector3 pos, HashSet<T> objects)
        where T : MonoBehaviour
    {
        var minDistSq = float.MaxValue;
        T nearest = null;

        foreach (var obj in objects)
        {
            var thatPos = obj.transform.position;
            var distSq = (pos - thatPos).sqrMagnitude;

            if (distSq < minDistSq)
            {
                minDistSq = distSq;
                nearest = obj;
            }
        }

        return nearest;
    }

    public HashSet<Castle> Castles { get; } = new HashSet<Castle>();
    public HashSet<Crystal> Crystals { get; } = new HashSet<Crystal>();
    public HashSet<Invader> Invaders { get; } = new HashSet<Invader>();

    public Camera cam;
    public GameController gc;
    public PathingSystem ps;

    void OnEnable()
    {
        I = this;
        cam = Camera.main;
    }
    void OnDisable() => I = null;
}
