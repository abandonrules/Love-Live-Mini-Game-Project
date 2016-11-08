using UnityEngine;
using System.Collections;
using System;

public class ChibiList : IComparable<ChibiList> {
    public string name;
    public int score;

    public ChibiList(string newName, int newScore)
    {
        name = newName;
        score = newScore;
    }

    public int CompareTo(ChibiList other)
    {
        if (other == null)
        {
            return 1;
        }

        return score - other.score;
    }
}
