using UnityEngine;
using System.Collections;
using System;

public class Leaderboards : IComparable<Leaderboards> {

    public string name;
    public int score;

	public Leaderboards(string newName, int newScore)
    {
        name = newName;
        score = newScore;
    }

    public int CompareTo(Leaderboards other)
    {
        if (this.score == other.score)
        {
            return this.name.CompareTo(other.name);
        }
        return other.score.CompareTo(this.score);
    }
}
