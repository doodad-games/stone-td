using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(EXEC_ORDER)]
public class Refs : MonoBehaviour
{
    public const int EXEC_ORDER = -100;

    public static event Action<Stone.Type> onUsedTappedStonesChanged;

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
    public HashSet<Enemy> Enemies { get; } = new HashSet<Enemy>();
    public HashSet<Spawner> Spawners { get; } = new HashSet<Spawner>();
    public HashSet<Tower> Towers { get; } = new HashSet<Tower>();

    public Camera cam;
    public GameController gc;
    public PathingSystem ps;
    public UIController uic;
    public MouseInputController mouseC;

    public Dictionary<Stone.Type, HashSet<Stone>> tappedStones = new Dictionary<Stone.Type, HashSet<Stone>>();
    public Dictionary<Stone.Type, int> usedTappedStones = new Dictionary<Stone.Type, int>();

    void OnEnable()
    {
        I = this;
        cam = Camera.main;

        foreach (Stone.Type typeObj in Enum.GetValues(typeof(Stone.Type)))
        {
            var type = (Stone.Type)typeObj;

            if (type != Stone.Type.None)
            {
                tappedStones[type] = new HashSet<Stone>();
                usedTappedStones[type] = 0;
            }
        }
    }
    void OnDisable() => I = null;

    public void UseTappedStone(Stone.Type type)
    {
        ++usedTappedStones[type];
        onUsedTappedStonesChanged?.Invoke(type);
    }

    public void ReturnTappedStone(Stone.Type type)
    {
        --usedTappedStones[type];
        onUsedTappedStonesChanged?.Invoke(type);
    }
}
