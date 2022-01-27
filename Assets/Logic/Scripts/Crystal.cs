using System;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(EXEC_ORDER)]
public class Crystal : MonoBehaviour, IHasCollisionRadius, IPathingTarget
{
    public const int EXEC_ORDER = Invader.EXEC_ORDER - 1;

    const float CARRIER_FOLLOWER_DISTANCE_REFRESH_INTERVAL = 3f;

#pragma warning disable CS0649
    [SerializeField] GameObject _collision;
    [SerializeField] float _collisionRadius;
#pragma warning restore CS0649

    [HideInInspector] public Invader carriedBy;

    public float CollisionRadius => _collisionRadius;
    public Vector3 PathingTargetPoint => carriedBy == null ? transform.position : carriedBy.transform.position;

    float _nextCarrierFollowerRefreshTime;

    void OnEnable()
    {
        Refs.I.Crystals.Add(this);
        GameController.onTick += HandleTick;
    }
    void OnDisable()
    {
        if (Refs.I != null)
            Refs.I.Crystals.Remove(this);
        GameController.onTick -= HandleTick;
    }

    public void HandleGrabbed(Invader invader)
    {
        _collision.SetActive(false);
        transform.SetParent(invader.grabCrystalPoint, false);
        transform.localPosition = Vector3.zero;
        carriedBy = invader;
        RefreshCarrierFollowerDistances();
    }

    void HandleTick() => MaybeRefreshCarrierFollowerDistances();

    void MaybeRefreshCarrierFollowerDistances()
    {
        if (
            carriedBy != null &&
            Refs.I.gc.time > _nextCarrierFollowerRefreshTime
        ) RefreshCarrierFollowerDistances();
    }

    void RefreshCarrierFollowerDistances()
    {
        _nextCarrierFollowerRefreshTime = Refs.I.gc.time + CARRIER_FOLLOWER_DISTANCE_REFRESH_INTERVAL;

        var thisPos = transform.position;
        var followers = Refs.I.Invaders
            .Where(_ => _.targetCrystal == this && _ != carriedBy)
            .OrderBy(_ => (_.transform.position - thisPos).sqrMagnitude);

        var followerI = 0;
        foreach (var follower in followers)
        {
            if (
                follower.targetCrystal != this ||
                follower == carriedBy
            ) continue; 
            
            follower.SetCrystalFollowDistance(followerI);
            ++followerI;
        }
    }
}
