using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;

public partial class EnemySpawner : Node
{
    [Export(PropertyHint.NodeType, "NavigationRegion3D")]
    private NavigationRegion3D navRegion; // the navigation region to spawn enemies in

    [Export(PropertyHint.NodeType, "Timer")]
    private GameTimer timer;

    [Export]
    public int spawnDelayMS = 1000; // the delay between spawning enemies

    [Export(PropertyHint.Range, "0, 100, or_greater")]
    public int maxEnemyCount = 10; // the maximum number of total enemies allowed

    [Export(PropertyHint.Range, "1, 100, or_greater")]
    private uint baseSpawnLevel = 1; // the base level of enemies it spawn

    [Export(PropertyHint.Range, "10, 100")]
    private uint levelUpTimeSeconds = 30; // the time it takes to increase the spawned enemy levels by 1

    private ulong lastSpawnedTime = 0;

    public override void _Process(double delta)
    {
        ulong currentTime = Time.GetTicksMsec();

        if((int)(currentTime - lastSpawnedTime) > spawnDelayMS)
        {
            int enemyCount = GetTree().GetNodesInGroup("Enemies").Count;
            if(enemyCount < maxEnemyCount)
            {
                if(SpawnRandomEnemy() != null)
                {
                    // sets last spawn time on successful spawn
                    lastSpawnedTime = currentTime;
                }
            }
        }
    }

    // spawns a random enemy on the navregion, returns a reference to the enemy if successful, or null if the spawning failed
    public EnemyBase SpawnRandomEnemy()
    {
        if(navRegion == null) return null;

        // select a random point in the navregion
        Vector3 spawnLocation = NavigationServer3D.MapGetRandomPoint(navRegion.GetNavigationMap(), navRegion.NavigationLayers, true);
        // prevent spawning too close to a player
        List<Vector3> playerPositions = GetTree().GetNodesInGroup("Players").Select(p => ((PlayerCharacter)p).GlobalPosition).ToList();
        int tooClosePlayerCount = playerPositions.Where(p => p.DistanceTo(spawnLocation) < 10).Count();
        if(tooClosePlayerCount > 0) return null;

        // select a random enemy to spawn
        EnemyBase newEnemy = Globals.EnemyRepository.PickRandom().Instantiate<EnemyBase>();
        newEnemy.Level = (int)(baseSpawnLevel + (timer.TotalSeconds / levelUpTimeSeconds));
        newEnemy.Position = spawnLocation;
        // add the enemy to the scene
        GetTree().Root.AddChild(newEnemy);

        return newEnemy;
    }
}
