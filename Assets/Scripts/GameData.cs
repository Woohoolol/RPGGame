using UnityEngine;
using System.Collections.Generic;
using System.Collections;
[System.Serializable]
public class GameData
{
    public float money;
    public List<int> inventoryID;
    public List<int> inventoryQuantity;
    public List<CharacterStats> playerStats;
    public int checkpointID;
    public GameData()
    {
        money = 0;
        playerStats = new List<CharacterStats>();
        for(int i = 0; i < 4; i++)
        {
            CharacterStats stat = new CharacterStats();
            stat.characterType = i;
            stat.currenthp = 100;
            stat.currentmp = 100;
            stat.level = 10;
            stat.exp = 10;
            playerStats.Add(stat);
        }
        inventoryID = new List<int>{0, 1, 2, 3, 4};
        inventoryQuantity = new List<int>{3, 3, 3, 3, 3};
        checkpointID = 0;
    }
}
