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
    public List<int> eventFlags;
    public GameData()
    {
        money = 0;
        playerStats = new List<CharacterStats>();
        for(int i = 0; i < 1; i++)
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
        eventFlags = new List<int>{1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};
        //1 = starting out story
        //2 = talking to shopkeeper
        //3 = first save
        //4 = collected all allies

    }
}
