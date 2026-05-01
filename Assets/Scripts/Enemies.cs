using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Enemies
{
    //Unlike the playable characters, the biome enemies/boss are not dependent on level
    //Their stats are hard coded in the inspector
    public List<GameObject> biomeEnemies;
    public List<GameObject> biomeBosses;
    public Enemies()
    {
        biomeEnemies = new List<GameObject>();
        biomeBosses = new List<GameObject>();
    }
}
