using Godot;
using System;
using System.ComponentModel.Design;

public partial class EnemySpawner : Node
{
    [Export(PropertyHint.NodeType, "NavigationRegion3D")]
    private NavigationRegion3D navRegion; // the navigation region to spawn enemies in

    [Export]
    public int spawnDelayMS = 1000; // the delay between spawning enemies

    [Export(PropertyHint.Range, "0, 100, or_greater")]
    public int maxEnemyCount = 10; // the maximum number of total enemies allowed

    [Export(PropertyHint.Range, "1, 100, or_greater")]
    public int spawnLevel = 1; // the level of enemies it spawns

    private ulong lastSpawnedTime = 0;

    public override void _Process(double delta)
    {
        ulong currentTime = Time.GetTicksMsec();

        if((int)(currentTime - lastSpawnedTime) > spawnDelayMS)
        {
            int enemyCount = GetTree().GetNodesInGroup("Enemies").Count;
            if(enemyCount < maxEnemyCount)
            {
                SpawnRandomEnemy();
                lastSpawnedTime = currentTime;
            }
        }
    }

    // spawns a random enemy on the navregion, returns a reference to the enemy if successful, or null if the spawning failed
    public EnemyBase SpawnRandomEnemy()
    {
        if(navRegion == null) return null;

        // select a random point in the navregion
        Vector3 spawnLocation = NavigationServer3D.MapGetRandomPoint(navRegion.GetNavigationMap(), navRegion.NavigationLayers, true);
        spawnLocation.Y += 1;
        // select a random enemy to spawn
        EnemyBase newEnemy = Globals.EnemyRepository.PickRandom().Instantiate<EnemyBase>();
        newEnemy.Level = spawnLevel;
        newEnemy.Position = spawnLocation;
        // add the enemy to the scene
        GetTree().Root.AddChild(newEnemy);

        return newEnemy;
    }
}
