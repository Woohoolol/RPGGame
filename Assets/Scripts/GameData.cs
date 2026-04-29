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
    public Vector3 lastSavedLocation;
    public int checkpoint;
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
            stat.level = 1;
            stat.exp = 1;
            playerStats.Add(stat);
        }
        inventoryID = new List<int>{0, 1, 2, 3, 4};
        inventoryQuantity = new List<int>{1, 1, 1, 1, 1};
        lastSavedLocation = new Vector3(0, 0, 0);
        checkpoint = 0;
    }
}
