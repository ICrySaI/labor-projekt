using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SaveData : Resource
{
    public const string SavePath = "user://save_data.tres";

    [Export]
    private Godot.Collections.Dictionary<int, int> scores = new Godot.Collections.Dictionary<int, int>(); // scores are stored as (score, seconds)

    // returns a sorted list of scores
    public List<(int score, int seconds)> GetScores()
    {
        List<(int score, int seconds)> res = new List<(int, int)>();
        foreach (var score in scores)
        {
            res.Add((score.Key, score.Value));
        }
        return res.OrderByDescending(s => s.score).ToList();
    }

    public void AddScore(int score, int seconds)
    {
        scores.Add(score, seconds);
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
