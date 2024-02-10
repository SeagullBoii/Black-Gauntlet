using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public bool[] unlockedWeapons;

    public GameData()
    {
        unlockedWeapons = new bool[] {true, false, false};
    }
}
