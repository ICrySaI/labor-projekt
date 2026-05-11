using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SaveData : Resource
{
    public const string SavePath = "user://save_data.tres";

    [Export]
    private Godot.Collections.Array<int> scores = new Godot.Collections.Array<int>();

    // returns a sorted list of scores
    public List<int> GetScores()
    {
        List<int> result = [.. scores];
        return result.OrderDescending().ToList();
    }

    public void AddScore(int score)
    {
        scores.Add(score);
    }

    public void save()
    {
        ResourceSaver.Save(this, SavePath);
    }

    public static SaveData LoadOrCreate()
    {
        if (FileAccess.FileExists(SavePath))
        {
            return ResourceLoader.Load<SaveData>(SavePath);
        }
        else return new SaveData();
    }
}
