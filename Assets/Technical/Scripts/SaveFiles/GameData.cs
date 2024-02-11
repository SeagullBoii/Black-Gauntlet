using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public bool[] weapons;
    public bool[] weaponAbilities;

    public GameData()
    {
        weapons = new bool[] { true, false, false };
        weaponAbilities = new bool[] { false, false, false };
    }
}
