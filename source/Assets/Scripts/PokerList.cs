using UnityEngine;
using System.Collections;
using System;

public class PokerList : IComparable<PokerList> {

    public string name;
    public int input;

	public PokerList(string newName, int newInput)
    {
        name = newName;
        input = newInput;
    }

    public int CompareTo(PokerList other)
    {
        if (other == null)
        {
            return 1;
        }

        return input - other.input;
    }
}
