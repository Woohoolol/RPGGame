using UnityEngine;
using System.Collections.Generic;
using System.Collections;
[System.Serializable]
public class GameData
{
    public float money;
    public int battleSpawn;
    public List<int> inventoryID;
    public List<int> inventoryQuantity;
    public List<CharacterStats> playerStats;
    public GameData()
    {
        money = 0;
        battleSpawn = 0;
        playerStats = new List<CharacterStats>();
    }
}
