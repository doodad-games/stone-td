using System.Collections.Generic;
using MyLibrary;
using UnityEngine;

public class SFXController : MonoBehaviour
{
    HashSet<Crystal> _crystalsThatHaveBeenGrabbedBefore = new HashSet<Crystal>();

    void OnEnable()
    {
        UIController.onStonePlacementModeChanged += HandleStonePlacementModeChanged;
        UIController.onError += HandleUIError;
        GameController.onEnterDefencePhase += HandleEnterDefencePhase;
        GameController.onStonesAwakened += HandleStonesAwakened;
        GameController.onGameOver += HandleGameOver;
        Spawner.onSpawned += HandleSpawnedInvader;
        Enemy.onAnyDied += HandleEnemyDied;
        Enemy.onAnyLostLife += HandleEnemyLostLife;
        Stone.onAnyTappedChanged += HandleStoneTapChanged;
        Tower.onCountChanged += HandleTowerCountChanged;
        FireProjectilesAtEnemies.onAnyDidFire += HandleTowerFired;
        Wall.onCountChanged += HandleWallCountChanged;
        Crystal.onAnyGrabbed += HandleCrystalGrabbed;
        Stone.onAnyMerged += HandleStoneMerged;
        EnemyPathToTarget.onAnyWillDestroyBlocker += HandleWillDestroyBlocker;
        GameController.onPlayPauseToggled += HandlePlayPauseToggled;
        GameController.onWillSkipAhead += HandleWillSkipAhead;
    }

    void OnDisable()
    {
        UIController.onStonePlacementModeChanged -= HandleStonePlacementModeChanged;
        UIController.onError -= HandleUIError;
        GameController.onEnterDefencePhase -= HandleEnterDefencePhase;
        GameController.onStonesAwakened -= HandleStonesAwakened;
        GameController.onGameOver -= HandleGameOver;
        Spawner.onSpawned -= HandleSpawnedInvader;
        Enemy.onAnyDied -= HandleEnemyDied;
        Enemy.onAnyLostLife -= HandleEnemyLostLife;
        Stone.onAnyTappedChanged -= HandleStoneTapChanged;
        Tower.onCountChanged -= HandleTowerCountChanged;
        FireProjectilesAtEnemies.onAnyDidFire -= HandleTowerFired;
        Wall.onCountChanged -= HandleWallCountChanged;
        Crystal.onAnyGrabbed -= HandleCrystalGrabbed;
        Stone.onAnyMerged -= HandleStoneMerged;
        EnemyPathToTarget.onAnyWillDestroyBlocker -= HandleWillDestroyBlocker;
        GameController.onPlayPauseToggled -= HandlePlayPauseToggled;
        GameController.onWillSkipAhead -= HandleWillSkipAhead;
    }

    void HandleStonePlacementModeChanged()
    {
        if (Refs.I.uic.StonePlacementMode == Stone.Type.None)
            SoundController.Play("Click Off");
        else SoundController.Play("Click On");
    }

    void HandleUIError() => SoundController.Play("Error");
    void HandleEnterDefencePhase() => SoundController.Play("Defence Phase Begins");
    void HandleStonesAwakened() => SoundController.Play("Stones Awaken");

    void HandleGameOver(GameOverReason reason)
    {
        if (reason == GameOverReason.Victory)
            SoundController.Play("Victory");
        else if (reason == GameOverReason.InvadersStoleCrystal)
            SoundController.Play("Lose Humans");
        else if (reason == GameOverReason.StonesBrokeCrystal)
            SoundController.Play("Lose Stones");
    }

    void HandleSpawnedInvader() => SoundController.Play("Human Spawn");

    void HandleEnemyDied(Enemy enemy)
    {
        if (enemy.GetComponent<Invader>() != null)
            SoundController.Play("Human Die");
        else if (enemy.GetComponent<Stone>() != null)
            SoundController.Play("Stone Die");
    }

    void HandleEnemyLostLife(Enemy enemy)
    {
        if (enemy.GetComponent<Invader>() != null)
            SoundController.Play("Human Hurt");
        else if (enemy.GetComponent<Stone>() != null)
            SoundController.Play("Stone Hurt");
    }

    void HandleStoneTapChanged(Stone stone)
    {
        if (stone.isTapped)
            SoundController.Play("Stone Tap");
        else SoundController.Play("Stone Untap");
    }

    void HandleTowerCountChanged(bool justIncremented)
    {
        if (justIncremented)
            SoundController.Play("Tower Place");
        else SoundController.Play("Tower Reclaim");
    }

    void HandleTowerFired() => SoundController.Play("Tower Shoot");

    void HandleWallCountChanged(bool justIncremented)
    {
        if (justIncremented)
            SoundController.Play("Wall Place");
        else SoundController.Play("Wall Reclaim");
    }

    void HandleCrystalGrabbed(Crystal crystal)
    {
        if (_crystalsThatHaveBeenGrabbedBefore.Contains(crystal))
            return;
        _crystalsThatHaveBeenGrabbedBefore.Add(crystal);

        SoundController.Play("Pickup Crystal");
    }

    void HandleStoneMerged() => SoundController.Play("Stone Merge");

    void HandleWillDestroyBlocker(PathingBlocker blocker)
    {
        if (blocker.GetComponent<Wall>() != null)
            SoundController.Play("Wall Crumble");
        else if (blocker.GetComponent<Tower>() != null)
            SoundController.Play("Tower Crumble");
    }

    void HandlePlayPauseToggled()
    {
        if (Refs.I.gc.IsPlaying)
            SoundController.Play("Unpause");
        else SoundController.Play("Pause");
    }

    void HandleWillSkipAhead(float seconds)
    {
        var soundToPlay = Mathf.Clamp((int)seconds, 1, 3);
        SoundController.Play($"Speedup {soundToPlay} S");
    }
}
