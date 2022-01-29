using UnityEngine;

public class ProjectileTargetParams : MonoBehaviour
{
    public Transform[] hitLocations;
    public float hitLocationRadius;

    void OnEnable() =>
        GetComponentInParent<Enemy>().projectileTargetParams = this;
}
