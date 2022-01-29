using UnityEngine;

// Assumes no movement
public class SetZProportionalToYPosition : MonoBehaviour
{
    public float factor;

    void Awake()
    {
        var pos = transform.position;
        transform.position = new Vector3(
            pos.x,
            pos.y,
            pos.y * factor
        );
        Destroy(this);
    }
}
