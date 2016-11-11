using UnityEngine;
using System.Collections;
using System;

public class ScoutingList : IComparable<ScoutingList>
{

    public string name;
    public int ID;

    public ScoutingList(string newName, int newID)
    {
        name = newName;
        ID = newID;
    }

    public int CompareTo(ScoutingList other)
    {
        if (other == null)
        {
            return 1;
        }

        return ID - other.ID;
    }
}
