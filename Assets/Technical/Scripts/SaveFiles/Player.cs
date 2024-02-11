using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDataPersistence
{
    public bool[] unlockedWeapons;

    public void LoadData(GameData gameData)
    {
        //this.unlockedWeapons = gameData.unlockedWeapons;
    }

    public void SaveData(GameData gameData)
    {
        //gameData.unlockedWeapons = this.unlockedWeapons;
    }
}
