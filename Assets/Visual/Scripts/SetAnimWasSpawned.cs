using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SetAnimWasSpawned : MonoBehaviour
{
    public string propertyName = "Was Spawned";

    void Start()
    {
        var invader = GetComponentInParent<Invader>();
        var anim = GetComponent<Animator>();
        anim.SetBool(propertyName, invader.wasSpawned);
        Destroy(this);
    }
}
