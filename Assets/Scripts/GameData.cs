using UnityEngine;
using System.Collections.Generic;
using System.Collections;
[System.Serializable]
public class GameData
{
    public float exp;
    public float money;
    public int battleSpawn;
    public List<GameObject> playerList;
    public GameData()
    {
        exp = 0;
        money = 0;
        battleSpawn = 0;
        playerList = new List<GameObject>();
    }
}
